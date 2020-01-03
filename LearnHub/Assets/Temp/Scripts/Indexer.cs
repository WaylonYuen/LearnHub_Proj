using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Management {

    /// <summary>
    /// 索引器
    /// </summary>
	public class Indexer : MonoBehaviour {

        static public int size = 10;
        private string[] namelist = new string[size];
        

        //Instance
        public Indexer() {
            for (int i = 0; i < size; i++)
                namelist[i] = "N/A";
        }


        public string this[int index] {
            get {
                string tmp;

                if (index >= 0 && index <= size - 1) {
                    tmp = namelist[index];
                } else {
                    tmp = "";
                }

                return (tmp);
            }

            set {
                if (index >= 0 && index <= size - 1) {
                    namelist[index] = value;
                }
            }
        }

    }

}