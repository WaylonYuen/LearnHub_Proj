using System;
using System.Text;
using System.IO;
using MySql.Data.MySqlClient;

namespace Setup {

    /// <summary>
    /// 連接參數設定
    /// </summary>
    public static class RefConnection {

        //服務器
        public const string sHost = "169.254.111.178"; //192.168.0.129"; //NetworkUtil.GetLocalIPv4();                 //連接地址
        public const int sPort = 8088;                                          //連接端口

        //資料庫
        public const string dbHost = "127.0.0.1";                               //連接地址
        public const string dbUser = "root";                                    //用戶ID
        public const string dbName = "LearnHubDB";                              //數據庫名稱
        public const string dbPort = "3306";                                    //連接端口
        public const string dbFormat = "utf8;";                                 //字型協議
        private const string dbPass = "Waylon943734";                           //數據庫密碼 

        //數據庫登陸字串
        public const string DBConnStr =
            "server=" + dbHost +
            ";user=" + dbUser +
            ";database=" + dbName +
            ";port=" + dbPort +
            ";password=" + dbPass +
            ";CharSet=" + dbFormat;
    }

    public static class RefOnline {
        public const int HeartBeatTime = 30000;
    }

    /// <summary>
    /// 封包參數設定
    /// </summary>
    public static class RefPacket {
        public const int crcCode = 0;
        public const int encryptionType = 4;
        public const int sessionID = 6;
        public const int bodyLength = 14;
        public const int dataType = 18;
        public const int packageType = 20;
        public const int databaseType = 22;

        public const int HeadLength = 24;//定義封包頭長度
    }

    /// <summary>
    /// 遊戲地圖設定
    /// </summary>
    public static class RefMap {

        public static int Width = 60;        //地圖寬度
        public static int Height = 60;       //地圖高度

        public static int Floor = 1;         //預設地板數值
        public static int Wall = 0;          //預設牆壁數值
        public static int Building = 0;      //預設建築數值
    }

    public static class RefRoom {
        public static int ID = 0;            //房間ID
        public static int Members = 2;       //成員數量上限
    }

    public static class RefSetting {
        public static int TryNum = 0;
    }

}