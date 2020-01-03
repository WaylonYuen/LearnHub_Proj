using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Setup {

    /// <summary>
    /// 連線設定
    /// </summary>
    public static class Connection {
        public static string cHost = "169.254.111.178"; //"192.168.0.129"; //NetworkUtils.GetLocalIPv4();     //連接地址
        public static int cPort = 8088;                                             //連接端口
    }

    /// <summary>
    /// 路徑設定
    /// </summary>
    public static class SrcPath {   
        public static string ID = "PlayerID.txt";    //用戶數據位址
    }

    /// <summary>
    /// 封包設定
    /// </summary>
    public static class Define_Packet {
        public static int crcCode_SP = 0;
        public static int encryptionType_SP = 4;
        public static int sessionID_SP = 6;
        public static int bodyLength_SP = 14;
        public static int dataType_SP = 18;
        public static int packageType_SP = 20;
        public static int databaseType_SP = 22;
        public static int HeadLength = 24;//定義封包頭長度
    }

    public static class RefRoom {
        public static int ID = 0;            //房間ID
        public static int Members = 2;       //成員數量上限
        public static int RoomMembers = 0;   //目前房間中的成員數目
    }

    public static class RefSpawnPointNum {
        public static int PlayerSP = 2;
        public static int MonsterSP = 2;
    }

}