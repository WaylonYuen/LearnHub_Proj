using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Functions {

    public class ShowAndHide : MonoBehaviour {

        [SerializeField] private GameObject[] ShowObj;
        [SerializeField] private GameObject[] HideObj;

        public void ClickBtn() {

            for (int i = 0; i < ShowObj.Length; i++) {
                if (ShowObj[i] != null)
                    ShowObj[i].SetActive(true);
            }

            for (int i = 0; i < HideObj.Length; i++) {
                if (HideObj[i] != null)
                    HideObj[i].SetActive(false);
            }
        }

    }

}
