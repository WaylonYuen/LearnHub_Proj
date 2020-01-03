//#define DEBUG_THREAD
//#define MANAGEMENT

//系統自帶
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

//額外封裝(Mysql)
using MySql.Data.MySqlClient;
using MySql.Data;

//自定義 命名空間
using Setup;
using Client;
using CustomEnumType;
using Network.Packet;

//縮寫定義
//      縮寫              說明
//     -----------------------------
//     1、PK             Package
//     2、DB             DataBase     


/*宣告委派*/
public delegate void ServerCallBack(Player client, byte[] Head, byte[] Body);   //封包回調
public delegate void DataBaseCallBack(Player client, byte[] Head, byte[] Body); //數據庫回調

//封閉類
public sealed class Server {

    public static volatile bool isQuit;                                         //volatile定義: 全域時刻保持最新數據; isQuit判斷遊戲是否退出，並對線程進行調整
    private static Socket ServerSocket;                                         //服務器socket
    private ReceivePacket RecvPacket;                                           //接收封包消息類

    public static List<Player> Players;                                         //玩家集合宣告

    public static ConcurrentQueue<PK_CallBack> PK_CallBackQueue;                //封包回調方法列隊
    public static ConcurrentQueue<DB_CallBack> DB_CallBackQueue;                //數據庫回調方法列隊
    public static Dictionary<PackageType, ServerCallBack> PK_CallBacksDictionary = new Dictionary<PackageType, ServerCallBack>();           //封包類型與回調方法  
    public static Dictionary<DataBaseType, DataBaseCallBack> DB_CallBacksDictionary = new Dictionary<DataBaseType, DataBaseCallBack>();     //數據庫類型與回調方法

    private readonly DB_CallBackMethods DB_CallBackMethods = new DB_CallBackMethods();   //創建數據庫回調類
    private readonly PK_CallBackMethods PK_CallBackMethods = new PK_CallBackMethods();   //創建數據庫回調類

    //Instance : 初始化(執行及建立運行必要框架)
    public Server() {
        Players = new List<Player>();                                           //創建玩家集合

        PK_CallBackQueue = new ConcurrentQueue<PK_CallBack>();                  //創建封包回調方法列隊
        DB_CallBackQueue = new ConcurrentQueue<DB_CallBack>();                  //創建數據庫回調方法列隊

        PK_CallBackMethods.RegisterCallBackMethods();                           //註冊封包回調方法(父類：static)
        DB_CallBackMethods.RegisterCallBackMethods();                           //註冊數據庫回調方法

        ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //創建Socket
    }



    /// <summary>
    /// 啟動服務器
    /// </summary>
    public void Start() {

        #region 啟動服務器
        IPEndPoint IPEndPoint = new IPEndPoint(IPAddress.Parse(RefConnection.sHost), RefConnection.sPort);
        ServerSocket.Bind(IPEndPoint);      //初始化服務器ip地址與端口
        ServerSocket.Listen(10);            //啟動監聽(最大監聽量 10個)
        Console.WriteLine($"服務器已啟動\t Info [Gate {RefConnection.sHost}:{RefConnection.sPort} ]");//提示語
        #endregion

        #region 主線程
        //開啟等待玩家線程
        Thread Thread_HandleClient = new Thread(AwaitClient_Thread) { IsBackground = true };        //啟動後台線程 
        Thread_HandleClient.Start();                                                                //啟動線程

        //開啟封包回調方法線程
        Thread Thread_PKHandleCallBack = new Thread(PK_CallBack_Thread) { IsBackground = true };    //啟動後台線程
        Thread_PKHandleCallBack.Start();                                                            //啟動線程

        //開啟數據庫回調方法線程
        Thread Thread_DBHandleCallBack = new Thread(DB_CallBack_Thread) { IsBackground = true };    //啟動後台線程
        Thread_DBHandleCallBack.Start();                                                            //啟動線程
        #endregion

    }

    #region 線程相關方法
    /// <summary>
    /// 線程 等待玩家
    /// </summary>
    private void AwaitClient_Thread() {

        Socket ClientSocket = null;
        string[] ConnectMessage = {"Packet Info"};

        //Server持續監聽Client
        while (!isQuit) {
            try {
                //同步等待,程序会阻塞在这里（設置超時等待,線程休息）
                ClientSocket = ServerSocket.Accept();                           //为新的客户端连接创建一个Socket对象，接收並返回一個新的Socket
                string endPoint = ClientSocket.RemoteEndPoint.ToString();       //獲取客戶端唯一鍵

                //設置玩家連線資訊
                Player player = new Player(ClientSocket);                       //把PlayerSocket保存到Player資料中
                Players.Add(player);                                            //新增玩家到List集合中
                Console.WriteLine($"連接消息: {player.Socket.RemoteEndPoint} 連接成功！");

                PackageSetup(player);                                           //與Client確認封包資訊,包括玩家ID

                #region 子線程
                //開啟持續接收封包線程
                ParameterizedThreadStart ReceiveMethod = new ParameterizedThreadStart(ReceivePackage_Thread);   //開啟參數化線程，將obj傳入
                Thread Listener = new Thread(ReceiveMethod) { IsBackground = true };                            //創建新線程，用來監聽客戶端發送的消息
                Listener.Start(player);                                         //開始監聽客戶端發送的消息

                //開啟心跳確認線程(確認Client是否在線)
                ParameterizedThreadStart AliveChecking = new ParameterizedThreadStart(Alive_Thread);            //開啟參數化線程，將obj傳入
                Thread Thread_HandleAlive = new Thread(AliveChecking) { IsBackground = true };                  //啟動後台線程
                Thread_HandleAlive.Start(player);                               //啟動線程
                #endregion

            } catch (Exception ex) {
                Console.WriteLine($"#  Thread Close.\t Info [Thread Name : AwaitClient_Thread()]");
                Console.WriteLine(ex.Message);
            }
        }
    }

    /// <summary>
    /// 線程 持續接收封包,並判斷封包完整性，
    /// </summary>
    /// <param name="ClientSocket">用戶Socket</param>
    private void ReceivePackage_Thread(object ClientSocket) {

        Player player = ClientSocket as Player;
        RecvPacket = new ReceivePacket();   //創建一個讀取封包的類

        while (true) {  
            byte[] Head_Bytes = RecvPacket.Head(player);         //接收封包Head ( 0～24 Bytes )          
            if (isQuit || !player.IsConnected) {                                //判斷是否需要退出此線程（while循環）
                Offline(player);                                                //執行玩家離線程序
                break;
            }          
            byte[] Body_Bytes = RecvPacket.Body(player.Socket, Head_Bytes);     //接收封包Body ( 18～Body.Lenght Bytes ) 
            RecvPacket.CheckPacket(player, Head_Bytes, Body_Bytes);             //檢查封包
        }
        if(isQuit) Console.WriteLine($"#  Thread Close.\t Info [Thread Name : ReceivePackage_Thread()]");
    }

    /// <summary>
    /// 線程 定期發送心跳包,判斷Client是否還在線
    /// </summary>
    private void Alive_Thread(object ClientSocket) {

        Send Send = new Send();
        Player player = ClientSocket as Player;

        while (!isQuit) {
            Thread.Sleep(RefOnline.HeartBeatTime);                              //每隔30秒,發送一次心跳包
            if (player.IsConnected && player.Responese) {   
                player.Responese = false;                                       //設置本次響應為否 
                Send.BoolPacket(player, PackageType.HeartBeat, true);           //發送心跳包
            } else {                                                            //如果在線卻未響應則執行
                Console.WriteLine("#  偵測不到用戶的回覆, 執行斷線程序.");
                player.IsConnected = false;
                break;
            }
        }
        Console.WriteLine($"#  @ Thread Close.\t Info [Thread Name : Alive_Thread()]");
    }

    /// <summary>
    /// 線程 執行封包回調列隊中的回調方法
    /// </summary>
    private void PK_CallBack_Thread() {
        while (!isQuit) {
            if (PK_CallBackQueue.Count > 0)
                if (PK_CallBackQueue.TryDequeue(out PK_CallBack pK_CallBack)) {
                    pK_CallBack.Execute();
                    //Action async = pK_CallBack.Execute; //異步執行回調
                    //async.BeginInvoke(null, null);
                }
            Thread.Sleep(10);   //讓出線程（即：退出隊伍N秒重新排隊）
        }
        Console.WriteLine($"#  Thread Close.\t Info [Thread Name : PK_CallBack_Thread()]");
    }

    /// <summary>
    /// 線程 執行數據庫回調列隊中的 回調方法（包括 開啟數據庫）
    /// </summary>
    private void DB_CallBack_Thread() {

        //開啟數據庫連線
        DataBase dataBase = new DataBase(); //創建數據庫連接
        dataBase.Start();  //開啟數據庫

        while (!isQuit) {
            if (DB_CallBackQueue.Count > 0)
                if (DB_CallBackQueue.TryDequeue(out DB_CallBack dB_CallBack)) {
                    dB_CallBack.Execute();
                    //Action async = dB_CallBack.Execute;  //異步執行回調
                    //async.BeginInvoke(null, null);
                }
            Thread.Sleep(10);   //讓出線程（即：退出隊伍N秒重新排隊）
        }

        //關閉數據庫連線
        dataBase.Close(); //關閉數據庫
        Console.WriteLine($"#  Database Close.\t Info [Database Name : {RefConnection.dbName}]");
        Console.WriteLine($"#  Thread Close.\t Info [Thread Name : DataBase_CallBack_Thread()]");
    }
    #endregion

    #region 關於Server的方法
    /// <summary>
    /// 註冊 封包回調方法(靜態)
    /// </summary>
    /// <param name="packageType">封包型態</param>
    /// <param name="PK_CallBackMethod">封包回調方法</param>
    public static void PK_Register(PackageType packageType, ServerCallBack PK_CallBackMethod) {
        if (!PK_CallBacksDictionary.ContainsKey(packageType))
            PK_CallBacksDictionary.Add(packageType, PK_CallBackMethod);
        else
            Console.WriteLine("# Warning: 封包註冊了相同的回調事件");
    }

    /// <summary>
    /// 註冊 數據庫回調方法(靜態)
    /// </summary>
    /// <param name="dataBaseType">封包中關於數據庫行為型態</param>
    /// <param name="DB_DataBaseCallBack">資料庫回調方法</param>
    public static void DB_Register(DataBaseType dataBaseType, DataBaseCallBack DB_DataBaseCallBack) {
        if (!DB_CallBacksDictionary.ContainsKey(dataBaseType))
            DB_CallBacksDictionary.Add(dataBaseType, DB_DataBaseCallBack);
        else
            Console.WriteLine("# Warning: 數據庫註冊了相同的回調事件");
    }

    /// <summary>
    /// 封包資訊協議
    /// </summary>
    /// <param name="player"></param>
    private void PackageSetup(Player player) {

        Send Send = new Send();
        string[] Msg = { "Package Info" };

        #region 發送封包資訊
        player.Crccode = ServerUtil.RandomNum(111111, 999999);                  //產生隨機封包驗證碼
        player.SessionID = ServerUtil.RandomNum(111111111, 999999999);          //認證後會獲取IDKey(確保同一個用戶)
        player.EncryptionMethods = ServerUtil.EncrytionMethod;                  //告知加密方法
        Send.BoolPacket(player, PackageType.None, false);                       //發送封包資料同步資訊
        Console.WriteLine($"成功設定封包基本資訊");
        #endregion

        //測試用途
        //Unpack Unpack = new Unpack();
        //byte[] Packet = Send.PacketTest(player, PackageType.None);
        //int Crccode = Unpack.Head_CrcCode(Packet);
        //EncryptionType EncryptionType = Unpack.Head_Encryption(Packet);
        //long SessionID = Unpack.Head_SessionID(Packet);
    }

    /// <summary>
    /// 玩家離線處理
    /// </summary>
    /// <param name="player">玩家Client</param>
    public void Offline(Player player) {
        player.Socket.Shutdown(SocketShutdown.Both);    //關閉Socket 接收\發送 gate
        player.Socket.Close();                          //關閉Socket
        Players.Remove(player);                         //移除玩家
        Console.WriteLine($"#  用戶離線");
        Console.WriteLine($"#  @ Socket Close \r\n#  @ Thread Close.\t Info [Thread Name : ReceivePackage_Thread()]");
    }
    #endregion
}

public class ReceivePacket {

    Unpack Unpack = new Unpack();

    //接收封包Head
    public byte[] Head(Player player) {

        int RecvAlready;    //接收到的字節組   
        int HeadLength = RefPacket.HeadLength;          //封包Head 長度

        byte[] Head_Bytes = new byte[HeadLength];           //創建一個容器，保存封包頭對封包描述的訊息，大小為描述長度。

        //如果當前需要接收的字節數大於0 and 遊戲未退出 則循環接收
        while (HeadLength > 0) {
            byte[] RecvHead_Bytes = new byte[HeadLength];

            RecvAlready = 0;    //重置已經接收到的字節數

            //檢查緩存區是否有資料需要讀取: True為有資料, False為緩存區沒有資料
            if (!(player.Socket.Available == 0)) {                                  //為避免線程阻塞在讀取部分而設置的緩存去內容判斷
                if (HeadLength >= RecvHead_Bytes.Length)                            //防沾包：如果當前接收的字節組大於緩存區，則按緩存區大小接收，否則按剩餘需要接收的字節組接收。
                    RecvAlready = player.Socket.Receive(RecvHead_Bytes, RecvHead_Bytes.Length, 0);
                else RecvAlready = player.Socket.Receive(RecvHead_Bytes, HeadLength, 0);    
                RecvHead_Bytes.CopyTo(Head_Bytes, Head_Bytes.Length - HeadLength);  //將接收到的字節數保存       
                HeadLength -= RecvAlready;                                          //減掉已經接收到的字節數
            } else {
                Thread.Sleep(50);   //讓出線程
                if (Server.isQuit || !player.IsConnected) break; //若Client端已關閉，則跳出此while循環
            }
        }
        return Head_Bytes;
    }

    //接收封包Body
    public byte[] Body(Socket PlayerSocket, byte[] Head_Bytes) {

        int RecvAlready;    //接收到的字節組
        int BodyLength = Unpack.Head_BodyLength(Head_Bytes);//封包整體長度

        byte[] Body_Bytes = new byte[BodyLength];   //創建一個容器，大小為封包對內容描述的長度大小，用以存放內容。

        //如果當前需要接收的字節數大於0 and 遊戲未退出 則循環接收
        while (BodyLength > 0) {
            byte[] RecvBody_Bytes = new byte[BodyLength < 1024 ? BodyLength : 1024];    //若內容長度超過緩存長度，則整個緩存大小作為長度

            RecvAlready = 0;    //重置已經接收到的字節數

            //防沾包：如果當前接收的字節組大於緩存區，則按緩存區大小接收，否則按剩餘需要接收的字節組接收。
            if (BodyLength >= RecvBody_Bytes.Length)
                RecvAlready = PlayerSocket.Receive(RecvBody_Bytes, RecvBody_Bytes.Length, 0);
            else RecvAlready = PlayerSocket.Receive(RecvBody_Bytes, BodyLength, 0);         
            RecvBody_Bytes.CopyTo(Body_Bytes, Body_Bytes.Length - BodyLength);  //將接收到的字節數保存
            BodyLength -= RecvAlready;                                          //減掉已經接收到的字節數
        }
        return Body_Bytes;
    }

    /// <summary>
    /// 檢查接收到的封包是否符合規格
    /// </summary>
    /// <param name="player">來源玩家資料</param>
    /// <param name="Head_Bytes">封包描述</param>
    /// <param name="Body_Bytes">封包內容</param>
    public void CheckPacket(Player player, byte[] Head_Bytes, byte[] Body_Bytes) {
        
        //解析封包Head中的部分描述資訊
        int crcCode = Unpack.Head_CrcCode(Head_Bytes);                          //封包驗證碼
        long sessionID = Unpack.Head_SessionID(Head_Bytes);                     //用戶身份Key
        PackageType packageType = Unpack.Head_PackageType(Head_Bytes);          //封包類別
        DataBaseType dataBaseType = Unpack.Head_DataBaseType(Head_Bytes);

        bool isIDKey = player.SessionID == sessionID;                           //判斷玩家ID識別碼
        bool isCrccode = player.Crccode == crcCode;                             //判斷封包識別碼

        bool canEntry = (isIDKey && isCrccode) || PackageType.Login == packageType || DataBaseType.Login == dataBaseType; //登陸封包可以直接通過

        //決定封包是否丟棄
        if (canEntry) {
            if (Server.PK_CallBacksDictionary.ContainsKey(packageType)) {  //確認 合格封包相對應的方法是否已經註冊
                PK_CallBack callBack = new PK_CallBack(player, Head_Bytes, Body_Bytes, Server.PK_CallBacksDictionary[packageType]);  //將封包打包成列隊格式(即:添加來源玩家訊息,將封包分類)
                Server.PK_CallBackQueue.Enqueue(callBack);    //將合格的封包丟進列隊中(回調線程會在列隊中抓取封包解讀,並且根據封包類別去執行不同的方法)
            } else 
                Console.WriteLine($"#### Warning 未知的封包型態: {(int)packageType}  封包註冊表中未發現此類型態的描述！");  //未知的封包類別提示
        } else
            Console.WriteLine($"#### Warning 封包驗證碼{isCrccode} OR 用戶身份ID:{isIDKey}, 收到來源不明的封包！");   //來源不明封包的提示
    }

}

public class PK_CallBack {

    private ServerCallBack ServerCallBack { get; set; }
    private Player Player { get; set; }
    private byte[] Head { get; set; }
    private byte[] Body { get; set; }

    //此類唯一安全接口
    public PK_CallBack(Player player, byte[] head, byte[] body, ServerCallBack serverCallBack) {
        Player = player;
        Head = head;
        Body = body;
        ServerCallBack = serverCallBack;
    }

    //執行回調
    public void Execute() {
        ServerCallBack(Player, Head, Body);
    }
}

public class DB_CallBack {

    private DataBaseCallBack DataBaseCallBack { get; set; }
    private Player Player { get; set; }
    private byte[] Head { get; set; }
    private byte[] Body { get; set; }

    //此類唯一安全接口
    public DB_CallBack(Player player, byte[] head, byte[] body, DataBaseCallBack dataBaseCallBack) {
        Player = player;
        Head = head;
        Body = body;
        DataBaseCallBack = dataBaseCallBack;
    }

    //執行回調
    public void Execute() {
        DataBaseCallBack(Player, Head, Body);
    }
}
