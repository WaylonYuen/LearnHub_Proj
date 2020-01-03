using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 動態控制桿
/// </summary>
public class DynamicJoystick : Joystick {
    public float MoveThreshold { get { return moveThreshold; } set { moveThreshold = Mathf.Abs(value); } }

    [SerializeField] private float moveThreshold = 1;

    protected override void Start() {
        MoveThreshold = moveThreshold;
        base.Start();
        background.gameObject.SetActive(true);
    }

    public override void OnPointerDown(PointerEventData eventData) {
        //background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);    //紀錄點擊時操縱桿的世界座標
        background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData) {
        background.gameObject.SetActive(true);
        base.OnPointerUp(eventData);
    }

    //允許浮動
    protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam) {
        if (magnitude > moveThreshold) {
            Vector2 difference = normalised * (magnitude - moveThreshold) * radius;
            background.anchoredPosition += difference;
        }
        base.HandleInput(magnitude, normalised, radius, cam);
    }
}