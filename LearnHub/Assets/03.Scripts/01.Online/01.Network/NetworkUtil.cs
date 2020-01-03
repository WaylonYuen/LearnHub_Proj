using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// NetworkUtils.cs : 提供網絡工具函數
/// 
///             函數                              說明
///     -------------------------------------------------------
///     1、GetlocalIPv4()                    獲取本地IPv4主機地址
///     2、Serialize(object obj)             序列化
///     3、T Deserialize<T>(byte[] data)     反序列化
///     
/// </summary>

//重載（重寫反序列化功能: 加載對方的類別庫到自己的類別庫中引用）
public class UBinder : SerializationBinder {
    public override Type BindToType(string assemblyName, string typeName) {
        Assembly ass = Assembly.GetExecutingAssembly();
        return ass.GetType(typeName);
    }
}

public static class NetworkUtil {

    /// <summary>
    /// GetIPv4() : 獲取IPv4訊息
    /// </summary>
    /// <returns>IPv4訊息</returns>
    public static string GetLocalIPv4() {
        string hostName = Dns.GetHostName();    //獲取主機名稱
        IPHostEntry ipEntry = Dns.GetHostEntry(hostName);
        for (int i = 0; i < ipEntry.AddressList.Length; i++) {
            //從IP地址列表中篩選出IPv4類型的IP地址
            if (ipEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                return ipEntry.AddressList[i].ToString();
        }
        return null;
    }

    /// <summary>
    /// Serialize() : 序列化
    /// </summary>
    /// <param name="obj">序列化物件</param>
    /// <returns>序列化後的資料</returns>
    public static byte[] Serialize(object obj) {

        //物件不為空 且可被序列化
        if (obj == null || !obj.GetType().IsSerializable)
            return null;

        BinaryFormatter formatter = new BinaryFormatter();  //創建物件

        using (MemoryStream stream = new MemoryStream()) {
            formatter.Serialize(stream, obj);
            byte[] data = stream.ToArray();
            return data;
        }
    }

    /// <summary>
    /// Deserialize() : 反序列化
    /// </summary>
    /// <typeparam name="T">序列化</typeparam>
    /// <param name="data">序列化資料</param>
    /// <returns>反序列化後的資料</returns>
    public static T Deserialize<T>(byte[] data) where T : class {

        //數據不為空 且T是可序列化的類型
        if (data == null || !typeof(T).IsSerializable)
            return null;

        IFormatter formatter = new BinaryFormatter();
        formatter.Binder = new UBinder();
        using (MemoryStream stream = new MemoryStream(data)) {
            object obj = formatter.Deserialize(stream);
            return obj as T;
        }
    }



}

namespace CustomEnumType {

    /// <summary>
    /// 封包型態定義，決定此封包的目的（e.g: 實時同步、 Msg、 Login訊息）
    /// </summary>
    public enum PackageType {
        None,           //無         必須定義DataType
        Test,

        /*系統封包*/
        HeartBeat,      //心跳包       統一空包 0
        Connection,     //連線封包      Bool

        /*玩家請求*/
        DataBase,       //數據庫       DatabaseType

        Login,          //登入封包      String

        GetData,
        GetCourseData,  //獲取課程資料
        SearchCourse
    }

    /// <summary>
    /// 封包內容型態定義，決定以什麼型態封裝、序列化（e.g: int、 float、 double、 string）//請忽修改次序, 會打亂switch的次序
    /// </summary>
    public enum DataType {  //請忽修改次序, 會打亂switch的次序
        None,           //無            必須定義PackageType
        Test,

        StringData,     //字串型態內容
        IntData,        //整數型態內容    Int32   4bytes
        LongData,       //長整型態內容    Int64   8bytess
        FloatData,      //浮點型態內容
        BoolData,       //布爾型態內容
        ByteData,       //字節型態內容
        VectorData,     //座標型態內容    ？？？

    }

    /// <summary>
    /// 資料庫行為定義（需和Client同步資料）
    /// </summary>
    public enum DataBaseType {

        /*一般數據*/
        None,           //無
        Test,

        Check,          //查詢資料（對比資料）    bool（查到:True）
        Insert,         //插入資料（追加資料）    bool（成功:True）   保存指定資料（保存單項資料,屬於覆蓋類型）
        Update,         //更新資料（覆蓋資料）    bool（成功:True）
        Delete,         //刪除資料              bool（成功:True）

        /*獲取 指定數據*/
        Login,         //登入    封包內容String

        GetData,
        getAccountData,
        GetCourseData,   //獲取課程資料
        SearchCourse,
        SelectCourse,
        ExitCourse
    }

    /// <summary>
    /// 加密方式選擇
    /// </summary>
    public enum EncryptionType {
        None,
        Test,

        RES256,
    }
}