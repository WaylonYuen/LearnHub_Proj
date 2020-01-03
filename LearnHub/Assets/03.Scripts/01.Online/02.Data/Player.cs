using System.Net.Sockets;

using CustomEnumType;

public class Player {

    //連線資料
    public Socket Socket { get; private set; }

    //封包資料
    public int Crccode { get; set; }                            //封包驗證碼
    public long SessionID { get; set; }                         //身份ID
    public EncryptionType EncryptionMethods { get; set; }       //加密方式

    //遊戲資料
    public bool Ready { get; set; } //等待遊戲房間開始遊戲(true: 等待開始, false: 未等待)
    public bool InRoom { get; set; }

    //Instance
    public Player(Socket socket) {
        Socket = socket;

        Ready = false;
        InRoom = false;
    }

}
