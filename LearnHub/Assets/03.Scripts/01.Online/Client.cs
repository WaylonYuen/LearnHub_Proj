//#define DEBUG_THREAD

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

//自行封裝
using Util;
using Setup;
using CustomEnumType;   //自定義枚舉型態定義
using Network.Packet;
using LearnHub.Data;

//委派(託)宣告
public delegate void ClientCallBack(Player client, byte[] Head, byte[] Body);    //封包回調

public class Client {

    #region 全域宣告

    //物件(Struct)
    public static ConcurrentQueue<CallBack> callBackQueue = new ConcurrentQueue<CallBack>();                                    //創建回調方法列隊
    public static Dictionary<PackageType, ClientCallBack> CallBacksDictionary = new Dictionary<PackageType, ClientCallBack>();  //封包類型與回調方法

    //物件(Class)
    private readonly ReceivePacket RecvPacket = new ReceivePacket();     //接收封包消息類
    private CallBackMethods CallBackMethods = new CallBackMethods();
    #endregion

    #region 屬性
    public Socket Socket { get; private set; }
    public Player Player { get; private set; }
    #endregion

    //Instance 設定IP及端口 & 啟動連接
    public Client() {
        Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);     //獲取自身的Socket
        CallBackMethods.RegisterCallBackMethods();  //執行方法註冊
    }
  
    /// <summary>
    /// 連接服務器
    /// </summary>
    public void Connect() {
    
        IPEndPoint IPEndPoint = new IPEndPoint(IPAddress.Parse(Connection.cHost), Connection.cPort);      //建立連接IP及端口

        try {
            Socket.Connect(IPEndPoint);                 //啟動連線           
            Player = new Player(Socket);                //創建Player物件
            SetupInfo_Package();                        //與Server確認封包資訊,包括玩家ID
            Debug.Log("成功連接服務器!");
        } catch {
            Debug.Log("服務器連接失敗!");
            return;
        }

        #region 線程
        //開啟持續接收線程
        Thread Listener = new Thread(ReceivePackage_Thread) { IsBackground = true };    //創建新線程, 用來監聽客戶端加入及其發送的消息
        Listener.Start(); //啟動線程

        //開啟回調方法線程
        Thread handle = new Thread(CallBack_Thread) { IsBackground = true };    //啟動後台線程, 處理回調函數
        handle.Start(); //啟動線程
        #endregion

        Debug.Log("客戶端連接完成, 線程已啟動");
    }

    #region 線程
    /// <summary>
    /// 持續接收封包，並判斷封包完整性
    /// </summary>
    private void ReceivePackage_Thread() {
        while (true) {          
            byte[] Head_Bytes = RecvPacket.Head(Player.Socket);                 //接收封包Head ( 0～24 Bytes )           
            if (StartOnline.isQuit) break;                                      //若Client端已關閉，則跳出此while循環   //判斷是否需要退出此線程（while循環）
            byte[] Body_Bytes = RecvPacket.Body(Player.Socket, Head_Bytes);     //接收封包Body ( 18～Body.Lenght Bytes )      
            RecvPacket.CheckPacket(Player, Head_Bytes, Body_Bytes);             //將封包存入回調列隊中,等待處理
        }
        Debug.Log($"#  Thread Close.\t Info [Thread Name : ReceivePackage_Thread()]");
    }

    /// <summary>
    /// 執行回調列隊中的 回調方法線程
    /// </summary>
    private void CallBack_Thread() {
        while (!StartOnline.isQuit) {
            if (callBackQueue.Count > 0)
                if (callBackQueue.TryDequeue(out CallBack callBack)) {
                    callBack.Execute();
                    //Action async = callBack.Execute; //異步執行回調
                    //async.BeginInvoke(null, null);
                }
            Thread.Sleep(10);   //讓出線程（即：退出隊伍N秒重新排隊）
        }
        Debug.Log($"#  Thread Close.\t Info [Thread Name : CallBack_Thread()]");
    }
    #endregion

    #region 關於Client的方法
    /// <summary>
    /// 回調註冊方法
    /// </summary>
    /// <param name="packageType">封包型態</param>
    /// <param name="CallBackMethod">回調方法</param>
    public static void Register(PackageType packageType, ClientCallBack CallBackMethod) {
        if (!CallBacksDictionary.ContainsKey(packageType))
            CallBacksDictionary.Add(packageType, CallBackMethod);
        else
            Debug.Log("註冊了相同的回調事件");
    }

    /// <summary>
    /// 檢查、設定玩家資訊及封包資訊（可以改善成異步程式碼）
    /// 1、接收Server封包設定資訊
    /// 2、設定本地資料緩存
    /// 3、發送登入請求
    /// </summary>
    private void SetupInfo_Package() {

        //物件
        Unpack Unpack = new Unpack();
        Send Send = new Send();

        #region 接收封包資訊
        //接收封包: Server會發送封包資訊(封包驗證碼、加密方法)給Client
        byte[] Head_Bytes = RecvPacket.Head(Player.Socket);
        byte[] Body_Bytes = RecvPacket.Body(Player.Socket, Head_Bytes); //此處服務器需改成發送空白封包

        //解析封包 並 保存資料緩存
        Player.Crccode = Unpack.Head_CrcCode(Head_Bytes);                       //解析 封包驗證碼
        Player.SessionID = Unpack.Head_SessionID(Head_Bytes);                   //解析 封包用戶身份驗證碼
        Player.EncryptionMethods = Unpack.Head_Encryption(Head_Bytes);          //解析 封包加密方法
        #endregion

        //發動登入請求
        //Send.LoginPacket(Player, DataBaseType.Login, LocalData.Identity, LocalData.Password);      //發送用戶ID及密碼（Login資料）
    }
    #endregion
}

public class ReceivePacket {

    readonly Unpack Unpack = new Unpack();

    //接收封包Head
    public byte[] Head(Socket PlayerSocket) {

        int RecvAlready;    //接收到的字節組 
        int HeadLength = Setup.Define_Packet.HeadLength;    //封包Head 長度
        byte[] Head_Bytes = new byte[HeadLength];           //創建一個容器，保存封包頭對封包描述的訊息，大小為描述長度。

        //如果當前需要接收的字節數大於0 and 遊戲未退出 則循環接收
        while (HeadLength > 0) {
            byte[] RecvHead_Bytes = new byte[HeadLength];

            //檢查緩存區是否有資料需要讀取: True為有資料, False為緩存區沒有資料
            if (!(PlayerSocket.Available == 0)) {  //為避免線程阻塞在讀取部分而設置的緩存去內容判斷
                if (HeadLength >= RecvHead_Bytes.Length) {  //防沾包：如果當前接收的字節組大於緩存區，則按緩存區大小接收，否則按剩餘需要接收的字節組接收。
                    RecvAlready = PlayerSocket.Receive(RecvHead_Bytes, RecvHead_Bytes.Length, 0);
                } else {
                    RecvAlready = PlayerSocket.Receive(RecvHead_Bytes, HeadLength, 0);
                }

                //將接收到的字節數保存
                RecvHead_Bytes.CopyTo(Head_Bytes, Head_Bytes.Length - HeadLength);
                //減掉已經接收到的字節數
                HeadLength -= RecvAlready;

            } else {
                Thread.Sleep(50);   //讓出線程
                if (StartOnline.isQuit) //若Client端已關閉，則跳出此while循環
                    break;
            }
        }

        return Head_Bytes;
    }

    //接收封包Body
    public byte[] Body(Socket PlayerSocket, byte[] Head_Bytes) {

        int RecvAlready;                                                        //接收到的字節組
        int BodyLength = Unpack.Head_BodyLength(Head_Bytes);                    //封包整體長度
        byte[] Body_Bytes = new byte[BodyLength];                               //創建一個容器，大小為封包對內容描述的長度大小，用以存放內容。

        //如果當前需要接收的字節數大於0 and 遊戲未退出 則循環接收
        while (BodyLength > 0) {
            byte[] RecvBody_Bytes = new byte[BodyLength < 1024 ? BodyLength : 1024];    //若內容長度超過緩存長度，則整個緩存大小作為長度

            //防沾包：如果當前接收的字節組大於緩存區，則按緩存區大小接收，否則按剩餘需要接收的字節組接收。
            if (BodyLength >= RecvBody_Bytes.Length) {
                RecvAlready = PlayerSocket.Receive(RecvBody_Bytes, RecvBody_Bytes.Length, 0);
            } else {
                RecvAlready = PlayerSocket.Receive(RecvBody_Bytes, BodyLength, 0);
            }

            //將接收到的字節數保存
            RecvBody_Bytes.CopyTo(Body_Bytes, Body_Bytes.Length - BodyLength);
            //減掉已經接收到的字節數
            BodyLength -= RecvAlready;
        }
        return Body_Bytes;
    }

    //檢查封包
    public void CheckPacket(Player player, byte[] Head_Bytes, byte[] Body_Bytes) {

        //解析封包Head中的部分描述資訊
        int crcCode = Unpack.Head_CrcCode(Head_Bytes);                    //封包驗證碼
        long sessionID = Unpack.Head_SessionID(Head_Bytes);               //用戶身份Key
        PackageType packageType = Unpack.Head_PackageType(Head_Bytes);    //封包類別

        //判斷封包是否合格（檢查 Crccode 和 IDkey）
        if ((player.SessionID == sessionID) &&  (player.Crccode == crcCode)) {  //比對 封包驗證碼 及 用戶身份Key
            if (Client.CallBacksDictionary.ContainsKey(packageType)) {  //確認 合格封包的類別是否存在
                CallBack callBack = new CallBack(player, Head_Bytes, Body_Bytes, Client.CallBacksDictionary[packageType]);  //將封包打包成列隊格式(即:添加來源玩家訊息,將封包分類)
                Client.callBackQueue.Enqueue(callBack);    //將合格的封包丟進列隊中(回調線程會在列隊中抓取封包解讀,並且根據封包類別去執行不同的方法)
            } else
                Debug.Log($"#### 錯誤 未知的封包型態: {(int)packageType}  註冊表中未發現此類型態的描述！");  //未知的封包類別提示
        } else
            Debug.Log($"#### 錯誤 封包驗證碼{crcCode} OR 用戶身份ID:{sessionID},收到來源不明的封包！");   //來源不明封包的提示
    }
}

public class CallBack {

    private ClientCallBack ClientCallBack { get; set; }
    private Player Player { get; set; }
    private byte[] Head { get; set; }
    private byte[] Body { get; set; }

    //此類唯一安全接口
    public CallBack(Player player, byte[] head, byte[] body, ClientCallBack serverCallBack) {
        Player = player;
        Head = head;
        Body = body;
        ClientCallBack = serverCallBack;
    }

    //執行回調
    public void Execute() {
        ClientCallBack(Player, Head, Body);
    }
}
