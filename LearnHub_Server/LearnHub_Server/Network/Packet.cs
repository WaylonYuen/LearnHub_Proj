using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

using CustomEnumType;
using Setup;
using Client;

namespace Network.Packet {

    /// <summary>
    /// 創建 及 發送封包
    /// </summary>
    public class Send {
        
        /// <summary>
        /// 測試
        /// </summary>
        /// <param name="player"></param>
        /// <param name="packageType"></param>
        /// <returns></returns>
        public byte[] PacketTest(Player player, PackageType packageType) {
            byte[] data_Byte = new byte[0];
            byte[] Packet = Build_Package(player.Crccode, packageType, DataBaseType.None, DataType.IntData, player.EncryptionMethods, player.SessionID, data_Byte);
            return Packet;
        }

        #region 構建封包 一般類型
        /// <summary>
        /// 構建封包 string類型
        /// </summary>
        /// <param name="player">玩家類</param>
        /// <param name="packageType">封包型態</param>
        /// <param name="Data">資料</param>
        public void StringPacket(Player player, PackageType packageType, string[] Data) {

            //計算MsgBody的長度
            int DataBodyLenght = 0;

            for (int i = 0; i < Data.Length; i++) {
                if (Data[i] == "")
                    break;
                DataBodyLenght += Encoding.UTF8.GetBytes(Data[i]).Length;   //計算每個字串組中的長度總和 e.g: string[3] Data = {"Hello", "Welcome", "Hi"}; 分別為： 5 + 7 + 2 = DataBodyLenght;
            }

            //定義封包體的字節數組
            byte[] data_Byte = new byte[DataBodyLenght + (Data.Length * 4)];    //保留每個字串組前4Bytes，以保存每組的字串長度。

            //紀錄存入消息體數組的字節數目前的索引位置
            int TempIndex = 0;
            for (int i = 0; i < Data.Length; i++) {

                //單個消息，單個字串組
                byte[] Temp_Bytes = Encoding.UTF8.GetBytes(Data[i]); //將第i個字串組取出

                BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Temp_Bytes.Length)).CopyTo(data_Byte, TempIndex);    //計算第i個字組長度存放到TempIndex中
                TempIndex += 4; //右邊往左邊存， 1個int = 4bytes
                Temp_Bytes.CopyTo(data_Byte, TempIndex);    //存入
                TempIndex += Temp_Bytes.Length; //加上字串組長度，索引至新位置；準備存第二組字串組。
            }

            //將所有資料打包成封包
            byte[] Packet = Build_Package(player.Crccode, packageType, DataBaseType.None, DataType.StringData, player.EncryptionMethods, player.SessionID, data_Byte); //組合封包
            Send_Packet(player.Socket, Packet);
        }

        /// <summary>
        /// 構建封包 Int型態
        /// </summary>
        /// <param name="player">玩家類</param>
        /// <param name="packageType">封包型態</param>
        /// <param name="Data">資料</param>
        public void IntPacket(Player player, PackageType packageType, int Data) {
            byte[] data_Byte = BitConverter.GetBytes(Data);    //int -> Byte[]
            byte[] Packet = Build_Package(player.Crccode, packageType, DataBaseType.None, DataType.IntData, player.EncryptionMethods, player.SessionID, data_Byte);
            Send_Packet(player.Socket, Packet);
        }

        /// <summary>
        /// 構建封包 long型態
        /// </summary>
        /// <param name="player">玩家類</param>
        /// <param name="packageType">封包型態</param>
        /// <param name="Data">資料</param>
        public void LongPacket(Player player, PackageType packageType, long Data) {
            byte[] data_Byte = BitConverter.GetBytes(Data);    //long -> Byte[]
            byte[] Packet = Build_Package(player.Crccode, packageType, DataBaseType.None, DataType.LongData, player.EncryptionMethods, player.SessionID, data_Byte);
            Send_Packet(player.Socket, Packet);
        }

        /// <summary>
        /// 構建封包 float型態
        /// </summary>
        /// <param name="player">玩家類</param>
        /// <param name="packageType">封包型態</param>
        /// <param name="Data">資料</param>
        public void FloatPacket(Player player, PackageType packageType, float Data) {
            byte[] data_Byte = BitConverter.GetBytes(Data);    //float -> Byte[]
            byte[] Packet = Build_Package(player.Crccode, packageType, DataBaseType.None, DataType.FloatData, player.EncryptionMethods, player.SessionID, data_Byte);
            Send_Packet(player.Socket, Packet);
        }

        /// <summary>
        /// 構建封包 bool型態
        /// </summary>
        /// <param name="player">玩家類</param>
        /// <param name="packageType">封包型態</param>
        /// <param name="Data">資料</param>
        public void BoolPacket(Player player, PackageType packageType, bool Data) {
            byte[] data_Byte = BitConverter.GetBytes(Data);    //bool -> Byte[]
            byte[] Packet = Build_Package(player.Crccode, packageType, DataBaseType.None, DataType.BoolData, player.EncryptionMethods, player.SessionID, data_Byte);
            Send_Packet(player.Socket, Packet);
        }

        /// <summary>
        /// 構建封包 byte型態
        /// </summary>
        /// <param name="player">玩家類</param>
        /// <param name="packageType">封包型態</param>
        /// <param name="data_Byte">資料</param>
        public void BytePacket(Player player, PackageType packageType, byte[] data_Byte) {
            byte[] Packet = Build_Package(player.Crccode, packageType, DataBaseType.None, DataType.ByteData, player.EncryptionMethods, player.SessionID, data_Byte);
            Send_Packet(player.Socket, Packet);
        }

        /// <summary>
        /// 構建封包 Vector型態
        /// </summary>
        /// <param name="player">玩家類</param>
        /// <param name="packageType">封包型態</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void VectorPacket(Player player, PackageType packageType, float x, float y, float z) {

            byte[] X_Byte = BitConverter.GetBytes(x);    //float -> Byte[]
            byte[] Y_Byte = BitConverter.GetBytes(y);
            byte[] Z_Byte = BitConverter.GetBytes(z);

            byte[] data_Byte = new byte[12];

            X_Byte.CopyTo(data_Byte, 0);
            Y_Byte.CopyTo(data_Byte, 4);
            Z_Byte.CopyTo(data_Byte, 8);

            byte[] Packet = Build_Package(player.Crccode, packageType, DataBaseType.None, DataType.FloatData, player.EncryptionMethods, player.SessionID, data_Byte);
            Send_Packet(player.Socket, Packet);
        }

        #endregion
        
        #region 構建封包 數據庫類型
        /// <summary>
        /// 構建封包 string類型
        /// </summary>
        /// <param name="player">玩家類</param>
        /// <param name="databaseType">資料型態</param>
        /// <param name="Data">資料</param>
        public void StringPacket_DB(Player player, DataBaseType databaseType, string[] Data) {

            //計算MsgBody的長度
            int DataBodyLenght = 0;

            for (int i = 0; i < Data.Length; i++) {
                if (Data[i] == "")
                    break;
                DataBodyLenght += Encoding.UTF8.GetBytes(Data[i]).Length;   //計算每個字串組中的長度總和 e.g: string[3] Data = {"Hello", "Welcome", "Hi"}; 分別為： 5 + 7 + 2 = DataBodyLenght;
            }

            //定義封包體的字節數組
            byte[] data_Byte = new byte[DataBodyLenght + (Data.Length * 4)];    //保留每個字串組前4Bytes，以保存每組的字串長度。

            //紀錄存入消息體數組的字節數目前的索引位置
            int TempIndex = 0;
            for (int i = 0; i < Data.Length; i++) {

                //單個消息，單個字串組
                byte[] Temp_Bytes = Encoding.UTF8.GetBytes(Data[i]); //將第i個字串組取出

                BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Temp_Bytes.Length)).CopyTo(data_Byte, TempIndex);    //計算第i個字組長度存放到TempIndex中
                TempIndex += 4; //右邊往左邊存， 1個int = 4bytes
                Temp_Bytes.CopyTo(data_Byte, TempIndex);    //存入
                TempIndex += Temp_Bytes.Length; //加上字串組長度，索引至新位置；準備存第二組字串組。
            }

            //將所有資料打包成封包
            byte[] Packet = Build_Package(player.Crccode, PackageType.DataBase, databaseType, DataType.StringData, player.EncryptionMethods, player.SessionID, data_Byte); //組合封包
            Send_Packet(player.Socket, Packet);
        }

        /// <summary>
        /// 構建封包 Int型態
        /// </summary>
        /// <param name="player">玩家類</param>
        /// <param name="databaseType">資料型態</param>
        /// <param name="Data">資料</param>
        public void IntPacket_DB(Player player, DataBaseType databaseType, int Data) {
            byte[] data_Byte = BitConverter.GetBytes(Data);    //int -> Byte[]
            byte[] Packet = Build_Package(player.Crccode, PackageType.DataBase, databaseType, DataType.IntData, player.EncryptionMethods, player.SessionID, data_Byte);
            Send_Packet(player.Socket, Packet);
        }

        /// <summary>
        /// 構建封包 long型態
        /// </summary>
        /// <param name="player">玩家類</param>
        /// <param name="databaseType">資料型態</param>
        /// <param name="Data">資料</param>
        public void LongPacket_DB(Player player, DataBaseType databaseType, long Data) {
            byte[] data_Byte = BitConverter.GetBytes(Data);    //long -> Byte[]
            byte[] Packet = Build_Package(player.Crccode, PackageType.DataBase, databaseType, DataType.LongData, player.EncryptionMethods, player.SessionID, data_Byte);
            Send_Packet(player.Socket, Packet);
        }

        /// <summary>
        /// 構建封包 float型態
        /// </summary>
        /// <param name="player">玩家類</param>
        /// <param name="databaseType">資料型態</param>
        /// <param name="Data">資料</param>
        public void FloatPacket_DB(Player player, DataBaseType databaseType, float Data) {
            byte[] data_Byte = BitConverter.GetBytes(Data);    //float -> Byte[]
            byte[] Packet = Build_Package(player.Crccode, PackageType.DataBase, databaseType, DataType.FloatData, player.EncryptionMethods, player.SessionID, data_Byte);
            Send_Packet(player.Socket, Packet);
        }

        /// <summary>
        /// 構建封包 bool型態
        /// </summary>
        /// <param name="player">玩家類</param>
        /// <param name="databaseType">資料型態</param>
        /// <param name="Data">資料</param>
        public void BoolPacket_DB(Player player, DataBaseType databaseType, bool Data) {
            byte[] data_Byte = BitConverter.GetBytes(Data);    //bool -> Byte[]
            byte[] Packet = Build_Package(player.Crccode, PackageType.DataBase, databaseType, DataType.BoolData, player.EncryptionMethods, player.SessionID, data_Byte);
            Send_Packet(player.Socket, Packet);
        }

        /// <summary>
        /// 構建封包 byte型態
        /// </summary>
        /// <param name="player">玩家類</param>
        /// <param name="databaseType">資料型態</param>
        /// <param name="data_Byte">資料</param>
        public void BytePacket_DB(Player player, DataBaseType databaseType, byte[] data_Byte) {
            byte[] Packet = Build_Package(player.Crccode, PackageType.DataBase, databaseType, DataType.ByteData, player.EncryptionMethods, player.SessionID, data_Byte);
            Send_Packet(player.Socket, Packet);
        }
        #endregion

        #region 構建封包 特殊類型
        /// <summary>
        /// 無內容封包
        /// </summary>
        /// <param name="player">玩家類</param>
        /// <param name="databaseType">資料型態</param>
        public void BlankPacket(Player player, DataBaseType databaseType) {
            byte[] data_Byte = new byte[0]; //空容器
            byte[] Packet = Build_Package(player.Crccode, PackageType.DataBase, databaseType, DataType.StringData, player.EncryptionMethods, player.SessionID, data_Byte); //組合封包
            Send_Packet(player.Socket, Packet);
        }

        /// <summary>
        /// 登入型封包
        /// </summary>
        /// <param name="player">玩家類</param>
        /// <param name="databaseType">資料型態</param>
        /// <param name="ID">用戶帳號</param>
        /// <param name="Passsword">用戶密碼</param>
        public void LoginPacket(Player player, DataBaseType databaseType, long ID, string[] Passsword) {
            //計算MsgBody的長度
            int DataBodyLenght = 0;

            for (int i = 0; i < Passsword.Length; i++) {
                if (Passsword[i] == "")
                    break;
                DataBodyLenght += Encoding.UTF8.GetBytes(Passsword[i]).Length;   //計算每個字串組中的長度總和 e.g: string[3] Data = {"Hello", "Welcome", "Hi"}; 分別為： 5 + 7 + 2 = DataBodyLenght;
            }

            //定義封包體的字節數組
            byte[] data_Byte = new byte[DataBodyLenght + (Passsword.Length * 4)];    //保留每個字串組前4Bytes，以保存每組的字串長度。

            //紀錄存入消息體數組的字節數目前的索引位置
            int TempIndex = 0;
            for (int i = 0; i < Passsword.Length; i++) {

                //單個消息，單個字串組
                byte[] Temp_Bytes = Encoding.UTF8.GetBytes(Passsword[i]); //將第i個字串組取出

                BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Temp_Bytes.Length)).CopyTo(data_Byte, TempIndex);    //計算第i個字組長度存放到TempIndex中
                TempIndex += 4; //右邊往左邊存， 1個int = 4bytes
                Temp_Bytes.CopyTo(data_Byte, TempIndex);    //存入
                TempIndex += Temp_Bytes.Length; //加上字串組長度，索引至新位置；準備存第二組字串組。
            }

            //將所有資料打包成封包
            byte[] Packet = Build_Package(player.Crccode, PackageType.DataBase, databaseType, DataType.StringData, player.EncryptionMethods, ID, data_Byte); //組合封包
            Send_Packet(player.Socket, Packet);
        }
        #endregion

        #region 同步封包 同步類型
        public void VectorData(Player player, PackageType packageType, float x, float y, float z) {
            byte[] X_Byte = BitConverter.GetBytes(x);    //float -> Byte[]
            byte[] Y_Byte = BitConverter.GetBytes(y);    //float -> Byte[]
            byte[] Z_Byte = BitConverter.GetBytes(z);    //float -> Byte[]

            byte[] Vector = new byte[12];

            X_Byte.CopyTo(Vector, 0);
            Y_Byte.CopyTo(Vector, 4);
            Z_Byte.CopyTo(Vector, 8);

            byte[] Packet = Build_Package(player.Crccode, packageType, DataBaseType.None, DataType.VectorData, player.EncryptionMethods, player.SessionID, Vector);
            Send_Packet(player.Socket, Packet);
        }
        #endregion

        #region 封裝封包
        /// <summary>
        /// 封裝 一般封包  
        /// </summary>
        /// <param name="crcCode">封包驗證碼:是否收取</param>
        /// <param name="packageType">封包型態:如何做</param>
        /// <param name="databaseType">數據庫封包型態:如何做</param>
        /// <param name="dataType">資料型態:如何解析</param>
        /// <param name="encryptionType">加密方式:如何解密</param>
        /// <param name="sessionID">身份驗證:ID訊息</param>
        /// <param name="data_Byte">內容</param>
        private byte[] Build_Package(int crcCode, PackageType packageType, DataBaseType databaseType, DataType dataType, EncryptionType encryptionType, long sessionID, byte[] data_Byte) {

            /*格式轉換*/
            byte[] crcCode_Byte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(crcCode));                         //Crcode            (int)       封包驗證碼       
            byte[] encryptionType_Byte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)encryptionType));    //EncryptionType    (Enum)      加密方式
            byte[] sessionID_Byte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(sessionID));                     //SessionID         (Long)      用戶身份
            byte[] bodyLenght_Byte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data_Byte.Length));             //BodyLenght        (int)       內容長度
            byte[] dataType_Byte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)dataType));                //DataType          (Enum)      資料型態
            byte[] packageType_Byte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)packageType));          //PackageType       (Enum)      封包型態
            byte[] databaseType_Byte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)databaseType));        //DatabaseType      (Enum)      數據庫型態
            
            byte[] packet = new byte[RefPacket.HeadLength + data_Byte.Length];  //定義封包容器

            /*封裝*/
            crcCode_Byte.CopyTo(packet, RefPacket.crcCode);                  //int   4 Byte 0~4
            encryptionType_Byte.CopyTo(packet, RefPacket.encryptionType);    //short 2 Byte 4~6
            sessionID_Byte.CopyTo(packet, RefPacket.sessionID);              //long  8 Byte 6~14
            bodyLenght_Byte.CopyTo(packet, RefPacket.bodyLength);            //int   4 Byte 14~18
            dataType_Byte.CopyTo(packet, RefPacket.dataType);                //short 2 Byte 18~20
            packageType_Byte.CopyTo(packet, RefPacket.packageType);          //short 2 Byte 20~22
            databaseType_Byte.CopyTo(packet, RefPacket.databaseType);        //short 2 Byte 22~24           
            data_Byte.CopyTo(packet, RefPacket.HeadLength);                     //HeadLength ~ [BodyLenght+Headlength]

            return packet;
        }
        #endregion

        /// <summary>
        /// 發送封包
        /// </summary>
        /// <param name="Target">發送對象</param>
        /// <param name="Packet">封包</param>
        private void Send_Packet(Socket Target, byte[] Packet) {
            if (Target.Connected) {
                try {
                    Target.Send(Packet, Packet.Length, 0);  //發送封包
                } catch (Exception ex) {
                    Console.WriteLine($"封包發送失敗:{ex.Message}");
                }
            } else {
                throw new Exception($"{Target.RemoteEndPoint}:已斷線！");
            }
        }
    }
    
    public class Unpack {
        #region 解析 Head
        /// <summary>
        /// 解析 驗證碼 int = 4 Byte; 0~4
        /// </summary>
        /// <param name="Head"></param>
        /// <returns></returns>
        public int Head_CrcCode(byte[] Head) {
            byte[] Temp_Byte = new byte[4];        //建立容器
            Array.Copy(Head, RefPacket.crcCode, Temp_Byte, 0, 4);  //讀取資料
            int CrcCode = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(Temp_Byte, 0));    // Byte -> int
            return CrcCode;
        }

        /// <summary>
        /// 解析 加密方式 short = 2 Byte; 4~6
        /// </summary>
        /// <param name="Head">封包頭</param>
        /// <returns>資料</returns>
        public EncryptionType Head_Encryption(byte[] Head) {
            byte[] Temp_Byte = new byte[2];        //建立容器
            Array.Copy(Head, RefPacket.encryptionType, Temp_Byte, 0, 2); //讀取資料
            short Encryption = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Temp_Byte, 0));    // Byte -> short
            return (EncryptionType)Encryption;
        }

        /// <summary>
        /// 解析 用戶身份ID long = 8 Byte; 6~14
        /// </summary>
        /// <param name="Head"></param>
        /// <returns></returns>
        public long Head_SessionID(byte[] Head) {
            byte[] Temp_Byte = new byte[8];        //建立容器
            Array.Copy(Head, RefPacket.sessionID, Temp_Byte, 0, 8); //讀取資料
            long SessionID = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(Temp_Byte, 0));    // Byte -> long
            return SessionID;
        }

        /// <summary>
        /// 解析 封包Body長度 int = 4 Byte; 14~18
        /// <param name="Head"></param>
        /// </summary>
        /// <returns></returns>
        public int Head_BodyLength(byte[] Head) {
            byte[] Temp_Byte = new byte[4];        //建立容器
            Array.Copy(Head, RefPacket.bodyLength, Temp_Byte, 0, 4);  //讀取資料
            int BodyLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(Temp_Byte, 0));    // Bytes -> int
            return BodyLength;
        }

        /// <summary>
        /// 解析 資料類型 short = 2 Byte; 18~20
        /// </summary>
        /// <param name="Head">封包頭</param>
        /// <returns>資料</returns>
        public DataType Head_DataType(byte[] Head) {
            byte[] Temp_Byte = new byte[2];        //建立容器
            Array.Copy(Head, RefPacket.dataType, Temp_Byte, 0, 2); //讀取資料
            short DataType = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Temp_Byte, 0));    // Byte -> short
            return (DataType)DataType;
        }

        /// <summary>
        /// 解析 封包類型 short = 2 Byte; 20~22
        /// </summary>
        /// <param name="Head">封包頭</param>
        /// <returns>資料</returns>
        public PackageType Head_PackageType(byte[] Head) {
            byte[] Temp_Byte = new byte[2];        //建立容器
            Array.Copy(Head, RefPacket.packageType, Temp_Byte, 0, 2);  //讀取資料
            short PackageType = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Temp_Byte, 0));    // Byte -> short
            return (PackageType)PackageType;
        }

        /// <summary>
        /// 解析 數據庫類型 short = 2 Byte; 22～24
        /// </summary>
        /// <param name="Head"></param>
        /// <returns></returns>
        public DataBaseType Head_DataBaseType(byte[] Head) {
            byte[] Temp_Byte = new byte[2];        //建立容器
            Array.Copy(Head, RefPacket.databaseType, Temp_Byte, 0, 2); //讀取資料
            short DatabaseType = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Temp_Byte, 0));    // Byte -> short
            return (DataBaseType)DatabaseType;
        }
        #endregion

        #region 解析 Body
        /// <summary>
        /// 解析 String型態 (### 未解決多筆資料回傳問題) {可共享}
        /// </summary>
        /// <param name="Body">封包內容</param>
        public string Body_StringData(byte[] Body) {

            string Data = "N/A";
            for (int i = 0; i < Body.Length; i++) {

                //取出該組字串長度
                byte[] IndexLenght = new byte[4];
                Array.Copy(Body, i, IndexLenght, 0, 4);
                i += 4;
                int counter = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(IndexLenght, 0));

                //根據字串長度讀取內容 
                byte[] StringData = new byte[counter];
                Array.Copy(Body, i, StringData, 0, counter);

                i += counter;

                Data = Encoding.UTF8.GetString(StringData, 0, StringData.Length);
            }

            return Data;
        }

        /// <summary>
        /// 解析 Int型態
        /// </summary>
        /// <param name="Body">封包內容</param>
        public int Body_IntData(byte[] Body) {
            int Data = BitConverter.ToInt32(Body, 0);
            return Data;
        }

        /// <summary>
        /// 解析 Long型態
        /// </summary>
        /// <param name="Body">封包內容</param>
        public long Body_LongData(byte[] Body) {
            long Data = BitConverter.ToInt64(Body, 0);
            return Data;
        }

        /// <summary>
        /// 解析 Float型態
        /// </summary>
        /// <param name="Body">封包內容</param>
        public float Body_FloatData(byte[] Body) {
            float Data = BitConverter.ToSingle(Body, 0);
            return Data;
        }

        /// <summary>
        /// 解析 Bool型態
        /// </summary>
        /// <param name="Body">封包內容</param>
        public bool Body_BoolData(byte[] Body) {
            bool Data = BitConverter.ToBoolean(Body, 0);
            return Data;
        }

        /// <summary>
        /// 解析 Vector型態
        /// </summary>
        /// <param name="Body"></param>
        /// <param name="Ref"></param>
        /// <returns></returns>
        public float Body_VectorData(byte[] Body, Vector vector) {

            int StartPos = 0;

            switch (vector) {
                case Vector.X: StartPos = 0; break;
                case Vector.Y: StartPos = 4; break;
                case Vector.Z: StartPos = 8; break;
            }

            float Data = BitConverter.ToSingle(Body, StartPos);

            return Data;
        }

        public enum Vector {
            X,
            Y,
            Z,
        }
        #endregion
    }

    /// <summary>
    /// 封裝
    /// </summary>
    public class PackUp {
        #region 封裝內容
        public byte[] NewAccount(long NewID, string NewPW) {
            byte[] ID_Byte = BitConverter.GetBytes(NewID);                      //long -> Bytes
            byte[] Key_Byte = Encoding.UTF8.GetBytes(NewPW);                    //String -> Bytes
            byte[] Keylenght_Byte = BitConverter.GetBytes(Key_Byte.Length);     //int -> Bytes

            byte[] LoginPackage = new byte[ID_Byte.Length + Keylenght_Byte.Length + Key_Byte.Length];   //創建容器

            ID_Byte.CopyTo(LoginPackage, 0);                                    //組合內容  ID
            Keylenght_Byte.CopyTo(LoginPackage, 8);                             //組合內容  PW Lenght
            Key_Byte.CopyTo(LoginPackage, 12);                                  //組合內容  PW string

            return LoginPackage;
        }
        #endregion
    }
}
