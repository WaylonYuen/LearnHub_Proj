using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Department_Dropdown : MonoBehaviour {
    public static int dropdownIndex { get; private set; }
    private int TargetIndexSave;

    private List<string> DepartmentNameIndex0 = new List<string>() { " 系所" };
    private List<string> DepartmentNameIndex1 = new List<string>() { "系所", "資訊系", "電機系", "電子系", "通訊系", "自控系", "資電系" };    //資電
    private List<string> DepartmentNameIndex2 = new List<string>() { "系所", "會計系", "企管系", "財稅系", "經濟系" };    //商學
    private List<string> DepartmentNameIndex3 = new List<string>() { "系所", "土木系", "水利系", "都資系", "土管系" };    //建設
    private List<string> DepartmentNameIndex4 = new List<string>() { "系所", "財金系", "風保系" };    //金融


    public Dropdown dropdown;
    public Text selectedName;
    private int GetIndex;
    public void Dropdown_IndexChanged(int index) {
        GetIndex = index;
        dropdownIndex = index;
    }

    private void Start() {
        dropdownIndex = -1;
        TargetIndexSave = College_Dropdown.dropdownIndex;
        PopulateList();
    }

    public void PopulateList() {
        dropdown.AddOptions(DepartmentNameIndex0);
    }

    private void Update() {

        if (College_Dropdown.dropdownIndex != TargetIndexSave) {
            TargetIndexSave = College_Dropdown.dropdownIndex;
            dropdown.ClearOptions();

            if (TargetIndexSave == 1) {
                dropdown.AddOptions(DepartmentNameIndex1);
            }

            if (TargetIndexSave == 2) {
                dropdown.AddOptions(DepartmentNameIndex2);
            }

            if (TargetIndexSave == 3) {
                dropdown.AddOptions(DepartmentNameIndex3);
            }

            if (TargetIndexSave == 4) {
                dropdown.AddOptions(DepartmentNameIndex4);
            }

            //dropdownIndex = 0;
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        //dropdownIndex = -1;
        //dropdownIndex = GetIndex;
    }
}
