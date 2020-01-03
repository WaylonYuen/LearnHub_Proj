using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Degree_Dropdown : MonoBehaviour, IPointerClickHandler {

    public static int dropdownIndex { get; private set; }

    private List<string> names = new List<string>() { "請選擇部門", "大學部", "碩士班", "博士班", "進修學士班" };

    public Dropdown dropdown;
    public Text selectedName;

    private int GetIndex;

    public void Dropdown_IndexChanged(int index) {
        selectedName.text = names[index];
        GetIndex = index;
        dropdownIndex = index;
    }

    private void Start() {
        dropdownIndex = 0;
        PopulateList();
    }

    public void PopulateList() {
        dropdown.AddOptions(names);
    }

    public void OnPointerClick(PointerEventData eventData) {
        dropdownIndex = GetIndex;
    }
}
