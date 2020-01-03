using System.Collections;
using System.Collections.Generic;
using Data.DataBuffer;
using Network.Packet;
using UnityEngine;
using UnityEngine.UI;

namespace Functions {

    public class CourseSelectBtn : MonoBehaviour {
        [SerializeField]
        private int CourseIndex;
        [SerializeField]
        private GameObject ThisObj;
        [SerializeField]
        private GameObject BtnTxtObj;   //按鈕子物件

        private string JoinBtnStr = "加選";
        private string CancelBtnStr = "取消";
        private string EixtBtnStr = "退選";

        private bool isClick;
        private Text BtnTxt;
        private Send send;

        private void Start() {
            isClick = false;

            BtnTxt = BtnTxtObj.GetComponent<Text>();
            send = new Send();

            //##未進行課表判斷
            BtnTxt.text = JoinBtnStr;

            ThisObj.SetActive(false);   //隱藏
        }

        public void SelectBtn() {

            if (!isClick) {
                isClick = true;
                BtnTxt.text = EixtBtnStr;   //顯示退選
                DataList.CourseCart[CourseIndex] = 1;//加入資料清單
                string[] CourseID = { DataList.SearchCourseTemp[CourseIndex].CourseID };
                send.StringPacket_DB(StartOnline.MyClient.Player, CustomEnumType.DataBaseType.SelectCourse, CourseID);
            } else {
                isClick = false;
                BtnTxt.text = JoinBtnStr;
                DataList.CourseCart[CourseIndex] = 0;//去除
                string[] CourseID = { DataList.SearchCourseTemp[CourseIndex].CourseID };
                send.StringPacket_DB(StartOnline.MyClient.Player, CustomEnumType.DataBaseType.ExitCourse, CourseID);
            }

            Debug.Log($"SearchCourseTemp[{CourseIndex}] : {DataList.CourseCart[CourseIndex]}"); //Log
        }
    }

}