using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    #region 安全接口

    #endregion

    #region 預設參數
    [SerializeField] protected RectTransform background = null;
    private Image image;
    #endregion


    protected virtual void Start() {
        image = GetComponent<Image>();
    }

    public virtual void OnPointerDown(PointerEventData eventData) {
        background.localScale = new Vector3(0.9f, 0.9f, 1);
        image.color = new Color(170f / 255f, 170f / 255f, 170f / 255f);
    }

    public virtual void OnPointerUp(PointerEventData eventData) {
        background.localScale = new Vector3(1, 1, 1);
        image.color = new Color(1f, 1f, 1f);
    }

}
