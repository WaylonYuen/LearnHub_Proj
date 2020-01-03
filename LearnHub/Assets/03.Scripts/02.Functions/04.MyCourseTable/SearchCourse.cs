using System.Collections;
using System.Collections.Generic;
using Data.DataBuffer;
using UnityEngine;
using UnityEngine.UI;

namespace Functions {

    public class SearchCourse : MonoBehaviour {

        [SerializeField]
        private GameObject[] CourseSection;
        [SerializeField]
        private GameObject[] SelectBtn;

        private int Index;
        public static int GetSearchIndex;

        private void Start() {
            Index = -1;
            GetSearchIndex = -1;
        }

        private void Update() {
            if (GetSearchIndex != -1) {
                Index++;
                GetSearchIndex--;
                SetMyCourse(Index); //例外處理：沒有資料的情況
            }
        }

        /// <summary>
        /// 設定搜索結果
        /// </summary>
        public void SetMyCourse(int Num) {

            //父物件 Section
            GameObject CourseNameObj = CourseSection[Num].transform.GetChild(0).gameObject;     //課程名稱
            GameObject ScoreObj = CourseSection[Num].transform.GetChild(1).gameObject;          //學分
            GameObject ElectiveObj = CourseSection[Num].transform.GetChild(2).gameObject;       //必選修
            GameObject Classs = CourseSection[Num].transform.GetChild(3).gameObject;            //開課班級
            GameObject Location = CourseSection[Num].transform.GetChild(4).gameObject;         //上課時間or地點
            GameObject Teacher = CourseSection[Num].transform.GetChild(5).gameObject;           //授課老師
            GameObject Quota = CourseSection[Num].transform.GetChild(6).gameObject;             //名額

            //子物件 course
            GameObject SubCourseNameObj = CourseNameObj.transform.GetChild(0).gameObject;
            GameObject SubScoreObj = ScoreObj.transform.GetChild(0).gameObject;
            GameObject SubElectiveObj = ElectiveObj.transform.GetChild(0).gameObject;
            GameObject SubClasss = Classs.transform.GetChild(0).gameObject;
            GameObject SubLocation = Location.transform.GetChild(0).gameObject;
            GameObject SubTeacher = Teacher.transform.GetChild(0).gameObject;
            GameObject SubQuota = Quota.transform.GetChild(0).gameObject;

            ////text
            Text CourseNameTxt = SubCourseNameObj.GetComponent<Text>();
            Text ScoreTxt = SubScoreObj.GetComponent<Text>();
            Text ElectiveTxt = SubElectiveObj.GetComponent<Text>();
            Text ClasssTxt = SubClasss.GetComponent<Text>();
            Text LocationTxt = SubLocation.GetComponent<Text>();
            Text TeacherTxt = SubTeacher.GetComponent<Text>();
            Text QuotaTxt = SubQuota.GetComponent<Text>();

            CourseNameTxt.text = DataList.SearchCourse.CourseName;
            ScoreTxt.text = DataList.SearchCourse.CourseScore.ToString();
            ElectiveTxt.text = DataList.SearchCourse.Elective;
            ClasssTxt.text = DataList.SearchCourse.CourseClass;
            LocationTxt.text =
                DataList.SearchCourse.DateOfWeek1 + " " + DataList.SearchCourse.Section1 + " " + DataList.SearchCourse.Location1 +
                DataList.SearchCourse.DateOfWeek2 + " " + DataList.SearchCourse.Section2 + " " + DataList.SearchCourse.Location2 +
                DataList.SearchCourse.DateOfWeek3 + " " + DataList.SearchCourse.Section3 + " " + DataList.SearchCourse.Location3;
            TeacherTxt.text = DataList.SearchCourse.Teacher;
            QuotaTxt.text = DataList.SearchCourse.Quota.ToString();


            SubCourseNameObj.SetActive(true);
            SubScoreObj.SetActive(true);
            SubElectiveObj.SetActive(true);
            SubClasss.SetActive(true);
            SubLocation.SetActive(true);
            SubTeacher.SetActive(true);
            SubQuota.SetActive(true);

            SelectBtn[Num].SetActive(true);
        }
    }
}