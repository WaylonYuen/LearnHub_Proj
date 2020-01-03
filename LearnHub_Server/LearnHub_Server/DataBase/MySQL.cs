using System;
using MySql.Data.MySqlClient;

using Game.DataBuff;
using Game.Setup;
using Client;
using Data.DataBuffer;

namespace MySqlCmds {

    /// <summary>
    /// 封裝Sql命令
    /// </summary>
    public class Cmd {

        public MySqlCommand Login = new MySqlCommand(Sql.Login, DataBase.ConnDB);                               //登入ID查詢
        public MySqlCommand LoginTime = new MySqlCommand(Sql.LoginTime, DataBase.ConnDB);                       //更新登入時間

        public MySqlCommand NewPlayer = new MySqlCommand(Sql.NewAccountTable, DataBase.ConnDB);                //建立新玩家
        public MySqlCommand NewGameData = new MySqlCommand(Sql.NewGameDataTable, DataBase.ConnDB);             //建立新遊戲資料表
        public MySqlCommand CreatePlayerTable = new MySqlCommand(Sql.NewPlayerTable, DataBase.ConnDB);       //建立玩家資料表

        //取得
        public MySqlCommand GetMapData = new MySqlCommand(Sql.Calculus("MapData", "GameData", "Account_ID", "@Account_ID"), DataBase.ConnDB); //取得地圖資料
    }

    /// <summary>
    /// Sql指令語句
    /// </summary>
    public static class Sql {

        //特殊命令
        public static string Login = "select * from Account where ID = @para1 and Password = @para2";
        public static string LoginTime = "Update Account Set Online_Date = Now() Where ID = @para1";

        //創建
        public static string NewAccountTable = "Insert into Account(Password, Creation_Date, Online_Date) Values(@para2, Now(), Now())";
        public static string NewPlayerTable = "Insert into Player(Name, Level, BaseHP, Account_ID) Values(@Name, @Level, @BaseHP, @Account_ID)";
        public static string NewGameDataTable = "Insert into GameData(MapData, Account_ID) Values(@ByteData, @para2)";

        //取得
        //public static string GetMapData = "select octet_length(MapData) datasize from GameData where Account_ID = @Account_ID"; //計算MapData資料長度
        public static string sqGetMapData = "select MapData from GameData where Account_ID = @Account_ID"; //取得MaData資料

        //添加資料
        public static string Insert(string Table, string Data) {
            return "Insert into " + Table + " Values(" + Data + ")";
        }

        //計算資料長度
        public static string Calculus(string Data, string From, string Where, string Target) {
            return "select octet_length(" + Data + ") datasize from " + From + " where " + Where + " = " + Target + "";
        }
    }

    public static class MySqlCmd {

        #region 基本命令
        /// <summary>
        /// 查詢命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static bool Query(MySqlCommand cmd) {
            bool isExist = true;
            using (MySqlDataReader MySqlReader = cmd.ExecuteReader())
            return isExist &= MySqlReader.Read();
        }

        /// <summary>
        /// 更新命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static int Update(MySqlCommand cmd) { return cmd.ExecuteNonQuery(); }

        /// <summary>
        /// 添加命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static int Insert(MySqlCommand cmd) { return cmd.ExecuteNonQuery(); }

        /// <summary>
        /// 純量命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static object Calculus(MySqlCommand cmd) { return cmd.ExecuteScalar(); }
        #endregion

        #region 特殊命令
        /// <summary>
        /// 登入查詢命令
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static bool Login(long ID, string Password) {
            Cmd cmd = new Cmd();
            cmd.Login.Parameters.AddWithValue("para1", ID);          //參數賦值
            cmd.Login.Parameters.AddWithValue("para2", Password);    //參數賦值
            return Query(cmd.Login);
        }

        /// <summary>
        /// 更新用戶登陸時間
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static int UpdateLoginTime(long ID) {
            Cmd cmd = new Cmd();
            cmd.LoginTime.Parameters.AddWithValue("para1", ID);
            return Update(cmd.LoginTime);
        }
        #endregion
    }

    /// <summary>
    /// 建立玩家資料表
    /// </summary>
    public static class Create {

        /// <summary>
        /// 創建新玩家帳戶並保存
        /// </summary>
        /// <param name="NewPW"></param>
        /// <returns>成功: ID; 失敗: Result;</returns>
        public static long Account(string NewPW) {
            Cmd cmd = new Cmd();
            cmd.NewPlayer.Parameters.AddWithValue("para2", NewPW);
            int Result = MySqlCmd.Insert(cmd.NewPlayer);
            if (Result == 1) {
                long ID = cmd.NewPlayer.LastInsertedId; //取得最後一筆自增值
                return ID;
            }
            return Result;
        }

        /// <summary>
        /// 創建新玩家Map資料並保存
        /// </summary>
        /// <param name="PlayerData_ID"></param>
        /// <returns></returns>
        public static int MapData(int PlayerData_ID) {

            Cmd cmd = new Cmd();

            #region 建立遊戲檔案
            MapData mapData = new MapData();                                    //建立地圖
            Initial.MapData(mapData);                                           //初始化地圖
            byte[] MapData_Byte = NetworkUtil.Serialize(mapData);               //序列化地圖
            //### 優化：保存地圖長度 + 資料
            #endregion

            cmd.NewGameData.Parameters.Add("ByteData", MySqlDbType.Blob);       //定義sql命令中參數sql型態
            cmd.NewGameData.Parameters["ByteData"].Value = MapData_Byte;        //保存定義的資料
            cmd.NewGameData.Parameters.AddWithValue("@para2", PlayerData_ID);
            return MySqlCmd.Insert(cmd.NewGameData);
        }

        /// <summary>
        /// 創建新玩家資料
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static int Player(int ID) {
            Cmd cmd = new Cmd();
            cmd.CreatePlayerTable.Parameters.AddWithValue("Name", "Defualt");
            cmd.CreatePlayerTable.Parameters.AddWithValue("Level", 1);
            cmd.CreatePlayerTable.Parameters.AddWithValue("BaseHP", 2000f);
            cmd.CreatePlayerTable.Parameters.AddWithValue("Account_ID", ID);
            return MySqlCmd.Insert(cmd.CreatePlayerTable);
        }

    }

    /// <summary>
    /// 取得玩家資料
    /// </summary>
    public class Get {

        /// <summary>
        /// 獲取課程資料
        /// </summary>
        /// <param name="player"></param>
        public void CourseData(Player player) {

            string[] CourseID = { "N/A" };
            int CourseIndex = 0;

            Network.Packet.Send send = new Network.Packet.Send();
            CourseData courseData = new CourseData();

            //Checking
            for (int i = 1; i <= 14; i++) {

                string Checking = "SELECT CourseID_" + i.ToString() + " FROM LearnHubDB.MyCourse where UID = " + player.AccountID.ToString() + ";";
                MySqlCommand cmdChecking = new MySqlCommand(Checking, DataBase.ConnDB);
                MySqlDataReader ReaderCheck = cmdChecking.ExecuteReader();

                try {
                    ReaderCheck.Read();
                    string GetCourseID = ReaderCheck.GetString(0);
                    ReaderCheck.Close();

                    if (GetCourseID != "N/A") {
                        CourseID[CourseIndex] = GetCourseID;
                        CourseIndex++;
                    }

                } catch (Exception ex) {
                    ReaderCheck.Close();
                    Console.WriteLine($"找不到MyCourse資料：{ex.Message}");
                }

            }


            for (int i = 0; i < CourseIndex; i++) {
                Console.WriteLine($"GetcourseID : {CourseID[i]}");
                string GetCourseData =
                "SELECT ElectiveID, CourseID, CourseName, CourseScore, Elective, CourseClass, " +
                    "DateOfWeek1, Section1, location1, " +
                    "DateOfWeek2, Section2, location2, " +
                    "DateOfWeek3, Section3, location3, " +
                "Teacher, received, Quota," +
                "Degree, College, Department FROM LearnHubDB.Course WHERE CourseID = '" + CourseID[i] + "';";

                MySqlCommand cmdGetCourseData = new MySqlCommand(GetCourseData, DataBase.ConnDB);
                MySqlDataReader Reader = cmdGetCourseData.ExecuteReader();

                try { 
                    while (Reader.Read()) {
                        courseData.ElectiveID = Reader.GetInt16(0);
                        courseData.CourseID = Reader.GetString(1);
                        courseData.CourseName = Reader.GetString(2);
                        courseData.CourseScore = Reader.GetInt16(3);
                        courseData.Elective = Reader.GetString(4);
                        courseData.CourseClass = Reader.GetString(5);

                        courseData.DateOfWeek1 = Reader.GetString(6);
                        courseData.Section1 = Reader.GetString(7);
                        courseData.Location1 = Reader.GetString(8);

                        courseData.DateOfWeek2 = Reader.GetString(9);
                        courseData.Section2 = Reader.GetString(10);
                        courseData.Location2 = Reader.GetString(11);

                        courseData.DateOfWeek3 = Reader.GetString(12);
                        courseData.Section3 = Reader.GetString(13);
                        courseData.Location3 = Reader.GetString(14);

                        courseData.Teacher = Reader.GetString(15);
                        courseData.Received = Reader.GetInt16(16);
                        courseData.Quota = Reader.GetInt16(17);

                        courseData.Degree = Reader.GetString(18);
                        courseData.College = Reader.GetString(19);
                        courseData.Department = Reader.GetString(20);

                        byte[] courseData_Byte = NetworkUtil.Serialize(courseData); //序列化資料
                        send.BytePacket(player, CustomEnumType.PackageType.GetCourseData, courseData_Byte); //發送資料
                        Console.WriteLine($"GetMyCourseID: {courseData.CourseID}");
                    }
                    Reader.Close();
                    Console.WriteLine($"CourseData: 資料發送完畢");
                } catch (Exception ex) {
                    Reader.Close();
                    Console.WriteLine($"找不到資料：{ex.Message}");
                }
                Reader.Close();
            }


        }

        /// <summary>
        /// 獲取搜尋的課程結果
        /// </summary>
        /// <param name="player"></param>
        public void SearchCourse(Player player) {

            Network.Packet.Send send = new Network.Packet.Send();
            CourseData courseData = new CourseData();

            string search = "SELECT ElectiveID, CourseID, CourseName, CourseScore, Elective, CourseClass, " +
                    "DateOfWeek1, Section1, location1, " +
                    "DateOfWeek2, Section2, location2, " +
                    "DateOfWeek3, Section3, location3, " +
                    "Teacher, received, Quota," +
                    "Degree, College, Department " +
                    "FROM LearnHubDB.Course where Degree = '" + DataList.searchData.degree +
                    "' and College = '" + DataList.searchData.college +
                    "' and Department = '" + DataList.searchData.department +
                    "' and CourseClass = '"+ DataList.searchData.classs +
                    "';";


            MySqlCommand cmdGetCourseData = new MySqlCommand(search, DataBase.ConnDB);
            MySqlDataReader Reader = cmdGetCourseData.ExecuteReader();

            try {
                while (Reader.Read()) {
                    courseData.ElectiveID = Reader.GetInt16(0);
                    courseData.CourseID = Reader.GetString(1);
                    courseData.CourseName = Reader.GetString(2);
                    courseData.CourseScore = Reader.GetInt16(3);
                    courseData.Elective = Reader.GetString(4);
                    courseData.CourseClass = Reader.GetString(5);

                    courseData.DateOfWeek1 = Reader.GetString(6);
                    courseData.Section1 = Reader.GetString(7);
                    courseData.Location1 = Reader.GetString(8);

                    courseData.DateOfWeek2 = Reader.GetString(9);
                    courseData.Section2 = Reader.GetString(10);
                    courseData.Location2 = Reader.GetString(11);

                    courseData.DateOfWeek3 = Reader.GetString(12);
                    courseData.Section3 = Reader.GetString(13);
                    courseData.Location3 = Reader.GetString(14);

                    courseData.Teacher = Reader.GetString(15);
                    courseData.Received = Reader.GetInt16(16);
                    courseData.Quota = Reader.GetInt16(17);

                    courseData.Degree = Reader.GetString(18);
                    courseData.College = Reader.GetString(19);
                    courseData.Department = Reader.GetString(20);

                    byte[] courseData_Byte = NetworkUtil.Serialize(courseData); //序列化資料
                    send.BytePacket(player, CustomEnumType.PackageType.SearchCourse, courseData_Byte); //發送資料
                    Console.WriteLine($"Search Test {courseData.ElectiveID}");
                }
                Reader.Close();
                Console.WriteLine($"CourseData: 資料發送完畢");
            } catch (Exception ex) {
                Reader.Close();
                Console.WriteLine($"找不到資料：{ex.Message}");
            }


        }


        /// <summary>
        /// 獲取玩家資料
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public byte[] PlayerData(int ID) {

            PlayerData playerData = new PlayerData();   //Game.DataBuff

            string GetPlayerData = "select Name, Level, BaseHP from Player where Account_ID =" + ID;
            MySqlCommand cmdGetPlayerData = new MySqlCommand(GetPlayerData, DataBase.ConnDB);
            MySqlDataReader Reader = cmdGetPlayerData.ExecuteReader();

            try {
                Reader.Read();
                playerData.Name = Reader.GetString(0);
                playerData.Level = Reader.GetInt16(1);
                playerData.MaxHP = Reader.GetFloat(2);
                Reader.Close();

                byte[] player_Byte = NetworkUtil.Serialize(playerData); //序列化資料    
                return player_Byte;

            } catch (Exception ex) {

                Reader.Close();
                Console.WriteLine($"找不到資料：{ex.Message}");
                return new byte[0];
            }
        }

        /// <summary>
        /// 獲取玩家地圖資料
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public byte[] MapData(int ID) {

            Cmd cmd = new Cmd();

            //計算MapData長度
            cmd.GetMapData.Parameters.AddWithValue("Account_ID", ID);           //利用ID獲取地圖包
            object DataLen = MySqlCmd.Calculus(cmd.GetMapData);

            int Lenght = -1;
            if (DataLen != null) Lenght = int.Parse(DataLen.ToString());                         //強制轉型
            else Console.WriteLine($"@ Warning:玩家未註冊 ID {ID}    Lenght {Lenght}"); //測試
            
            //取得MapData
            byte[] getData = new byte[Lenght];  //創建容器
            string sqlGetMapData = "select MapData from GameData where Account_ID =" + ID;
            MySqlCommand cmdGetMapData = new MySqlCommand(sqlGetMapData, DataBase.ConnDB); //sql命令
            MySqlDataReader Reader = cmdGetMapData.ExecuteReader();   //執行

            //讀取byte
            while (Reader.Read()) { getData = (byte[])Reader.GetValue(0); }
            Reader.Close();

            return getData;
        }
    }

    public class Set {

        public void SelectCourse(Player player, string CourseID) {

            int nullIndex = -1;

            //Checking
            for (int i = 14; i >= 1; i--) {
                string Checking = "SELECT CourseID_" + i.ToString() + " FROM LearnHubDB.MyCourse where UID = " + player.AccountID.ToString() + ";";
                MySqlCommand cmdChecking = new MySqlCommand(Checking, DataBase.ConnDB);
                MySqlDataReader Reader = cmdChecking.ExecuteReader();
                try {

                    Reader.Read();
                    string GetCourseID = Reader.GetString(0);
                    if (GetCourseID.Equals(CourseID)) {
                        //@發送封包：不可重複加選
                        Reader.Close();
                        return;
                    }

                    if(GetCourseID == "N/A") {
                        nullIndex = i;
                    }

                    Reader.Close();
                } catch (Exception ex) {
                    Reader.Close();
                    Console.WriteLine($"找不到資料：{ex.Message}");
                }
            }

            //加選
            string setMyCourse = "Update LearnHubDB.MyCourse Set CourseID_" + nullIndex.ToString() + " = '" + CourseID + "' Where UID = " + player.AccountID.ToString() + ";";
            MySqlCommand cmdUpdateCourse = new MySqlCommand(setMyCourse, DataBase.ConnDB);

            try {
                cmdUpdateCourse.ExecuteNonQuery();
                Console.WriteLine($"課表更新成功 ID :{player.AccountID}");
            } catch (Exception ex) {
                Console.WriteLine($"更新失敗：{ex.Message}");
                //@發送封包：更新失敗
            }
        }

        public void ExitCourse(Player player, string CourseID) {

            //Checking
            for (int i = 1; i <=14; i++) {
                string Checking = "SELECT CourseID_" + i.ToString() + " FROM LearnHubDB.MyCourse where UID = " + player.AccountID.ToString() + ";";
                MySqlCommand cmdChecking = new MySqlCommand(Checking, DataBase.ConnDB);
                MySqlDataReader Reader = cmdChecking.ExecuteReader();
                try {

                    Reader.Read();
                    string GetCourseID = Reader.GetString(0);
                    if (GetCourseID.Equals(CourseID)) {
                        Reader.Close();
                        //@發送封包：不可重複加選
                        string setMyCourse = "Update LearnHubDB.MyCourse Set CourseID_" + i.ToString() + " = 'N/A' Where UID = " + player.AccountID.ToString() + ";";
                        MySqlCommand cmdUpdateCourse = new MySqlCommand(setMyCourse, DataBase.ConnDB);

                        try {
                            cmdUpdateCourse.ExecuteNonQuery();
                            Console.WriteLine($"課表更新成功 ID :{player.AccountID}");
                        } catch (Exception ex) {
                            Console.WriteLine($"更新失敗：{ex.Message}");
                            //@發送封包：更新失敗
                        }
                        return;
                    }

                    Reader.Close();
                } catch (Exception ex) {
                    Reader.Close();
                    Console.WriteLine($"找不到資料：{ex.Message}");
                }
            }

        }

    }

}