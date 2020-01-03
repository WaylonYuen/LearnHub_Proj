using System;
using System.IO;
using Game.DataBuff;
using Setup;

namespace Game.Setup {

    /// <summary>
    /// 設定遊戲資料
    /// </summary>
    public static class Initial {

        #region 地圖
        public static void MapData(MapData mapData) {

            mapData.Width = RefMap.Width;
            mapData.Height = RefMap.Height;
            mapData.Floor = new int[mapData.Width, mapData.Height];
            mapData.Wall = new int[mapData.Width, mapData.Height];
            mapData.Building = new int[mapData.Width, mapData.Height];

            Set2(mapData);
            Show(mapData);

        }

        private static void Set1(MapData mapData) {
            for (int y = 0; y < RefMap.Height; y++) {
                for (int x = 0; x < RefMap.Width; x++) {
                    mapData.Floor[x, y] = RefMap.Floor;        //初始化地板

                    //初始化牆壁
                    if (x == 0 || y == 0 || x == RefMap.Width || y == RefMap.Height)
                        mapData.Wall[x, y] = 2;
                    else
                        mapData.Wall[x, y] = 0;



                    mapData.Building[x, y] = RefMap.Building;  //初始化建築
                }
            }
        }

        private static void Set2(MapData mapData) {

            MapSet("map.txt", FileMode.Open, FileAccess.Read);

            int i = 0;
            for (int y = 0; y < RefMap.Height; y++) {
                for (int x = 0; x < RefMap.Width; x++) {

                    mapData.Floor[x, y] = RefMap.Floor;        //初始化地板
                    mapData.Wall[x, y] = WallData[i]; i++;
                    mapData.Building[x, y] = RefMap.Building;  //初始化建築
                }
            }
        }

        public static int[] WallData = new int[3600];

        public static void MapSet(string srcPath, FileMode fileMode, FileAccess fileAccess) {

            try {
                using (FileStream fileStream = new FileStream(srcPath, fileMode, fileAccess)) {
                    using (StreamReader streamReader = new StreamReader(fileStream)) {

                        int i = 0;
                        string line = "";

                        while (!streamReader.EndOfStream) {             // 判读是否读完

                            line = streamReader.ReadLine();             // 读取一行

                            string[] points = line.Split(' ');          // 拆分当前行
                            foreach (string item in points) {           // 转换 string 为 int  
                                int.TryParse(item, out int vertice);
                                //Console.Write(vertice + " ");
                                WallData[i] = vertice;
                                i++;
                            }
                        }

                    }
                }
            } catch {
                Console.WriteLine($"\n#  Warning: Cannot found the Path\t Info [Path: {srcPath} ]");
            }

        }

        private static void Show(MapData mapData) {
            //Testing(show)
            for (int y = 0; y < RefMap.Height; y++) {
                for (int x = 0; x < RefMap.Width; x++) {
                    Console.Write($" {mapData.Wall[x, y]}");
                }
                Console.WriteLine(" ");
            }
        }
        #endregion

        //Test
        public static bool[] InitialBoolean(bool[] Data, bool Set) {
            for (int i = 0; i < Data.Length; i++) Data[i] = Set;
            return Data;
        }
    }
}
