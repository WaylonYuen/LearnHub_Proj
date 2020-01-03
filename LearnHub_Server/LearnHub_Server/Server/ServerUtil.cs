using System;
using CustomEnumType;

/// <summary>
/// 服務器工具
/// </summary>
public static class ServerUtil {

    private static Random random;

    public static EncryptionType EncrytionMethod { get => EncryptionType.RES256; }   // # 未完成 暫時

    /// <summary>
    /// 隨機產生N位驗證碼
    /// </summary>
    /// <param name="LowerBound">上界</param>
    /// <param name="UpperBound">下界</param>
    /// <returns>隨機數</returns>
    public static int RandomNum(int LowerBound, int UpperBound) {
        random = new Random();
        int Num = random.Next(LowerBound, UpperBound);   //產生6位隨機數
        return Num;
    }

    /// <summary>
    /// 隨機產生字串KEY = password
    /// </summary>
    /// <param name="Lenght">Key長度</param>
    /// <param name="useArray">Key需要用何種方式保存</param>
    /// <returns></returns>
    public static string RandomKey(int Lenght, bool useArray) {

        //可被產生出的符號
        char[] constant = {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        };

        string KEY = string.Empty;  //清空
        random = new Random();

        for (int i = 0; i < Lenght; i++) {
            KEY += constant[random.Next(62)].ToString();    //產生
        }

        if (useArray) return KEY;   //陣列格式(回傳)

        //轉換成非陣列String
        string[] ArrayKEY = { "N/A" };
        ArrayKEY[0] = KEY;
        string Key = ArrayKEY[0];

        return Key;
    }

}

/// <summary>
/// 服務器控制介面 # 未完成
/// </summary>
public class Server_CommandLine {

    private int CommandRequest;
    private string Password;

    private enum Request {
        Help,
        Menu,
        Quit,
    }

    //此類唯一安全接口
    public void InputServer_CommandLine(int commandRequest) {
        CommandRequest = commandRequest;
    }

    /// <summary>
    /// 命令行 菜單
    /// </summary>
    public void Server_CommandLineMenu() {
        Console.WriteLine("Command Line Menu");
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine($"*  Plz enter the number of request:");
        Console.WriteLine($"*  ");
        Console.WriteLine($"*  Help\t - {(int)Request.Help} -");
        Console.WriteLine($"*  Menu\t - {(int)Request.Menu} -");
        Console.WriteLine($"*  Quit\t - {(int)Request.Quit} -");
        Console.WriteLine("-------------------------------------------");
    }

    /// <summary>
    /// 執行命令
    /// </summary>
    public void Server_CommandLineExecute() {

        switch ((Request)CommandRequest) {
            case Request.Help:
                break;
            case Request.Menu:
                Server_CommandLineMenu();
                break;
            case Request.Quit:
                Command_Quit();
                break;
            default:
                Console.WriteLine("Plz enter the corr command number!");
                break;
        }
    }

    #region 命令方法
    /// <summary>
    /// 結束Server運行
    /// </summary>
    private void Command_Quit() {
        Console.WriteLine("Warning: Quiz Server!");
        Console.WriteLine("-------------------------------");
        Console.WriteLine($"Plz enter panagement password:");
        string enterPassword = Console.ReadLine();
        if (enterPassword == Password) {
            Server.isQuit = true;
        } else {
            Console.WriteLine("Password is error!");
        }
    }
    #endregion
}
