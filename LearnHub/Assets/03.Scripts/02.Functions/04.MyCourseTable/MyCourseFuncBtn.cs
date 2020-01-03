using System.Collections;
using System.Collections.Generic;
using Network.Packet;
using UnityEngine;

namespace Functions {

    public class MyCourseFuncBtn : MonoBehaviour {

        Send send = new Send();
        public static int MyCourseIndexTemp;

        public void GetCourseBtn() {
            MyCourseIndexTemp = 0;
            send.BoolPacket_DB(StartOnline.MyClient.Player, CustomEnumType.DataBaseType.GetCourseData, true);
        }

    }
}
