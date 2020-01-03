using System;

using Network.Packet;
using MySqlCmds;
using CustomEnumType;
using Client;

namespace Game.Data {
    /// <summary>
    /// 創建
    /// </summary>
    public class SetData {

        private string NewPW;
        public long NewID { get; private set; }
        public byte[] NewAccount { get; private set; }

        private readonly PackUp PackUp;

        /// <summary>
        /// 一般構造器
        /// </summary>
        public SetData() => PackUp = new PackUp();

        /// <summary>
        /// 建立 用戶
        /// </summary>
        /// <returns></returns>
        public byte[] Execute() {
            Console.WriteLine($"# 創建新用戶:");

            #region 創建資料
            AccountData();
            //GameData();
            //PlayerData();
            #endregion

            Console.WriteLine($"  #  @ Finished!\t\t Info [New ID : {NewID}]");
            Console.WriteLine("-------------------------------------------------------------");
            return PackUp.NewAccount(NewID, NewPW);
        }

        #region 建立資料
        /// <summary>
        /// 創建 新帳戶資料
        /// </summary>
        private void AccountData() {
            NewPW = ServerUtil.RandomKey(20, false);                            //獲取20位鑰匙（false = 非陣列）
            long ID = Create.Account(NewPW);                                    //建立新用戶

            if (ID != 0) {                                                      // >0則表示建立成功
                NewID = ID;
                Console.WriteLine($"  #  @ Create New AccountData.\t Info [Build : Successful]");
            } else Console.WriteLine($"  #  @ Create New AccountData.\t Info [Build : Fail]");  //### 未進行失敗例外處理
        }




        /// <summary>
        /// 創建 新帳戶遊戲資料
        /// </summary>
        private void GameData() {

            int Result = Create.MapData((int)NewID);                            //建立新遊戲檔案(地圖)

            //提示語
            if (Result == 1) Console.WriteLine($"  #  @ Create New MapData.\t Info [Build : Successful]");
            else Console.WriteLine($"  #  @ Create New MapData.\t Info [Build : Fail]");
        }

        /// <summary>
        /// Player Table
        /// </summary>
        private void PlayerData() {
            //建立新遊戲檔案(地圖)
            int Result = Create.Player((int)NewID);

            //提示語
            if (Result == 1) Console.WriteLine($"  #  @ Create New PlayerData.\t Info [Build : Successful]");
            else Console.WriteLine($"  #  @ Create New PlayerData.\t Info [Build : Fail]");
        }
        #endregion

    }
}
