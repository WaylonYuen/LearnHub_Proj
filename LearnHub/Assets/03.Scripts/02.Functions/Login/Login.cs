using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Network.Packet;
using CustomEnumType;


namespace LearnHub.Functions.Login {

    public class Login : MonoBehaviour {

        public static string ID { private get; set; }
        public static string Password { private get; set; }

        [SerializeField]
        private Text LoginInfoTxt;  //登入訊息顯示
        public static bool IsPass;
        private static bool LoadSecne;

        private static bool Locker;

        private void Start() {
            LoginInfoTxt.text = "Login";
            LoadSecne = false;
            Locker = true;
        }

        private void Update() {
            if(!Locker)
                if (!IsPass) {
                    LoginInfoTxt.text = "Login Fail!";
                }

            if (LoadSecne) {
                SceneManager.LoadScene(1);
                LoadSecne = false;
            }
        }

        #region 按鈕事件
        /// <summary>
        /// Submit用戶登入資料
        /// </summary>
        public void Submit() {

            Send Send = new Send();

            //ID = "123456";
            //Password = "Password";

            //檢查
            if (!CheckSubmitData()) return;

            long UID = 0;
            //ID型態轉換 string -> long
            try {
                UID = Int64.Parse(ID);
            } catch (FormatException e) {
                Debug.Log(e.Message);
            }

            //封裝發送
            Debug.Log($"Login Sends : ID {UID} | PW {Password}");   //Test

            //TTT
            string[] PW = { Password }; //型態轉換
            Send.LoginPacket(StartOnline.MyClient.Player, DataBaseType.Login, UID, PW);      //發送用戶ID及密碼（Login資料）
        }

        /// <summary>
        /// 發送資料前的格式檢查
        /// </summary>
        /// <returns>是否通過檢查</returns>
        private bool CheckSubmitData() {

            bool check = true;

            //檢查ID範圍
            if (ID.Length < 4 || ID.Length > 8) {
                Debug.Log($"# Error! 'ID' Length is out of range[4,8] : {ID.Length}");
                check = false;
                Locker = false;
            }

            //檢查密碼範圍
            if (Password.Length < 8 || Password.Length > 16) {
                Debug.Log($"# Error! 'Password' Length is out of range[8,16] : {Password.Length}");
                check = false;
                Locker = false;
            }

            return check;
        }
        #endregion

        /// <summary>
        /// 登入檢驗
        /// </summary>
        public static void Inspect(Player player) {
            Send Send = new Send();
            if (IsPass) {
                //等待服務器發送資料(異步)
                //Send.BlankPacket(player, DataBaseType.GetData);

                LoadSecne = true;
                Debug.Log("登入成功");
            } else {
                Debug.Log("登入失敗");
            }
        }
    }

}

