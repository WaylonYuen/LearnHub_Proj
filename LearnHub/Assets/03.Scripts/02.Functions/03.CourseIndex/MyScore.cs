using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Functions {

    public class MyScore : MonoBehaviour {

        public int Quesion;

        public void AnsA(bool value) {
            if(value)
                TotalScore.Answer[Quesion] = 1;
            else
                TotalScore.Answer[Quesion] = 0;
        }

        public void AnsB(bool value) {
            if (value)
                TotalScore.Answer[Quesion] = 2;
            else
                TotalScore.Answer[Quesion] = 0;
        }

        public void AnsC(bool value) {
            if (value)
                TotalScore.Answer[Quesion] = 3;
            else
                TotalScore.Answer[Quesion] = 0;
        }

        public void AnsD(bool value) {
            if (value)
                TotalScore.Answer[Quesion] = 4;
            else
                TotalScore.Answer[Quesion] = 0;
        }

        public void AnsE(bool value) {
            if (value)
                TotalScore.Answer[Quesion] = 5;
            else
                TotalScore.Answer[Quesion] = 0;
        }

    }
}