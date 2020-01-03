using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.DataBuffer;

public class CourseSelectController : MonoBehaviour {

    private void Awake() {
        DataList.CourseCart = new int[50];
        DataList.SearchCourseTemp = new Data.DataBuffer.CourseData[50];
    }

    private void Start() {
        ClearCourseCart();
    }

    public static void ClearCourseCart() {
        for (int i = 0; i < DataList.CourseCart.Length; i++) {
            DataList.CourseCart[i] = 0;
        }
    }

}
