#define isShow_PacketData

using System;
using System.Text;
using UnityEngine;

//自定義
using CustomEnumType;
using Network.Packet;
using Data.DataBuffer;
using LearnHub.Data;

public class CallBackMethods {

    /// <summary>
    /// 註冊不同類型的封包以及其類型的解讀、執行方法
    /// </summary>
    public void RegisterCallBackMethods() {
        Client.Register(PackageType.HeartBeat, HeartBeat);                      //註冊心跳包方法
        Client.Register(PackageType.Login, Login);                              //註冊登陸結果方法
        Client.Register(PackageType.GetData, GetData);                          //註冊遊戲資料方法
        Client.Register(PackageType.Connection, Connection);                    //註冊連線回覆方法（Server -> Client）
        Client.Register(PackageType.GetCourseData, GetCourseData);
        Client.Register(PackageType.SearchCourse, SearchCourse);
    }

    /// <summary>
    /// 心跳包方法，判斷玩家是否還在線
    /// </summary>
    /// <param name="player"></param>
    /// <param name="Head"></param>
    /// <param name="Body"></param>
    private void HeartBeat(Player player, byte[] Head, byte[] Body) {
        Send Send = new Send();
        Send.BoolPacket(player, PackageType.HeartBeat, true);   //回覆心跳包
    }

    /// <summary>
    /// 確認登入
    /// </summary>
    /// <param name="player"></param>
    /// <param name="Head"></param>
    /// <param name="Body"></param>
    private void Login(Player player, byte[] Head, byte[] Body) {

        Unpack Unpack = new Unpack();

        LearnHub.Functions.Login.Login.IsPass = Unpack.Body_BoolData(Body);
        LearnHub.Functions.Login.Login.Inspect(player);
    }

    //Delete : （Server -> Client）暫時沒有方法
    private void Connection(Player player, byte[] Head, byte[] Body) {

    }

    /// <summary>
    /// 獲取遊戲資料包
    /// </summary>
    /// <param name="player"></param>
    /// <param name="Head"></param>
    /// <param name="Body"></param>
    private void GetData(Player player, byte[] Head, byte[] Body) {

        Unpack Unpack = new Unpack();
        DataBaseType dataBaseType = Unpack.Head_DataBaseType(Head);       //獲取資料類型

        switch (dataBaseType) {

            case DataBaseType.GetCourseData:

                //DataList.CourseData = NetworkUtil.Deserialize<CourseData>(Body);  //反序列化 課程資料
                break;

            default:
                Debug.Log("Warning: No define DatabaseType!");
                break;
        }

        Debug.Log($"得到資料: {dataBaseType}");
    }


    private void GetCourseData(Player player, byte[] Head, byte[] Body) {
        DataList.CourseData = NetworkUtil.Deserialize<Data.DataBuffer.CourseData>(Body);  //反序列化 課程資料
        DataList.CourseDataTemp[Functions.MyCourseFuncBtn.MyCourseIndexTemp] = DataList.CourseData;
        //Debug.Log($"{DataList.CourseData.ElectiveID} {DataList.CourseData.CourseName}");
        Functions.MyCourseList.MyCourseIndex++;
        Functions.MyCourseFuncBtn.MyCourseIndexTemp++;
    }

    //搜尋結果
    private void SearchCourse(Player player, byte[] Head, byte[] Body) {
        DataList.SearchCourse = NetworkUtil.Deserialize<Data.DataBuffer.CourseData>(Body);  //反序列化 課程資料
        DataList.SearchCourseTemp[SearchBtn.SearchIndexTemp] = DataList.SearchCourse;    //保存每一筆資料

        Functions.SearchCourse.GetSearchIndex++;
        SearchBtn.SearchIndexTemp++;
    }
}
