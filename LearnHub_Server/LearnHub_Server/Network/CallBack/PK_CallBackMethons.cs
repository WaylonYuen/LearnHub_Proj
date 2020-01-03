#define SHOW

using System;
using System.IO;
using System.Net.Sockets;
using Client;
using CustomEnumType;
using Network.Packet;

using MySqlCmds;

public class PK_CallBackMethods {

    /// <summary>
    /// 註冊不同類型的封包以及其類型的解讀、執行方法
    /// </summary>
    public void RegisterCallBackMethods() {
        Server.PK_Register(PackageType.Test, Test);

        Server.PK_Register(PackageType.HeartBeat, HeartBeat);          //註冊心跳包回覆方法
        Server.PK_Register(PackageType.Connection, Connection);        //註冊連線回覆方法

        Server.PK_Register(PackageType.Login, Login);
        Server.PK_Register(PackageType.DataBase, DataBase);            //註冊調用數據庫方法
    }

    //Delete : 測試
    private void Test(Player player, byte[] Head, byte[] Body) {
        //Console.WriteLine("##### Testing");
    }

    #region 連線方法
    /// <summary>
    /// 心跳包回覆方法: 判斷玩家是否還處於連線狀態（判斷玩家是否掉線的機制）
    ///     #收到來自Client的心跳封包的處理流程:
    ///         ~確認封包驗證碼(驗證碼可以幫助Server辨識封包來源,避免黑客混入自製封包干預Server) //因為只需要加密驗證碼,所以整體加解密效率提升,無需全部加密.
    ///         ~
    /// </summary>
    /// <param name="player">玩家類結構</param>
    /// <param name="Head">封包Head</param>
    /// <param name="Body">封包Body</param>
    private void HeartBeat(Player player, byte[] Head, byte[] Body) => player.Responese = true;
    
    /// <summary>
    /// 玩家離線封包
    /// </summary>
    /// <param name="player"></param>
    /// <param name="Head"></param>
    /// <param name="Body"></param>
    private void Connection(Player player, byte[] Head, byte[] Body) {
        Unpack Unpack = new Unpack();
        player.IsConnected = Unpack.Body_BoolData(Body);    //設置玩家離線資料
    }

    /// <summary>
    /// 登入請求封包(轉發給Database)
    /// </summary>
    /// <param name="player"></param>
    /// <param name="Head"></param>
    /// <param name="Body"></param>
    private void Login(Player player, byte[] Head, byte[] Body) => DataBase(player, Head, Body);   //轉發給Database處理
    #endregion

    #region 轉發方法
    /// <summary>
    /// 調用數據庫方法（將封包轉發給數據庫線程 -> 防止數據庫調用數據過久造成封包阻塞）
    /// </summary>
    /// <param name="player">玩家類結構</param>
    /// <param name="Head">封包Head</param>
    /// <param name="Body">封包Body</param>
    private void DataBase(Player player, byte[] Head, byte[] Body) {
        Unpack Unpack = new Unpack();
        DataBaseType databaseType = Unpack.Head_DataBaseType(Head); //封包類別

        if (Server.DB_CallBacksDictionary.ContainsKey(databaseType)) {  //確認 合格封包的類別是否存在
            DB_CallBack callBack = new DB_CallBack(player, Head, Body, Server.DB_CallBacksDictionary[databaseType]); //將封包打包成列隊格式(即:添加來源玩家訊息,將封包分類)
            Server.DB_CallBackQueue.Enqueue(callBack);  //將合格的封包丟進數據庫列隊中(回調線程會在列隊中抓取封包解讀,並且根據封包類別去執行不同的方法)
        } else
            Console.WriteLine($"#### 錯誤 未知的封包型態: {(int)databaseType}  數據庫註冊表中未發現此類型態的描述！");  //未知的封包類別提示
    }
    #endregion

}