using System;

//額外封裝(Mysql)
using MySql.Data.MySqlClient;
using MySql.Data;

//自定義 命名空間
using Setup;

public class DataBase {

    //屬性
    public static MySqlConnection ConnDB { get; private set; }

    //Instance
    public DataBase() => ConnDB = new MySqlConnection(RefConnection.DBConnStr);
    
    #region 方法
    public void Start() {
        try {
            ConnDB.Open();  //開啟數據庫
            Console.WriteLine($"數據庫已連接\t Info [Gate {RefConnection.dbHost}:{RefConnection.dbPort} | DataBase Name: {RefConnection.dbName} ]");
            Console.WriteLine("-------------------------------------------------------------");
            Console.WriteLine("一切準備就緒...\n\n");
        } catch (MySqlException ex) {
            switch (ex.Number) {
                case 0:
                    Console.WriteLine("@ Warning: 無法連線到資料庫,找不到資料庫.");
                    break;
                case 1045:
                    Console.WriteLine("@ Warning: 使用者帳號或密碼錯誤,請再試一次.");
                    break;
                default:
                    Console.WriteLine("@ Warning: 未開啟目標數據庫.");
                    break;
            }
        }
    }

    public void Close() => ConnDB.Close();
    #endregion
}
