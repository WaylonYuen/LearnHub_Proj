using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Touch_Inventory : Touch {

    [SerializeField] private Camera cam;                //相機
    [SerializeField] private Canvas canvas;             //主畫布
    [SerializeField] private RectTransform baseRect;    //基底(用於座標轉換,將screen座標 轉換 成基底座標) = swipeBox

    [SerializeField] private RectTransform SelectBox;      //選擇框
    [SerializeField] private RectTransform[] iterm;        //保存所有可觸控物件(即選擇欄)

    private Vector2 touchPoint;
    private float itermOffset = 10f;                    //子物件偏差值
    private float itermSideSize = 180f;                 //子物件邊長
    private float TotalOffset;                          //總偏差值

    public static int itermNum;                         //保存點擊的iterm

    CoordinateTransformation screenPointToAncPos;

    protected override void Start() {
        base.Start();

        SelectBox.anchoredPosition = new Vector2(1530f, 10f);   //預設選擇框位置

        Vector2 rectOffset = baseRect.sizeDelta;    //取得目標尺寸(以便計算偏差值)
        screenPointToAncPos = new CoordinateTransformation(new ScreenPointToAnchoredPosition(cam, canvas, baseRect, rectOffset, OffsetDirection.Bottom));   //座標轉換實例

        TotalOffset = itermOffset + itermSideSize;  //計算每個物件相對於前一個物件的座標偏差值
    }

    public override void OnPointerDown(PointerEventData eventData) {
        base.OnPointerDown(eventData);
        touchPoint = screenPointToAncPos.Execute(eventData.position);   //取得在baseRect平面上的座標
    }

    /// <summary>
    /// 重寫觸控事件
    /// </summary>
    public override void Event() {

        //單擊
        if(GetTouchEvent == TouchEvent.Click) {
            CheckItermNum();    //計算點擊的位置上是幾號子物件
            SelectBox.anchoredPosition = iterm[itermNum].anchoredPosition;      //更新選擇框的位置
        }

        //雙擊
        if(GetTouchEvent == TouchEvent.DoubleClick) {
            CheckItermNum();    //計算點擊的位置上是幾號子Box
            SelectBox.anchoredPosition = iterm[itermNum].anchoredPosition;      //更新選擇框的位置

            //不為0則表示有東西
            //if (GameDataList.InventoryData.ItermBoxObj[itermNum] != null && GameDataList.InventoryData.NumberOfIterm[itermNum] > 0) {
            //    GameDataList.InventoryData.ItermBoxObj[itermNum].Use();
            //    GameDataList.InventoryData.NumberOfIterm[itermNum]--;
            //}
        }
    }

    private void CheckItermNum() {
        itermNum = (int)(touchPoint.x / TotalOffset);   //取商
        float Remainder = touchPoint.x % TotalOffset;   //取模
        if (Remainder > 0)
            itermNum++;

        itermNum--;
        Debug.Log(itermNum);
    }
}
