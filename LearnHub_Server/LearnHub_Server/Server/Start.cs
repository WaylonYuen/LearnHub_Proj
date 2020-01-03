using System;
using Network.Packet;
using CustomEnumType;

using LearnHub_Server.Server;

/// <summary>
/// 項目入口
/// </summary>
class Start {

    public static Server server;

    private static void Main() {

        //服務器初始化
        Server.isQuit = false;  //關閉服務器退出信號燈
        server = new Server();  //初始化Server (構建所有必要結構)

        Console.WriteLine("正在啟動服務器...\n");
        server.Start();         //啟動Server  （持續監聽）

        //###未完成：服務器控制介面 CMD Line   
        while (true) {
            string cmd = Console.ReadLine().ToLower();
            if (cmd.Equals("exit")) {
                Console.WriteLine("#  Bye");
                break;
            }
            ServerCmdLine.Command(cmd);
        }
        
        Server.isQuit = true;   //開啟服務器退出信號燈
        Console.ReadKey();
        Console.WriteLine("#  Server is Close");
    }

}

