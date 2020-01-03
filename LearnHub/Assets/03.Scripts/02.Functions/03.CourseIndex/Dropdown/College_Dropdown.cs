using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class College_Dropdown : MonoBehaviour {
    public static int dropdownIndex { get; private set; }
    private int TargetIndexSave;

    private List<string> CollegeNameIndex0 = new List<string>() { " 學院" };
    private List<string> CollegeNameIndex1 = new List<string>() { "學院", "資電學院", "商學院", "建設學院", "金融學院" };
    private List<string> CollegeNameIndex2 = new List<string>() { "index[1,0]", "index[1,1]", "index[1,2]", "index[1,3]" };
    private List<string> CollegeNameIndex3 = new List<string>() { "index[2,0]", "index[2,1]", "index[2,2]", "index[2,3]" };
    private List<string> CollegeNameIndex4 = new List<string>() { "index[3,0]", "index[3,1]", "index[3,2]", "index[3,3]" };


    public Dropdown dropdown;
    public Text selectedName;
    private int GetIndex;
    public void Dropdown_IndexChanged(int index) {
        GetIndex = index;
        dropdownIndex = index;
    }

    private void Start() {
        dropdownIndex = -1;
        TargetIndexSave = Degree_Dropdown.dropdownIndex;
        PopulateList();
    }

    public void PopulateList() {
        dropdown.AddOptions(CollegeNameIndex0);
    }

    private void Update() {

        if(Degree_Dropdown.dropdownIndex != TargetIndexSave) {
            TargetIndexSave = Degree_Dropdown.dropdownIndex;
            dropdown.ClearOptions();

            if (TargetIndexSave == 1) {
                dropdown.AddOptions(CollegeNameIndex1);
            }

            if (TargetIndexSave == 2) {
                dropdown.AddOptions(CollegeNameIndex2);
            }

            if (TargetIndexSave == 3) {
                dropdown.AddOptions(CollegeNameIndex3);
            }

            if (TargetIndexSave == 4) {
                dropdown.AddOptions(CollegeNameIndex4);     
            }

            //dropdownIndex = 0;
        }  
    }

    public void OnPointerClick(PointerEventData eventData) {
        //dropdownIndex = -1;
        //dropdownIndex = GetIndex;
    }
}
