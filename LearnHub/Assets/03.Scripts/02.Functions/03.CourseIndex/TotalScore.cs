using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Functions {

    public class TotalScore : MonoBehaviour {

        public GameObject ChatperMarkObj;

        private static int Quesion = 3;

        public static int[] Answer = new int[Quesion];       //學生作答
        public static int[] CorrAns = new int[Quesion];      //正確答案

        public static int MyScore;

        private void Start() {
            MyScore = 0;
            CorrAns[0] = 3;
            CorrAns[1] = 1;
            CorrAns[2] = 4;
        }

        public void CalculusScore() {
            MyScore = 0;
            for (int i = 0; i < Quesion; i++) {
                if (CorrAns[i] == Answer[i])
                    MyScore += 35;
            }

            Text Txt = ChatperMarkObj.GetComponent<Text>();
            Txt.text = "Score: " + MyScore;
        }
    }

}
