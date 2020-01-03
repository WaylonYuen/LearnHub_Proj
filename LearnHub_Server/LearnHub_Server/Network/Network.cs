//#define SHOW

using System;
using System. Text;
using System.Net;
using System.Net.Sockets;

using CustomEnumType;
using Client;

public class NetworkT {

    public const int HeadLength = 24;

    #region 構建封包 {可共享} 
    #region 構建 一般型封包 [命名標準： 型態Package() ] (外部封包參數) {可共享}
    /// <summary>
    /// 構建 一般型封包 string類型
    /// </summary>
    /// <param name="Crccode"> int 封包驗證碼 4 Bytes </param>
    /// <param name="PackageType"> enum 封包型態 2 Bytes </param>
    /// <param name="SessionID"> long 登陸後用戶識別碼 4 Bytes </param>
    /// <param name="Encryption">int 加密方式 4 Bytes </param>
    /// <param name="DataBody"> string[] 封包內容 長度視來源內容</param>
    /// <returns>Package封包</returns>
    public static byte[] StringPackage(int Crccode, PackageType PackageType, long SessionID, int Encryption, string[] DataBody) {

        //計算MsgBody的長度
        int DataBodyLenght = 0;

        for (int i = 0; i < DataBody.Length; i++) {
            if (DataBody[i] == "")
                break;
            DataBodyLenght += Encoding.UTF8.GetBytes(DataBody[i]).Length;   //計算每個字串組中的長度總和 e.g: string[3] Data = {"Hello", "Welcome", "Hi"}; 分別為： 5 + 7 + 2 = DataBodyLenght;
        }

        //定義封包體的字節數組
        byte[] DataBody_Byte = new byte[DataBodyLenght + (DataBody.Length * 4)];    //保留每個字串組前4Bytes，以保存每組的字串長度。

        //紀錄存入消息體數組的字節數目前的索引位置
        int TempIndex = 0;
        for (int i = 0; i < DataBody.Length; i++) {

            //單個消息，單個字串組
            byte[] Temp_Bytes = Encoding.UTF8.GetBytes(DataBody[i]); //將第i個字串組取出

            BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Temp_Bytes.Length)).CopyTo(DataBody_Byte, TempIndex);    //計算第i個字組長度存放到TempIndex中
            TempIndex += 4; //右邊往左邊存， 1個int = 4bytes
            Temp_Bytes.CopyTo(DataBody_Byte, TempIndex);    //存入
            TempIndex += Temp_Bytes.Length; //加上字串組長度，索引至新位置；準備存第二組字串組。
        }

        //將所有資料打包成封包
        byte[] Packet = Build_Packet(Crccode, PackageType, SessionID, Encryption, DataType.StringData, DataBody_Byte); //組合封包

        return Packet;
    }

    /// <summary>
    /// 構建 一般型封包 vector類型 *注: 可用作 float類型封包構建,y,z參數填0即可(解析時只需解析x)
    /// </summary>
    /// <param name="Crccode"> int 封包驗證碼 4 Bytes </param>
    /// <param name="PackageType"> enum 封包型態 2 Bytes </param>
    /// <param name="SessionID"> long 登陸後用戶識別碼 4 Bytes </param>
    /// <param name="Encryption">int 加密方式 4 Bytes </param>
    /// <param name="x"> Vector x </param>
    /// <param name="y"> Vector y </param>
    /// <param name="z"> Vector z </param>
    /// <returns>Package封包</returns>
    public static byte[] VectorPackage(int Crccode, PackageType PackageType, long SessionID, int Encryption, float x, float y, float z) {

        byte[] x_Byte = BitConverter.GetBytes(x);
        byte[] y_Byte = BitConverter.GetBytes(y);
        byte[] z_Byte = BitConverter.GetBytes(z);

        byte[] Vector_Byte = new byte[x_Byte.Length + y_Byte.Length + z_Byte.Length];   //12 Bytes

        x_Byte.CopyTo(Vector_Byte, 0);
        y_Byte.CopyTo(Vector_Byte, 4);
        z_Byte.CopyTo(Vector_Byte, 8);

        //將所有資料打包成封包
        byte[] Packet = Build_Packet(Crccode, PackageType, SessionID, Encryption, DataType.VectorData, Vector_Byte); //組合封包

        return Packet;
    }

    /// <summary>
    /// 構建 一般型封包 int類型
    /// </summary>
    /// <param name="Crccode"> int 封包驗證碼 4 Bytes </param>
    /// <param name="PackageType"> enum 封包型態 2 Bytes </param>
    /// <param name="SessionID"> long 登陸後用戶識別碼 4 Bytes </param>
    /// <param name="Encryption">int 加密方式 4 Bytes </param>
    /// <param name="data">int資料</param>
    /// <returns>Package封包</returns>
    public static byte[] IntPackage(int Crccode, PackageType PackageType, long SessionID, int Encryption, int data) {
        byte[] data_Byte = BitConverter.GetBytes(data); //int -> Bytes
        byte[] Packet = Build_Packet(Crccode, PackageType, SessionID, Encryption, DataType.VectorData, data_Byte); //組合封包
        return Packet;
    }

    /// <summary>
    /// 構建 一般型封包 Long類型
    /// </summary>
    /// <param name="Crccode"> int 封包驗證碼 4 Bytes </param>
    /// <param name="PackageType"> enum 封包型態 2 Bytes </param>
    /// <param name="SessionID"> long 登陸後用戶識別碼 4 Bytes </param>
    /// <param name="Encryption">int 加密方式 4 Bytes </param>
    /// <param name="data">int資料</param>
    /// <returns>Package封包</returns>
    public static byte[] LongPackage(int Crccode, PackageType PackageType, long SessionID, int Encryption, long data) {
        byte[] data_Byte = BitConverter.GetBytes(data); //long -> Bytes
        byte[] Packet = Build_Packet(Crccode, PackageType, SessionID, Encryption, DataType.VectorData, data_Byte); //組合封包
        return Packet;
    }

    /// <summary>
    /// 構建 一般型封包 Boolean類型
    /// </summary>
    /// <param name="Crccode"> int 封包驗證碼 4 Bytes </param>
    /// <param name="PackageType"> enum 封包型態 2 Bytes </param>
    /// <param name="SessionID"> long 登陸後用戶識別碼 4 Bytes </param>
    /// <param name="Encryption">int 加密方式 4 Bytes </param>
    /// <param name="data">int資料</param>
    /// <returns>Package封包</returns>
    public static byte[] BooleanPackage(int Crccode, PackageType PackageType, long SessionID, int Encryption, bool data) {
        byte[] data_Byte = BitConverter.GetBytes(data); //Bool -> Bytes
        byte[] Packet = Build_Packet(Crccode, PackageType, SessionID, Encryption, DataType.VectorData, data_Byte); //組合封包
        return Packet;
    }

    /// <summary>
    /// 構建 一般型封包 DataBase string類型
    /// </summary>
    /// <param name="Crccode"> int 封包驗證碼 4 Bytes </param>
    /// <param name="dataBaseType"> enum 數據庫型態 2 Bytes </param>
    /// <param name="SessionID"> long 登陸後用戶識別碼 4 Bytes </param>
    /// <param name="Encryption">int 加密方式 4 Bytes </param>
    /// <param name="DataBody"> string[] 封包內容 長度視來源內容</param>
    /// <returns>Package封包</returns>
    public static byte[] DatabaseStringPackage(int Crccode, DataBaseType dataBaseType, long SessionID, int Encryption, string[] DataBody) {

        //計算MsgBody的長度
        int DataBodyLenght = 0;
        for (int i = 0; i < DataBody.Length; i++) {
            if (DataBody[i] == "")
                break;
            DataBodyLenght += Encoding.UTF8.GetBytes(DataBody[i]).Length;   //計算每個字串組中的長度總和 e.g: string[3] Data = {"Hello", "Welcome", "Hi"}; 分別為： 5 + 7 + 2 = DataBodyLenght;
        }

        //定義封包體的字節數組
        byte[] DataBody_Byte = new byte[DataBodyLenght + (DataBody.Length * 4)];    //保留每個字串組前4Bytes，以保存每組的字串長度。

        //紀錄存入消息體數組的字節數目前的索引位置
        int TempIndex = 0;
        for (int i = 0; i < DataBody.Length; i++) {
            //單個消息，單個字串組
            byte[] Temp_Bytes = Encoding.UTF8.GetBytes(DataBody[i]); //將第i個字串組取出
            BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Temp_Bytes.Length)).CopyTo(DataBody_Byte, TempIndex);    //計算第i個字組長度存放到TempIndex中
            TempIndex += 4; //右邊往左邊存， 1個int = 4bytes
            Temp_Bytes.CopyTo(DataBody_Byte, TempIndex);    //存入
            TempIndex += Temp_Bytes.Length; //加上字串組長度，索引至新位置；準備存第二組字串組。
        }

        //將所有資料打包成封包
        byte[] Packet = Build_DataBasePacket(Crccode, PackageType.DataBase, SessionID, Encryption, dataBaseType, DataBody_Byte); //組合封包

        return Packet;
    }
    #endregion

    #region 構建 直發型封包 [命名標準： Send_型態Package() ] (內部封包參數) {可共享}
    /// <summary>
    /// 構建 直發型封包 String類型
    /// </summary>
    /// <param name="player">玩家結構</param>
    /// <param name="packageType">封包型態</param>
    /// <param name="DataBody">內容</param>
    public static void Send_StringPackage(Player player, PackageType packageType, string[] DataBody) {

        //計算MsgBody的長度
        int DataBodyLenght = 0;
        for (int i = 0; i < DataBody.Length; i++) {
            if (DataBody[i] == "")
                break;
            DataBodyLenght += Encoding.UTF8.GetBytes(DataBody[i]).Length;   //計算每個字串組中的長度總和 e.g: string[3] Data = {"Hello", "Welcome", "Hi"}; 分別為： 5 + 7 + 2 = DataBodyLenght;
        }

        //定義封包體的字節數組
        byte[] DataBody_Byte = new byte[DataBodyLenght + (DataBody.Length * 4)];    //保留每個字串組前4Bytes，以保存每組的字串長度。

        //紀錄存入消息體數組的字節數目前的索引位置
        int TempIndex = 0;
        for (int i = 0; i < DataBody.Length; i++) {
            //單個消息，單個字串組
            byte[] Temp_Bytes = Encoding.UTF8.GetBytes(DataBody[i]); //將第i個字串組取出
            BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Temp_Bytes.Length)).CopyTo(DataBody_Byte, TempIndex);    //計算第i個字組長度存放到TempIndex中
            TempIndex += 4; //右邊往左邊存， 1個int = 4bytes
            Temp_Bytes.CopyTo(DataBody_Byte, TempIndex);    //存入
            TempIndex += Temp_Bytes.Length; //加上字串組長度，索引至新位置；準備存第二組字串組。
        }

        //將所有資料打包成封包
        byte[] Packet = Build_Packet(player.Crccode, packageType, player.SessionID, player.EncryptionMethod, DataType.StringData, DataBody_Byte); //組合封包

        //發送封包
        SendPackage(player.Socket, Packet);
    }

    /// <summary>
    /// 構建 直發型封包 Vector類型
    /// </summary>
    /// <param name="player">玩家結構</param>
    /// <param name="packageType">封包型態</param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void Send_VectorPackage(Player player, PackageType packageType, float x, float y, float z) {

        byte[] x_Byte = BitConverter.GetBytes(x);
        byte[] y_Byte = BitConverter.GetBytes(y);
        byte[] z_Byte = BitConverter.GetBytes(z);

        byte[] Vector_Byte = new byte[x_Byte.Length + y_Byte.Length + z_Byte.Length];   //12 Bytes

        x_Byte.CopyTo(Vector_Byte, 0);
        y_Byte.CopyTo(Vector_Byte, 4);
        z_Byte.CopyTo(Vector_Byte, 8);

        //將所有資料打包成封包
        byte[] Packet = Build_Packet(player.Crccode, packageType, player.SessionID, player.EncryptionMethod, DataType.VectorData, Vector_Byte); //組合封包

        //發送封包
        SendPackage(player.Socket, Packet);
    }

    /// <summary>
    /// 構建 直發型封包 Int類型
    /// </summary>
    /// <param name="player">玩家結構</param>
    /// <param name="packageType">封包型態</param>
    /// <param name="data">內容</param>
    public static void Send_IntPackage(Player player, PackageType packageType, int data) {
        byte[] data_Byte = BitConverter.GetBytes(data); //int -> Bytes
        byte[] Packet = Build_Packet(player.Crccode, packageType, player.SessionID, player.EncryptionMethod, DataType.IntData, data_Byte); //組合封包
        SendPackage(player.Socket, Packet); //發送
    }

    /// <summary>
    /// 構建 直發型封包 Long類型
    /// </summary>
    /// <param name="player">玩家結構</param>
    /// <param name="packageType">封包型態</param>
    /// <param name="data">內容</param>
    public static void Send_LongPackage(Player player, PackageType packageType, long data) {
        byte[] data_Byte = BitConverter.GetBytes(data); //long -> Bytes
        byte[] Packet = Build_Packet(player.Crccode, packageType, player.SessionID, player.EncryptionMethod, DataType.LongData, data_Byte); //組合封包
        SendPackage(player.Socket, Packet); //發送
    }

    /// <summary>
    /// 構建 直發型封包 Bool類型
    /// </summary>
    /// <param name="player">玩家結構</param>
    /// <param name="packageType">封包型態</param>
    /// <param name="data">內容</param>
    public static void Send_BooleanPackage(Player player, PackageType packageType, bool data) {
        byte[] data_Byte = BitConverter.GetBytes(data); //bool -> Bytes
        byte[] Packet = Build_Packet(player.Crccode, packageType, player.SessionID, player.EncryptionMethod, DataType.BoolData, data_Byte); //組合封包
        SendPackage(player.Socket, Packet); //發送
    }

    /// <summary>
    /// 構建 直發型封包 Bytes類型
    /// </summary>
    /// <param name="player">玩家結構</param>
    /// <param name="packageType">封包型態</param>
    /// <param name="data">內容</param>
    public static void Send_BytesPackage(Player player, PackageType packageType, DataType dataType, byte[] data) {
        byte[] Packet = Build_Packet(player.Crccode, packageType, player.SessionID, player.EncryptionMethod, dataType, data); //組合封包
        SendPackage(player.Socket, Packet); //發送
    }

    /// <summary>
    /// 構建 直發型封包 DataBase Bytes類型
    /// </summary>
    /// <param name="player">玩家結構</param>
    /// <param name="dataBaseType">資料庫行為選擇</param>
    /// <param name="data_Byte">數據</param>
    public static void Send_Database_BytePackage(Player player, DataBaseType dataBaseType, byte[] data_Byte) {
        byte[] Packet = Build_DataBasePacket(player.Crccode, PackageType.DataBase, player.SessionID, player.EncryptionMethod, dataBaseType, data_Byte); //組合封包
        SendPackage(player.Socket, Packet); //發送
    }

    /// <summary>
    /// 構建 直發型封包 DataBase String類型
    /// </summary>
    /// <param name="player"></param>
    /// <param name="dataBaseType"></param>
    /// <param name="DataBody"></param>
    public static void Send_Database_StringPackage(Player player, DataBaseType dataBaseType, string[] DataBody) {

        //計算MsgBody的長度
        int DataBodyLenght = 0;
        for (int i = 0; i < DataBody.Length; i++) {
            if (DataBody[i] == "")
                break;
            DataBodyLenght += Encoding.UTF8.GetBytes(DataBody[i]).Length;   //計算每個字串組中的長度總和 e.g: string[3] Data = {"Hello", "Welcome", "Hi"}; 分別為： 5 + 7 + 2 = DataBodyLenght;
        }

        //定義封包體的字節數組
        byte[] DataBody_Byte = new byte[DataBodyLenght + (DataBody.Length * 4)];    //保留每個字串組前4Bytes，以保存每組的字串長度。

        //紀錄存入消息體數組的字節數目前的索引位置
        int TempIndex = 0;
        for (int i = 0; i < DataBody.Length; i++) {
            //單個消息，單個字串組
            byte[] Temp_Bytes = Encoding.UTF8.GetBytes(DataBody[i]); //將第i個字串組取出
            BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Temp_Bytes.Length)).CopyTo(DataBody_Byte, TempIndex);    //計算第i個字組長度存放到TempIndex中
            TempIndex += 4; //右邊往左邊存， 1個int = 4bytes
            Temp_Bytes.CopyTo(DataBody_Byte, TempIndex);    //存入
            TempIndex += Temp_Bytes.Length; //加上字串組長度，索引至新位置；準備存第二組字串組。
        }

        //將所有資料打包成封包
        byte[] Packet = Build_DataBasePacket(player.Crccode, PackageType.DataBase, player.SessionID, player.EncryptionMethod, dataBaseType, DataBody_Byte); //組合封包

        //發送封包
        SendPackage(player.Socket, Packet);
    }
    #endregion

    #region 構建 轉發型封包 [命名標準： Transpond_型態Package() ] (內部封包參數) {可共享}
    /// <summary>
    /// 構建 轉發型封包 Vector類型
    /// </summary>
    /// <param name="PlayerSocket">轉發目標</param>
    /// <param name="Head">封包Head</param>
    /// <param name="Body">封包Body</param>
    public static void Transpond_VectorPackage(Socket PlayerSocket, byte[] Head, byte[] Body) {
        byte[] Packet = new byte[HeadLength + Body.Length]; //封包創建容器
        Head.CopyTo(Packet, 0);             //封裝Head
        Body.CopyTo(Packet, HeadLength);    //封裝Body
        SendPackage(PlayerSocket, Packet);  //發送
    }
    #endregion
    #endregion

    #region 組合封包 [命名標準： Build_型態Packet() ] (外部封包參數) {可共享}
    /// <summary>
    /// 組合 一般型封包
    /// </summary>
    /// <param name="Crccode"> int 封包驗證碼 4 Bytes </param>
    /// <param name="PackageType"> enum 封包型態 2 Bytes </param>
    /// <param name="SessionID"> long 登陸後用戶識別碼 4 Bytes </param>
    /// <param name="Encryption">int 加密方式 4 Bytes </param>
    /// <param name="dataType"> enum 內容型態 2 Bytes </param>
    /// <param name="DataBody_Byte"> 內容</param>
    /// <returns>Packet</returns>
    public static byte[] Build_Packet(int Crccode, PackageType PackageType, long SessionID, int Encryption, DataType dataType, byte[] DataBody_Byte) {

        //將封包頭部資訊轉為字節組（非字符類型需將主機序 轉為 網絡序），網絡序以Big-Ending儲存
        byte[] Crccode_Byte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Crccode));
        byte[] PackageType_Byte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)PackageType));
        byte[] SessionID_Byte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(SessionID));
        byte[] Encryption_Byte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Encryption));
        byte[] DataType_Byte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)dataType));

        //定義Package封裝整個封包（即：添加封包頭資訊 及 封包內容 到package中）
        byte[] Packet = new byte[HeadLength + DataBody_Byte.Length];    //PackageHead： 4(驗證碼)+4(整個封包長度)+2(封包型態)+8(用戶識別碼)+4(加密方式)+2(資料型態) = 24 Bytes; PackageBody: 以前文編碼決定長度

        /*組合Package（進行最後的封裝)*/
        /*Head*/
        Crccode_Byte.CopyTo(Packet, 0);    //封裝驗證碼
        BitConverter.GetBytes(IPAddress.HostToNetworkOrder(DataBody_Byte.Length)).CopyTo(Packet, 4);   //封裝Body長度
        PackageType_Byte.CopyTo(Packet, 8);         //封裝封包型態
        SessionID_Byte.CopyTo(Packet, 10);          //封裝登陸用戶識別碼
        Encryption_Byte.CopyTo(Packet, 18);         //封裝加密方式     
        DataType_Byte.CopyTo(Packet, 22);           //內容型態
                                                    /*Body*/
        DataBody_Byte.CopyTo(Packet, HeadLength);   //封裝內容

        return Packet;
    }

    /// <summary>
    /// 組合 數據庫封包
    /// </summary>
    /// <param name="Crccode"> int 封包驗證碼 4 Bytes </param>
    /// <param name="PackageType"> enum 封包型態 2 Bytes </param>
    /// <param name="SessionID"> long 登陸後用戶識別碼 4 Bytes </param>
    /// <param name="Encryption">int 加密方式 4 Bytes </param>
    /// <param name="dataBaseType"> enum 內容型態 2 Bytes </param>
    /// <param name="DataBody_Byte"> 內容</param>
    /// <returns>Packet</returns>
    public static byte[] Build_DataBasePacket(int Crccode, PackageType PackageType, long SessionID, int Encryption, DataBaseType dataBaseType, byte[] DataBody_Byte) {

        //將封包頭部資訊轉為字節組（非字符類型需將主機序 轉為 網絡序），網絡序以Big-Ending儲存
        byte[] Crccode_Byte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Crccode));
        byte[] PackageType_Byte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)PackageType));
        byte[] SessionID_Byte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(SessionID));
        byte[] Encryption_Byte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Encryption));
        byte[] DataBaseType_Byte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)dataBaseType));

        //定義Package封裝整個封包（即：添加封包頭資訊 及 封包內容 到package中）
        byte[] Packet = new byte[HeadLength + DataBody_Byte.Length];    //PackageHead： 4(驗證碼)+4(整個封包長度)+2(封包型態)+8(用戶識別碼)+4(加密方式)+2(資料型態) = 24 Bytes; PackageBody: 以前文編碼決定長度

        /*組合Package（進行最後的封裝)*/
        /*Head*/
        Crccode_Byte.CopyTo(Packet, 0);    //封裝驗證碼
        BitConverter.GetBytes(IPAddress.HostToNetworkOrder(DataBody_Byte.Length)).CopyTo(Packet, 4);   //封裝Body長度
        PackageType_Byte.CopyTo(Packet, 8);         //封裝封包型態
        SessionID_Byte.CopyTo(Packet, 10);          //封裝登陸用戶識別碼
        Encryption_Byte.CopyTo(Packet, 18);         //封裝加密方式       
        DataBaseType_Byte.CopyTo(Packet, 22);       //內容型態
                                                    /*Body*/
        DataBody_Byte.CopyTo(Packet, HeadLength);   //封裝內容
        return Packet;
    }
    #endregion

    #region 發送封包 [命名標準： Send_Packet() ] (無) {可共享}
    /// <summary>
    /// 發送封包
    /// </summary>
    /// <param name="Target"></param>
    /// <param name="Package"></param>
    public static void SendPackage(Socket Target, byte[] Package) {
        if (Target.Connected) {
            try {

                Target.Send(Package, Package.Length, 0);    //發送封包
            } catch (Exception ex) {
                Console.WriteLine($"封包發送失敗:{ex.Message}");
            }
        } else {
            throw new Exception($"{Target.RemoteEndPoint}:已斷線！");
        }
    }
    #endregion

    #region 解析封包 {部分可共享}
    #region 解析 Head [命名標準： UnpackHead_型態() ] (參數: Head) {可共享}
    /// <summary>
    /// 解析 驗證碼 0～4
    /// </summary>
    /// <param name="Head"></param>
    /// <returns></returns>
    public static int UnpackHead_Crccode(byte[] Head) {
        byte[] Temp_Bytes = new byte[4];        //建立容器
        Array.Copy(Head, 0, Temp_Bytes, 0, 4);  //讀取資料
        int Crccode = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(Temp_Bytes, 0));    // Bytes -> int
        return Crccode;
    }

    /// <summary>
    /// 解析 封包Body長度 4～8
    /// </summary>
    /// <param name="Head"></param>
    /// <returns></returns>
    public static int UnpackHead_BodyLenght(byte[] Head) {
        byte[] Temp_Bytes = new byte[4];        //建立容器
        Array.Copy(Head, 4, Temp_Bytes, 0, 4);  //讀取資料
        int BodyLenght = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(Temp_Bytes, 0));    // Bytes -> int
        return BodyLenght;
    }

    /// <summary>
    /// 解析 封包類型 8～10
    /// </summary>
    /// <param name="Head">封包頭</param>
    /// <returns>資料</returns>
    public static int UnpackHead_PackageType(byte[] Head) {
        byte[] Temp_Bytes = new byte[2];        //建立容器
        Array.Copy(Head, 8, Temp_Bytes, 0, 2);  //讀取資料
        int PackageType = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Temp_Bytes, 0));    // Bytes -> int
        return PackageType;
    }

    /// <summary>
    /// 解析 用戶身份ID 10~18
    /// </summary>
    /// <param name="Head"></param>
    /// <returns></returns>
    public static long UnpackHead_SessionID(byte[] Head) {
        byte[] Temp_Bytes = new byte[8];        //建立容器
        Array.Copy(Head, 10, Temp_Bytes, 0, 8); //讀取資料
        long SessionID = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(Temp_Bytes, 0));    // Bytes -> long
        return SessionID;
    }

    /// <summary>
    /// 解析 加密方式 18～22
    /// </summary>
    /// <param name="Head">封包頭</param>
    /// <returns></returns>
    public static int UnpackHead_Encryption(byte[] Head) {
        byte[] Temp_Bytes = new byte[4];        //建立容器
        Array.Copy(Head, 18, Temp_Bytes, 0, 4); //讀取資料
        int Encryption = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(Temp_Bytes, 0));    // Bytes -> int
        return Encryption;
    }

    /// <summary>
    /// 解析 資料類型 22~24   (可強制轉型成數據庫類型,前提是使用書庫類型封裝的封包)
    /// </summary>
    /// <param name="Head">封包頭</param>
    /// <returns>資料</returns>
    public static int UnpackHead_DataType(byte[] Head) {
        byte[] Temp_Bytes = new byte[2];        //建立容器
        Array.Copy(Head, 22, Temp_Bytes, 0, 2); //讀取資料
        int DataType = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Temp_Bytes, 0));    // Bytes -> int
        return DataType;
    }
    #endregion

    #region 解析 Body [命名標準： UnpackBody_型態() ] (參數: Body) {可共享}
    /// <summary>
    /// 解析 String型態 (### 未解決多筆資料回傳問題) {可共享}
    /// </summary>
    /// <param name="Body">封包內容</param>
    public static string UnpackBody_StringData(byte[] Body) {

        string strData = "N/A";
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

            strData = Encoding.UTF8.GetString(StringData, 0, StringData.Length);
        }

        return strData;
    }

    /// <summary>
    /// 解析 Vector/float型態 （### 未解決多筆資料回傳問題）{可共享}
    /// </summary>
    /// <param name="Body">封包內容</param>
    public static float UnpackBody_VectorData(byte[] Body) {

        byte[] x_Bytes = new byte[4];
        byte[] y_Bytes = new byte[4];
        byte[] z_Bytes = new byte[4];

        Array.Copy(Body, 0, x_Bytes, 0, 4);
        Array.Copy(Body, 4, y_Bytes, 0, 4);
        Array.Copy(Body, 8, z_Bytes, 0, 4);

        float floatData_x = BitConverter.ToSingle(x_Bytes, 0);
        float floatData_y = BitConverter.ToSingle(y_Bytes, 0);
        float floatData_z = BitConverter.ToSingle(z_Bytes, 0);

        return floatData_x;
    }

    /// <summary>
    /// 解析 Int型態 {可共享}
    /// </summary>
    /// <param name="ClientSocket">用戶Socket</param>
    /// <param name="Body">封包內容</param>
    public static int UnpackBody_IntData(Socket ClientSocket, byte[] Body) {
        int intData = BitConverter.ToInt32(Body, 0);
#if SHOW
    ShowUnpackData(DataType.IntData, ref intData);
#endif
        return intData;
    }

    /// <summary>
    /// 解析 Long型態 {可共享}
    /// </summary>
    /// <param name="ClientSocket">用戶Socket</param>
    /// <param name="Body">封包內容</param>
    public static long UnpackBody_LongData(Socket ClientSocket, byte[] Body) {
        long longData = BitConverter.ToInt64(Body, 0);
#if SHOW
    ShowUnpackData(DataType.LongData, ref longData);
#endif
        return longData;
    }

    /// <summary>
    /// 解析 Boolean型態 {可共享}
    /// </summary>
    /// <param name="Body">封包內容</param>
    public static bool UnpackBody_BooleanData(byte[] Body) {
        bool booleanData = BitConverter.ToBoolean(Body, 0);
#if SHOW
    ShowUnpackData(DataType.BooleanData, ref booleanData);
#endif
        return booleanData;
    }
    #endregion

    #region 解析 整個封包 [命名標準： 無 ] (無) {不可共享}
    /// <summary>
    /// 解析 整個封包
    /// </summary>
    /// <param name="ClientSocket">用戶Socket</param>
    /// <param name="Head">封包Head</param>
    /// <param name="Body">封包Body</param>
    public static void UnpackData(Socket ClientSocket, byte[] Head, byte[] Body) {

        #region 解析 Head
        int Crccode = UnpackHead_Crccode(Head);                                 //封包驗證碼 0～4
        int BodyLenght = UnpackHead_BodyLenght(Head);                           //封包整體長度 4～8      
        PackageType packageType = (PackageType)UnpackHead_PackageType(Head);    //封包型態 8～10   
        long SessionID = UnpackHead_SessionID(Head);                            //用戶身份ID 10～18    
        int Encryption = UnpackHead_Encryption(Head);                           //加密方式 18～22
        DataType DataType = (DataType)UnpackHead_DataType(Head);                //資料型態 22～24
        #endregion

#if SHOW
    Console.WriteLine("-------------------------------------------------------");
    Console.WriteLine($"收到來自{ClientSocket.RemoteEndPoint}的封包");
    Console.WriteLine($"驗證碼:{Crccode}\t封包長度{BodyLenght}\t封包類型:{packageType}\t用戶ID:{SessionID}\t加密方式:{Encryption}\t資料類型{DataType}");
    Console.WriteLine("封包內容:\r\n");
#endif

        #region 解析 Body（只會輸出）
        //根據對應內容型態進行解析
        switch (DataType) {
            case DataType.None:
                break;

            case DataType.StringData:
                UnpackBody_StringData(Body);
                break;

            case DataType.VectorData:
                UnpackBody_VectorData(Body);
                break;

            case DataType.IntData:
                UnpackBody_IntData(ClientSocket, Body);
                break;

            case DataType.LongData:
                UnpackBody_LongData(ClientSocket, Body);
                break;

            case DataType.BoolData:
                UnpackBody_BooleanData(Body);
                break;

            default:
                throw new Exception("Error, 'Network.cs' UpackData(), switch(dataTypeIndex) can not found!");
        }
        #endregion
    }
    #endregion

    #region 顯示 封包資訊 [命名標準： 無 ] (參數: Body) {不可共享}
    private static void ShowUnpackData<T>(DataType dataType, ref T Data) {

        switch (dataType) {
            case DataType.StringData:
                Console.Write($"{Data} ");
                break;
            case DataType.VectorData:
                Console.Write($"{Data}    ");
                break;
            case DataType.IntData:
                Console.WriteLine($"{Data}");
#if SHOW
            Console.WriteLine("-------------------------------------------------------");
#endif
                break;
            case DataType.LongData:
                Console.WriteLine($"{Data}");
#if SHOW
            Console.WriteLine("-------------------------------------------------------");
#endif
                break;
            case DataType.BoolData:
                Console.WriteLine($"{Data}");
#if SHOW
            Console.WriteLine("-------------------------------------------------------");
#endif
                break;
        }
    }
    #endregion
    #endregion



}

