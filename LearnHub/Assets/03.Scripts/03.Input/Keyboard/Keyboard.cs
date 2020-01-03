using UnityEngine;
using UnityEngine.UI;

namespace LearnHub.Input.Keyboard {

    public class Keyboard : MonoBehaviour {

        protected string inputStr = "N/A";

        public Text OutputText;
        private TouchScreenKeyboard TouchScreenKeyboard;

        /// <summary>
        /// 打開鍵盤
        /// </summary>
        public void OpenKeyboard() {
            TouchScreenKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        }

        // Update is called once per frame
        protected virtual void Update() {

            if (TouchScreenKeyboard != null && TouchScreenKeyboard.visible) {
                if (TouchScreenKeyboard.status == TouchScreenKeyboard.Status.Done) {
                    inputStr = TouchScreenKeyboard.text;
                    TouchScreenKeyboard = null;
                    GetKeyboardValue();
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        protected virtual void GetKeyboardValue() {

        }
    }

}
