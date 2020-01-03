using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System.Collections;

using CustomEnumType;
using Network.Packet;

public class StartOnline : MonoBehaviour {

    public static volatile bool isQuit;       //volatile定義: 全域時刻保持最新數據; isQuit判斷遊戲是否退出，並對線程進行調整
    public static  Client MyClient;

    /// <summary>
    /// 初始化
    /// </summary>
    private void Initial() {

        //設定服務器初始值
        isQuit = false;

        //建立 & 連接服務器
        MyClient = new Client();    //創建一個client物件    
    }

    /// <summary>
    /// 網絡連接
    /// </summary>
    private void Awake() {

        Initial();  //初始化資料

        //開啟客戶端
        MyClient.Connect();




        ////載入遊戲紀錄
        if (MyClient.Socket.Connected) {
            Debug.Log("正在載入資料...");
            //載入資料
            //StartCoroutine(LoadGame()); 
        } else {
            Debug.Log("Client 未連接");
        } 
    }

    /// <summary>
    /// 地圖加載
    /// </summary>
    /// <returns></returns>
    //private IEnumerator LoadGame() {
    //    bool isLoadCompleted = false;

    //    int TimeOut = 0;

    //    //需設定超時func,否則會死循環
    //    while (TimeOut < 3000) {
    //        if (GameDataList.MapData != null) {
    //            isLoadCompleted = true;
    //            Debug.Log("獲取 MapData");
    //            break;
    //        }

    //        Thread.Sleep(100);

    //        TimeOut += 100;
            

    //    }

    //    if(TimeOut >= 3000) Debug.Log("地圖加載 超時");  
    //    else Debug.Log("地圖加載完畢");

    //    yield return isLoadCompleted;
    //}

    

}
