using System.Collections;
using System.Collections.Generic;
using Network.Packet;
using UnityEngine;

public class DataTest : MonoBehaviour {

    // Start is called before the first frame update
    void Start() {
        Send send = new Send();
        send.BoolPacket_DB(StartOnline.MyClient.Player, CustomEnumType.DataBaseType.GetCourseData, true);
    }
}
