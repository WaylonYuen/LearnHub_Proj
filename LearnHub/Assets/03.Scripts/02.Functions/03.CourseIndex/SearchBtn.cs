using System.Collections;
using System.Collections.Generic;
using Data.DataBuffer;
using Network.Packet;
using UnityEngine;

public class SearchBtn : MonoBehaviour {

    public static int SearchIndexTemp;

    public void Submit() {

        Send send = new Send();
        SearchData searchData = new SearchData();

        SearchIndexTemp = 0;
        CourseSelectController.ClearCourseCart();

        searchData.degree = DegreeToString(Degree_Dropdown.dropdownIndex);
        searchData.college = CollegeToString(College_Dropdown.dropdownIndex);
        searchData.department = DepartmentToString(College_Dropdown.dropdownIndex, Department_Dropdown.dropdownIndex);
        searchData.classs = ClassToString(Department_Dropdown.dropdownIndex, Class_Dropdown.dropdownIndex);

        Debug.Log($"de {searchData.degree} college {searchData.college} department {searchData.department}  class {searchData.classs}");

        byte[] SearchData_Byte = NetworkUtil.Serialize(searchData);
        send.BytePacket_DB(StartOnline.MyClient.Player, CustomEnumType.DataBaseType.SearchCourse, SearchData_Byte); //搜尋
    }

    /// <summary>
    /// 學位
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private string DegreeToString(int index) {
        switch (index) {
            case 1: return "大學部";
            case 2: return "碩士班";
            case 3: return "博士班";
            case 4: return "進修學士班";
            default: return "N/A";
        }
    }

    private string CollegeToString(int index) {

        switch (index) {
            case 1: return "資電學院";
            case 2: return "商學院";
            case 3: return "建設學院";
            case 4: return "金融學院";
            default: return "N/A";
        }

    }

    private string DepartmentToString(int CollegeIndex, int index) {
        if(CollegeIndex == 1)
            switch (index) {
                case 1: return "資訊系";
                case 2: return "電機系";
                case 3: return "電子系";
                case 4: return "通訊系";
                default: return "N/A";
            }

        if(CollegeIndex == 2)
            switch (index) {
                case 1: return "會計系";
                case 2: return "企管系";
                case 3: return "財稅系";
                case 4: return "經濟系";
                default: return "N/A";
            }

        return "N/A";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="CollegeIndex">學院</param>
    /// <param name="index">系</param>
    /// <returns></returns>
    private string ClassToString(int CollegeIndex, int index) {
        if (CollegeIndex == 1)  //資電學院
            switch (index) {
                case 1: return "資訊一甲";
                case 2: return "資訊一乙";
                case 3: return "資訊二甲";
                case 4: return "資訊二乙";
                case 5: return "資訊二丙";
                case 6: return "資訊二丁";
                default: return "N/A";
            }

        if (CollegeIndex == 2)
            switch (index) {
                case 1: return "會計一甲";
                case 2: return "會計一乙";
                case 3: return "會計二甲";
                case 4: return "會計二乙";
                default: return "N/A";
            }

        if (CollegeIndex == 3)
            switch (index) {
                case 1: return "都資一甲";
                case 2: return "都資一乙";
                case 3: return "都資二甲";
                case 4: return "都資二乙";
                default: return "N/A";
            }

        if (CollegeIndex == 4)
            switch (index) {
                case 1: return "財金一甲";
                case 2: return "財金一乙";
                case 3: return "財金二甲";
                case 4: return "財金二乙";
                default: return "N/A";
            }

        return "N/A";
    }
}
