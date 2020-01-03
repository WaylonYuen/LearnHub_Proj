using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System.Collections;

using CustomEnumType;
using Network.Packet;

public class OffLine : MonoBehaviour {

	void OnApplicationQuit() {

		Send Send = new Send();

		if (!StartOnline.MyClient.Socket.Connected) {
            StartOnline.MyClient.Socket.Shutdown(SocketShutdown.Both); //關閉Socket 接收\發送 gate
            StartOnline.MyClient.Socket.Close();                       //關閉Socket
		}
		else
			Send.BoolPacket(StartOnline.MyClient.Player, PackageType.Connection, false);//發送訊息給Server表示Shutdown
        StartOnline.isQuit = true;  //遊戲已退出
		Debug.Log("Close Apps");
	}

}
