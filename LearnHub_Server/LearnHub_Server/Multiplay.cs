using System;

namespace Multiplay {

    public enum MessageType {
        None,           //空類型
        HeartBeat,      //心跳包驗證
        CreatRoom,      //創建房間
        EnterRoom,      //進入房間
        ExitRoom,       //退出房間
        StartGame,      //開始遊戲

    }

}
