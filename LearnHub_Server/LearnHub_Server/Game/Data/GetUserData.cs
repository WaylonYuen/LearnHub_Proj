using System;

using Network.Packet;
using MySqlCmds;
using CustomEnumType;
using Client;

namespace Game.Data {

    /// <summary>
    /// 獲取
    /// </summary>
    public class GetData {

        private readonly Player player;
        private readonly Send Send;

        /// <summary>
        /// 構造器
        /// </summary>
        /// <param name="player"></param>
        public GetData(Player player) {
            this.player = player;
            Send = new Send();
        }

        public void Execute() {
            PlayerData();   //獲取玩家資料
            MapData();      //獲取地圖資料
        }

        #region 獲取資料
        /// <summary>
        /// 玩家資料
        /// </summary>
        private void PlayerData() {
            Get get = new Get();
            byte[] player_Byte = get.PlayerData((int)player.AccountID);                 //獲取資料

            if (player_Byte.Length > 0) {
                //Send.GameDataPacket(player, DataBaseType.getPlayerData, player_Byte);   //發送資料
                Console.WriteLine($"  #  @ Get PlayerData.\t Info [Get : Successful]\r\n  #  \tL Send PlayerData to Target ID '{player.AccountID}' Length '{player_Byte.Length}'");
            } else Console.WriteLine($"  #  @ Get PlayerData.\t Info [Get : Fail | PlayerData.Length = 0, 找不到相關資料]");

        }

        /// <summary>
        /// 地圖資料
        /// </summary>
        private void MapData() {
            Get get = new Get();                                                //實例化物件
            byte[] Map_Byte = get.MapData((int)player.AccountID);               //獲取資料

            //提示語
            if (Map_Byte.Length > 1000) {
                //Send.GameDataPacket(player, DataBaseType.getMapData, Map_Byte);     //發送資料
                Console.WriteLine($"  #  @ Get MapData.\t Info [Get : Successful]\r\n  #  \tL Send MapData to Target ID '{player.AccountID}' Length '{Map_Byte.Length}'");
            } else Console.WriteLine($"  #  @ Get MapData.\t Info [Get : Fail | MapData.Length < 1000, 資料長度不正確]");
        }
        #endregion

    }

}
