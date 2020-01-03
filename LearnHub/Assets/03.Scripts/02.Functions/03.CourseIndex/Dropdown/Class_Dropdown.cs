using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Class_Dropdown : MonoBehaviour {
    public static int dropdownIndex { get; private set; }
    private int TargetIndexSave;
    private int College_TargetIndexSave;

    private List<string> ClassNameIndex0 = new List<string>() { " 班級" };
    private List<string> ClassNameIndex1 = new List<string>() { "班級", "資訊一甲", "資訊一乙", "資訊二甲", "資訊二乙", "資訊二丙", "資訊二丁"};
    private List<string> ClassNameIndex2 = new List<string>() { "班級", "會計一甲", "會計一乙", "會計二甲", "會計二乙", "input資料不足" };
    private List<string> ClassNameIndex3 = new List<string>() { "班級", "都資一甲", "都資一乙", "都資二甲", "都資二乙", "input資料不足" };
    private List<string> ClassNameIndex4 = new List<string>() { "班級", "財金一甲", "財金一乙", "財金二甲", "財金二乙", "input資料不足" };
    private List<string> ClassNameIndex5 = new List<string>() { "input資料不足" };


    public Dropdown dropdown;
    public Text selectedName;
    private int GetIndex;
    public void Dropdown_IndexChanged(int index) {
        GetIndex = index;
        dropdownIndex = index;
    }

    private void Start() {
        dropdownIndex = -1;
        TargetIndexSave = Department_Dropdown.dropdownIndex;
        College_TargetIndexSave = College_Dropdown.dropdownIndex;
        PopulateList();
    }

    public void PopulateList() {
        dropdown.AddOptions(ClassNameIndex0);
    }

    private void Update() {


        if (Department_Dropdown.dropdownIndex != TargetIndexSave) {
            TargetIndexSave = Department_Dropdown.dropdownIndex;
            College_TargetIndexSave = College_Dropdown.dropdownIndex;
            dropdown.ClearOptions();

            if(College_TargetIndexSave == 1 && TargetIndexSave == 1) {
                dropdown.AddOptions(ClassNameIndex1);
            }

            if (College_TargetIndexSave == 2 && TargetIndexSave == 1) {
                dropdown.AddOptions(ClassNameIndex2);
            }

            if (College_TargetIndexSave == 3 && TargetIndexSave == 3) {
                dropdown.AddOptions(ClassNameIndex3);
            }

            if (College_TargetIndexSave == 4 && TargetIndexSave == 1) {
                dropdown.AddOptions(ClassNameIndex4);
            }

            //dropdownIndex = 0;
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        //dropdownIndex = -1;
        //dropdownIndex = GetIndex;
    }
}
