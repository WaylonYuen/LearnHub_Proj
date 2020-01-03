using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownController : MonoBehaviour {

    public enum names {
        None,
        Degree,
        College,
        Department,
        Class
    }

    public names nameList;
    public GameObject[] dropdownObj;
    private static int[] selector = new int[4];

    //未優化 -> json

    //部門
    private List<string> DegreeNameIndex = new List<string>() { "請選擇部門", "大學部", "碩士班", "博士班", "進修學士班" };

    //學院
    private List<string> CollegeNameIndex0 = new List<string>() { " 學院" };
    private List<string> CollegeNameIndex1 = new List<string>() { "資電學院", "商學院", "建設學院", "金融學院" };
    private List<string> CollegeNameIndex2 = new List<string>() { "index[1,0]", "index[1,1]", "index[1,2]", "index[1,3]" };
    private List<string> CollegeNameIndex3 = new List<string>() { "index[2,0]", "index[2,1]", "index[2,2]", "index[2,3]" };
    private List<string> CollegeNameIndex4 = new List<string>() { "index[3,0]", "index[3,1]", "index[3,2]", "index[3,3]" };

    //系
    private List<string> DepartmentNameIndex0 = new List<string>() { " 系所" };
    private List<string> DepartmentNameIndex1 = new List<string>() { "資訊系", "電機系", "電子系", "通訊系", "自控系", "資電系" };    //資電
    private List<string> DepartmentNameIndex2 = new List<string>() { "會計系", "企管系", "財稅系", "經濟系" };    //商學
    private List<string> DepartmentNameIndex3 = new List<string>() { "土木系", "水利系", "都資系", "土管系" };    //建設
    private List<string> DepartmentNameIndex4 = new List<string>() { "財金系", "風保系" };    //金融

    //班級
    private List<string> ClassNameIndex0 = new List<string>() { " 班級" };
    private List<string> ClassNameIndex1 = new List<string>() { "資訊一甲", "資訊一乙", "資訊二甲", "資訊二乙", "資訊二丙", "資訊二丁", "input資料不足" };
    private List<string> ClassNameIndex2 = new List<string>() { "會計一甲", "會計一乙", "會計二甲", "會計二乙", "input資料不足" };
    private List<string> ClassNameIndex3 = new List<string>() { "都資一甲", "都資一乙", "都資二甲", "都資二乙", "input資料不足" };
    private List<string> ClassNameIndex4 = new List<string>() { "財金一甲", "財金一乙", "財金二甲", "財金二乙", "input資料不足" };
    private List<string> ClassNameIndex5 = new List<string>() { "input資料不足" };

    public Dropdown dropdown;
    public Text selectedName;

    public void Dropdown_IndexChanged(int index) {

        Debug.Log($"Index : {index}");
        switch (nameList) {
            case names.Degree:
                Dropdown ListChange0 = dropdownObj[1].GetComponent<Dropdown>();
                ListChange0.ClearOptions();
                if (index == 1) { ListChange0.AddOptions(CollegeNameIndex1); break; }
                if (index == 2) { ListChange0.AddOptions(CollegeNameIndex2); break; }
                if (index == 3) { ListChange0.AddOptions(CollegeNameIndex3); break; }
                if (index == 4) { ListChange0.AddOptions(CollegeNameIndex4); break; }
                break;

            case names.College:
                Dropdown ListChange1 = dropdownObj[2].GetComponent<Dropdown>();
                ListChange1.ClearOptions();
                if (index == 0) { ListChange1.AddOptions(DepartmentNameIndex1); break; }
                if (index == 1) { ListChange1.AddOptions(DepartmentNameIndex2); break; }
                if (index == 2) { ListChange1.AddOptions(DepartmentNameIndex3); break; }
                if (index == 3) { ListChange1.AddOptions(DepartmentNameIndex4); break; }
                break;

            case names.Department:
                Dropdown ListChange2 = dropdownObj[3].GetComponent<Dropdown>();
                ListChange2.ClearOptions();
                if (index == 0) { ListChange2.AddOptions(ClassNameIndex1); break; }
                if (index == 1) { ListChange2.AddOptions(DepartmentNameIndex2); break; }
                if (index == 2) { ListChange2.AddOptions(DepartmentNameIndex3); break; }
                if (index == 3) { ListChange2.AddOptions(DepartmentNameIndex4); break; }
                break;

            case names.Class:
                dropdown.AddOptions(ClassNameIndex0);
                break;
        }
    }

    private void Start() {
        for (int i = 0; i < selector.Length; i++) selector[i] = 0;
        PopulateList();
    }

    public void PopulateList() {
        switch (nameList) {
            case names.Degree:
                dropdown.AddOptions(DegreeNameIndex);
                break;
            case names.College:
                dropdown.AddOptions(CollegeNameIndex0);
                break;
            case names.Department:
                dropdown.AddOptions(DepartmentNameIndex0);
                break;
            case names.Class:
                dropdown.AddOptions(ClassNameIndex0);
                break;
        }
    }

}
