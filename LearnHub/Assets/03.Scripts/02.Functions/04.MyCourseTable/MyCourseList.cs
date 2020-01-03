using System.Collections;
using System.Collections.Generic;
using Data.DataBuffer;
using UnityEngine;
using UnityEngine.UI;

namespace Functions {

    public class MyCourseList : MonoBehaviour {

        [SerializeField]
        private GameObject[] CourseSection;
        private GameObject CourseTableIndexObj;

        public static int MyCourseIndex;
        private int Index;

        private void Awake() {
            DataList.CourseDataTemp = new Data.DataBuffer.CourseData[50];
        }

        private void Start() {
            Index = -1;
            MyCourseIndex = -1;
        }


        private void Update() {

            if (MyCourseIndex != -1) {
                Index++;
                MyCourseIndex--;

                if(DataList.CourseDataTemp[Index].Section1 != "N/A") {
                    SetMyCourse(
                        int.Parse(DataList.CourseDataTemp[Index].Section1),
                        int.Parse(DataList.CourseDataTemp[Index].DateOfWeek1),
                        DataList.CourseDataTemp[Index].CourseName,
                        DataList.CourseDataTemp[Index].Location1);
                }
                if (DataList.CourseDataTemp[Index].Section2 != "N/A") {
                    SetMyCourse(
                        int.Parse(DataList.CourseDataTemp[Index].Section2),
                        int.Parse(DataList.CourseDataTemp[Index].DateOfWeek2),
                        DataList.CourseDataTemp[Index].CourseName,
                        DataList.CourseDataTemp[Index].Location2);
                }
                if (DataList.CourseDataTemp[Index].Section3 != "N/A") {
                    SetMyCourse(
                        int.Parse(DataList.CourseDataTemp[Index].Section3),
                        int.Parse(DataList.CourseDataTemp[Index].DateOfWeek3),
                        DataList.CourseDataTemp[Index].CourseName,
                        DataList.CourseDataTemp[Index].Location3);
                }
            }

        }




        /// <summary>
        /// 設定用戶課表
        /// </summary>
        /// <param name="Section">時段</param>
        /// <param name="DateOfWeek">星期</param>
        /// <param name="CourseName">課程名稱</param>
        /// <param name="Location">上課地點</param>
        public void SetMyCourse(int Section, int DateOfWeek, string CourseName, string Location) {

            CourseTableIndexObj = CourseSection[Section].transform.GetChild(DateOfWeek).gameObject;     //獲取子物件
            CourseTableIndexObj = CourseTableIndexObj.transform.GetChild(0).gameObject;                 //子物件中的子物件

            GameObject CourseNameObj = CourseTableIndexObj.transform.GetChild(1).gameObject;
            GameObject LocationObj = CourseTableIndexObj.transform.GetChild(2).gameObject;

            Text NameTxt = CourseNameObj.GetComponent<Text>();
            Text LocationTxt = LocationObj.GetComponent<Text>();

            NameTxt.text = CourseName;
            LocationTxt.text = Location;

            CourseTableIndexObj.SetActive(true);    //顯示
        }

        /// <summary>
        /// 清空課表資料
        /// </summary>
        public void ClearMyCourse() {

            for (int i = 0; i < CourseSection.Length; i++) {
                for(int j = 0; j < 7; j++) {
                    CourseTableIndexObj = CourseSection[i].transform.GetChild(j).gameObject;    //獲取子物件
                    CourseTableIndexObj = CourseTableIndexObj.transform.GetChild(0).gameObject; //子物件中的子物件
                    CourseTableIndexObj.SetActive(false);
                }
            }

        }

    }
}