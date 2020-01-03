using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BtnSettingMenu : Button, IPointerDownHandler, IPointerUpHandler {

    [SerializeField] private RectTransform Menu = null;

    private bool IsOpen = false;

    protected override void Start() {
        Menu.gameObject.SetActive(false);
        base.Start();
    }

    public override void OnPointerDown(PointerEventData eventData) {

        if (!IsOpen) {
            Menu.gameObject.SetActive(true);
            IsOpen = true;
        } else { 
            Menu.gameObject.SetActive(false);
            IsOpen = false;
        }

        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData) {
        base.OnPointerUp(eventData);
    }

}
