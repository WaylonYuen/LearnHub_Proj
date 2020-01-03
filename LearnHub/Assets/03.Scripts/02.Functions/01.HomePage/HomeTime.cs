using System;
using UnityEngine;
using UnityEngine.UI;

namespace Functions {

    public class HomeTime : MonoBehaviour {

        private string MM;
        private string dd;
        private string DateOfWeek;
        private string DateTimeStr;

        [SerializeField]
        private Text DateTimeTxt;
        [SerializeField]
        private float reflashTime;          //刷新間隔時間
        [SerializeField]
        private bool reflashLock;           //刷新鎖

        private float now;                  //目前
        private float FirstTimeRecord;      //第一次紀錄

        private void Start() {
            DateTimeStr = "N/A";
            MM = DateTime.Now.ToString("MM");
            dd = DateTime.Now.ToString("dd");
            DateOfWeek = DateTime.Now.DayOfWeek.ToString();
            FirstTimeRecord = Time.realtimeSinceStartup;
        }

        private void Update() {

            now = Time.realtimeSinceStartup;                    //紀錄目前時間
            float timeDifference = now - FirstTimeRecord;       //計算持續按下的時間

            if (timeDifference > reflashTime || !reflashLock) {
                MM = DateTime.Now.ToString("MM");
                dd = DateTime.Now.ToString("dd");
                DateOfWeek = DateTime.Now.DayOfWeek.ToString();
                FirstTimeRecord = Time.realtimeSinceStartup;

                SetDateTimeTxt();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void SetDateTimeTxt() {
            DateTimeFormat_WWddMM();
            DateTimeTxt.text = DateTimeStr;
        }

        /// <summary>
        /// 日期時間格式: Week, day Month
        /// </summary>
        private void DateTimeFormat_WWddMM() {
            DateTimeStr = DateOfWeek + ", " + dd + " " + ToMonthName(MM);
        }

        /// <summary>
        /// 月份數字轉月份英文全稱
        /// </summary>
        /// <param name="MM"></param>
        /// <returns></returns>
        public string ToMonthName(string MM) {
            switch (MM) {
                case "01": return "January";
                case "02": return "February";
                case "03": return "March";
                case "04": return "April";
                case "05": return "May";
                case "06": return "June";
                case "07": return "July";
                case "08": return "August";
                case "09": return "September";
                case "10": return "October";
                case "11": return "November";
                case "12": return "December";
                default: return "ToMonthName: reference Error!";
            }
        }
    }

}
