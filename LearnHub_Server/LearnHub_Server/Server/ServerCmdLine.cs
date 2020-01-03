using System;

namespace LearnHub_Server.Server {

    /// <summary>
    /// 服務器命令控制介面
    /// </summary>
    public static class ServerCmdLine {

        private static int request;

        /// <summary>
        /// 命令行模式
        /// </summary>
        public static void CmdLineMode() {
            
        }


        public static void Command(string modeStr) {

            switch (modeStr) {

                case "help":
                    Menu();
                    break;

                case "cmd mode":
                    Console.WriteLine("Test cmd mode");
                    break;

                default:
                    Console.WriteLine("Error : You have an error in your Server syntax;");
                    break;
            }
        }


        private static void Menu() {
            Console.WriteLine(
                "\n-------  ServerCmdLine  ---------\n\n" +
                "\t \"help\"\t-\t" + "命令集\n" +
                "\t \"x\"\t-\t" + "？？\n" +
                "\t \"x\"\t-\t" + "？？\n" +
                "\t \"x\"\t-\t" + "？？\n" +
                "\t \"exit\"\t-\t" + "關閉服務器\n\n" +
                "----------------------------------\n"
            );

        }

    }

}