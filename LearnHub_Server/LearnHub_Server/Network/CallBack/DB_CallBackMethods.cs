#define SHOW

using System;
using System.Threading;//測試

using CustomEnumType;
using Network.Packet;
using Game.Data;
using MySqlCmds;
using Client;
using Data.DataBuffer;
using Game.DataBuff;

public class DB_CallBackMethods {

    /// <summary>
    /// 註冊回調方法
    /// </summary>
    public void RegisterCallBackMethods() {
        Server.DB_Register(DataBaseType.Login, Login);              //用戶登陸 
        Server.DB_Register(DataBaseType.Check, Check);              //查詢資料（目前為測試）
        Server.DB_Register(DataBaseType.GetData, GetData);          //獲取遊戲記錄
        Server.DB_Register(DataBaseType.GetCourseData, GetCourseData);
        Server.DB_Register(DataBaseType.SearchCourse, SearchCourse);
        Server.DB_Register(DataBaseType.SelectCourse, SelectCourse);
        Server.DB_Register(DataBaseType.ExitCourse, EixtCourse);
    }

    //Delete : 封包測試函數
    private void Testing(Player player, byte[] Head, byte[] Body) {
        Thread.Sleep(3000);
        Console.WriteLine("Test Complete");
    }

    #region 一般數據
    /// <summary>
    /// Delete : 
    /// </summary>
    /// <param name="player"></param>
    /// <param name="Head"></param>
    /// <param name="Body"></param>
    private void Check(Player player, byte[] Head, byte[] Body) {
        Console.WriteLine("成功創建 數據庫線程");    //測試
    }
    #endregion

    #region 獲取指定數據
    /// <summary>
    /// 用戶查詢（登入）
    /// </summary>
    /// <param name="player"></param>
    /// <param name="Head"></param>
    /// <param name="Body"></param>
    private void Login(Player player, byte[] Head, byte[] Body) {

        Unpack Unpack = new Unpack();
        Send Send = new Send();

        long ID = Unpack.Head_SessionID(Head);                                  //解析用戶ID
        Console.WriteLine($"收到登入請求 ID: {ID}");
        
        bool isPass = MySqlCmd.Login(ID, Unpack.Body_StringData(Body));         //數據庫命令（查詢帳戶是否存在）

        if (isPass) {
            MySqlCmd.UpdateLoginTime(ID);   //更新登陸時間
            player.AccountID = ID;          //設置本地玩家ID
            Console.WriteLine($"登入成功 ID: {ID}");
        } else {
            Console.WriteLine($"登入失敗 ID: {ID}");
        }
              
        Send.BoolPacket(player, PackageType.Login, isPass);                     //發送封包
    }

    /// <summary>
    /// 發送課程資料
    /// </summary>
    /// <param name="player"></param>
    /// <param name="Head"></param>
    /// <param name="Body"></param>
    private void GetCourseData(Player player, byte[] Head, byte[] Body) {
        Get get = new Get();
        get.CourseData(player);
    }

    /// <summary>
    /// 搜尋課程
    /// </summary>
    /// <param name="player"></param>
    /// <param name="Head"></param>
    /// <param name="Body"></param>
    private void SearchCourse(Player player, byte[] Head, byte[] Body) {
        Get get = new Get();
        DataList.searchData = NetworkUtil.Deserialize<SearchData>(Body);
        get.SearchCourse(player);
    }

    /// <summary>
    /// 加選
    /// </summary>
    /// <param name="player"></param>
    /// <param name="Head"></param>
    /// <param name="Body"></param>
    private void SelectCourse(Player player, byte[] Head, byte[] Body) {
        Set set = new Set();
        Unpack unpack = new Unpack();
        Console.WriteLine($"加選課程 CourseID {unpack.Body_StringData(Body)}");//Testing
        set.SelectCourse(player, unpack.Body_StringData(Body));
    }

    /// <summary>
    /// 退選
    /// </summary>
    /// <param name="player"></param>
    /// <param name="Head"></param>
    /// <param name="Body"></param>
    private void EixtCourse(Player player, byte[] Head, byte[] Body) {
        Set set = new Set();
        Unpack unpack = new Unpack();
        Console.WriteLine($"退選課程 CourseID {unpack.Body_StringData(Body)}");//Testing
        set.ExitCourse(player, unpack.Body_StringData(Body));
    }



    /// <summary>
    /// Delete : 獲取用戶資料包
    /// </summary>
    /// <param name="player"></param>
    /// <param name="Head"></param>
    /// <param name="Body"></param>
    private void GetData(Player player, byte[] Head, byte[] Body) {
        GetData getData = new GetData(player);
        getData.Execute();
    }
    #endregion

}



