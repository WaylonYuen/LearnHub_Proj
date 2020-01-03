using System;

//遊戲資料
namespace Game.DataBuff {



    /// <summary>
    /// 玩家資料
    /// </summary>
    [Serializable]
    public class PlayerData {
        public string Name;     //玩家名稱
        public int Level;       //玩家等級
        public float MaxHP;     //最大血量
    }

    /// <summary>
    /// 地圖資料
    /// </summary>
    [Serializable]
    public class MapData {
        public int Width;          //地圖寬度
        public int Height;         //地圖高度
        public int[,] Floor;       //地面Node的位置
        public int[,] Wall;        //牆壁Node的位置
        public int[,] Building;    //建築Node的位置
    }


}

namespace Game.Synchronism {

    /// <summary>
    /// 保存玩家座標
    /// </summary>
    public class Player {
        public float X_Coor;
        public float Y_Coor;
        public float Z_Coor;
    }

}
