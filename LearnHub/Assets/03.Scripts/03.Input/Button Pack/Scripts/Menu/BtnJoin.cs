using CustomEnumType;
using Network.Packet;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class BtnJoin : Button, IPointerDownHandler, IPointerUpHandler {

    private Send send;

    protected override void Start() {
        send = new Send();
        base.Start();
    }


    public override void OnPointerDown(PointerEventData eventData) {

        Setup.RefRoom.RoomMembers = 0;
        GamingSceneLoader.CanStart = false; //鎖定遊戲加載,需要等待遊戲資料同步後才可進行加載

        //測試創建房間
        if (StartOnline.MyClient.Player.Ready == false) {
            //send.BoolPacket(StartOnline.MyClient.Player, PackageType.SrcRoom, true);    //testing: 搜尋房間並加入or創建
            StartOnline.MyClient.Player.Ready = true;
        }

        base.OnPointerDown(eventData);

        
        SceneManager.LoadScene(2);  //切換房間等待畫面

    }

    public override void OnPointerUp(PointerEventData eventData) {
        base.OnPointerUp(eventData);
    }

}
