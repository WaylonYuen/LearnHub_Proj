using System;
using System.Net.Sockets;

using CustomEnumType;
using Game.DataBuff;

namespace Client {

    /// <summary>
    /// 關於與玩家的封包資訊協議
    /// </summary>
    public class Player {

        #region 網絡
        //玩家資料
        public Socket Socket { get; private set; }                              //Player Socket
        public long AccountID { get; set; }                                     //帳戶ID 

        //玩家連線狀態
        public bool IsConnected { get; set; }                                   //是否還在線
        public bool Responese { get; set; }                                     //響應回應

        //封包相關
        public int Crccode { get; set; }                                        //封包驗證碼
        public long SessionID { get; set; }                                     //身份ID
        public int EncryptionMethod { get; set; }                               //加密方式
        public EncryptionType EncryptionMethods { get; set; }                   //加密方式
        #endregion

        //Instance
        public Player(Socket socket) {
            ConnectionInitializer(socket);
        }

        /// <summary>
        /// 連線資料初始化
        /// </summary>
        /// <param name="socket"></param>
        private void ConnectionInitializer(Socket socket) {
            Socket = socket;
            AccountID = -1;
            IsConnected = true;
            Responese = true;
        }
    }
}