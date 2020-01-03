using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LearnHub.Input.Keyboard;

namespace LearnHub.Functions.Login {

    public class GetPassword : Keyboard {

        protected override void Update() {
            base.Update();
        }

        /// <summary>
        /// 取得keyboard內容後所需執行的方法
        /// </summary>
        protected override void GetKeyboardValue() {
            Login.Password = inputStr;
            OutputText.text = "********";
        }

    }
}
