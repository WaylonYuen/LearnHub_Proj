using CustomEnumType;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Network.Packet;

public class BtnMatchRoomExit : Button, IPointerDownHandler, IPointerUpHandler {

    public override void OnPointerDown(PointerEventData eventData) {

        Send Send = new Send();
        //Send.BoolPacket(StartOnline.MyClient.Player, PackageType.LeaveRoom, true);//發送訊息給Server表示離開房間

        //GameDataList.SyncMapData = null;
        //GameDataList.SyncPlayers = null;
        StartOnline.MyClient.Player.Ready = false;

        base.OnPointerDown(eventData);

        
        SceneManager.LoadScene(1);  //切換到遊戲主頁面(玩家首頁)

    }

    public override void OnPointerUp(PointerEventData eventData) {
        base.OnPointerUp(eventData);
    }

}
