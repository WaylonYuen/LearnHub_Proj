using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public enum TouchEvent { Click, DoubleClick, Drag, LongPress }  //菜單

public class Touch : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {

    #region 屬性接口
    public bool IsPress { get; private set; }                   //檢查是否按著
    public float TouchTime_First { get; private set; }          //紀錄第一下點擊的時間
    public float TouchTime_Second { get; private set; }         //紀錄第二下點擊的時間
    #endregion

    #region 參數
    [SerializeField] private float pressTime;                                   //計算按下的持續時間
    [SerializeField] [Range(0, 1)] private float doubleClickOffset = 0.2f;      //雙擊的響應時間
    [SerializeField] [Range(0, 3)] private float longPressOffset = 1f;          //長按的響應時間
    
    private TouchEvent touchEvent;          //觸控方式(紀錄是哪種類型的觸控操作)
    private bool EventFlag;                 //取得觸控方式(獲得觸控類型,可確認最終操作)
    private bool EventLock;                 //觸控方式鎖(點擊時解鎖,當確認為某種觸控類型立即上鎖,防止觸控訊息被污染)
    #endregion

    /// <summary>
    /// 設定初始值
    /// </summary>
    protected virtual void Start() {
        IsPress = false;
        EventLock = true;
        EventFlag = false;
    }

    /// <summary>
    /// 按下時執行
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerDown(PointerEventData eventData) {
        EventLock = false;      //解鎖
        IsPress = true; 
        CheckDoubleClick();     //檢查是否雙擊
    }

    /// <summary>
    /// 拖拽時執行(拖拽會打斷長按)
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnDrag(PointerEventData eventData) {
        if (!EventLock)
            GetTouchEvent = TouchEvent.Drag;
    }

    /// <summary>
    /// 放開時執行
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerUp(PointerEventData eventData) {
        IsPress = false;
    }

    /// <summary>
    /// 持續偵測
    /// </summary>
    protected virtual void Update() {

        if (EventFlag) {        //檢查觸控行為是否已確認
            EventFlag = false;
            Event();
        } else if (!EventLock) {
            if (IsPress)
                StartCoroutine(CheckLongPress());
            else
                StartCoroutine(CheckClick());
        }

    }

    /// <summary>
    /// 單擊判定
    /// </summary>
    private IEnumerator CheckClick() {
        float Now = Time.realtimeSinceStartup;
        if (Now - TouchTime_First >= doubleClickOffset)
            if (!EventLock)
                GetTouchEvent = TouchEvent.Click;

        yield return null;
    }

    /// <summary>
    /// 雙擊判定
    /// </summary>
    private void CheckDoubleClick() {
        TouchTime_Second = Time.realtimeSinceStartup;  //紀錄點擊時間

        if (TouchTime_Second - TouchTime_First < doubleClickOffset)//點擊時間差若小於設定值,則判定為雙擊
            if (!EventLock)
                GetTouchEvent = TouchEvent.DoubleClick;

        TouchTime_First = TouchTime_Second;
    }

    /// <summary>
    /// 長按判定(需在Update中執行方法)
    /// </summary>
    private IEnumerator CheckLongPress() {
        float Now = Time.realtimeSinceStartup;  //紀錄目前時間
        pressTime = Now - TouchTime_First;      //計算持續按下的時間

        if (pressTime > longPressOffset) 
            if (!EventLock) 
                GetTouchEvent = TouchEvent.LongPress;

        yield return null;
    }

    /// <summary>
    /// 獲取觸控事件
    /// </summary>
    public TouchEvent GetTouchEvent {
        get {
            return touchEvent;
        }
        private set {
            EventLock = true;   //上鎖
            EventFlag = true;   //已取得觸控類型
            touchEvent = value; //賦值(觸控類型)
        }
    }

    /// <summary>
    /// 觸控事件處理
    /// </summary>
    public virtual void Event() {
        TouchEvent touch = GetTouchEvent;
        Debug.Log(touch);
    }
}





//以下程式需開新檔案(暫存於此)

//接口
public interface ICoordinateTransformation {
    Canvas Canvas { get; set; }             //主畫布
    Camera Cam { get; set; }                //相機
    RectTransform BaseRect { get; set; }    //轉換目標的基底
    Vector2 RectOffset { get; set; }        //參考位偏差值
    OffsetDirection OffsetDirection { get; set; }
    Vector2 CoorTransformation(Vector2 screenPosition);
}

/// <summary>
/// 座標轉換Instance
/// </summary>
public class CoordinateTransformation {

    ICoordinateTransformation TransformationTo;

    //讀取轉換類
    public CoordinateTransformation(ICoordinateTransformation TransformationTo) {
        this.TransformationTo = TransformationTo;
    }

    //執行
    public Vector2 Execute(Vector2 screenPosition) {
        return TransformationTo.CoorTransformation(screenPosition);
    }

}

//實現屏幕座標轉換成父類Rect座標
public class ScreenPointToAnchoredPosition : ICoordinateTransformation {
    private Camera cam;
    private Canvas canvas;
    private RectTransform baseRect;
    OffsetDirection offsetDirection;
    Vector2 rectOffset;
    

    //Instance
    public ScreenPointToAnchoredPosition(Camera cam, Canvas canvas, RectTransform baseRect, Vector2 rectOffset, OffsetDirection offsetDirection) {
        this.cam = cam;
        this.canvas = canvas;
        this.baseRect = baseRect;
        this.rectOffset = rectOffset;
        this.offsetDirection = offsetDirection;
    }

    Camera ICoordinateTransformation.Cam { get { return cam; } set { cam = value; } }
    Canvas ICoordinateTransformation.Canvas { get { return canvas; } set { canvas = value; } }
    RectTransform ICoordinateTransformation.BaseRect { get { return baseRect; } set { baseRect = value; } }
    OffsetDirection ICoordinateTransformation.OffsetDirection { get { return offsetDirection; } set { offsetDirection = value; } }
    Vector2 ICoordinateTransformation.RectOffset { get { return rectOffset; } set { rectOffset = value; } }

    //轉換方法
    Vector2 ICoordinateTransformation.CoorTransformation(Vector2 screenPosition) {
        Vector2 localPoint = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, screenPosition, cam, out localPoint))
            return localPoint += OffsetDirection();    //計算偏差值
        return Vector2.zero;
    }

    //偏差位
    public Vector2 OffsetDirection() {
        switch (offsetDirection) {
            case global::OffsetDirection.None:      return Vector2.zero;
            case global::OffsetDirection.Bottom:    return new Vector2(rectOffset.x/2f, 0f);
            default:                                return Vector2.zero;
        }
    }

}

//參考方位
public enum OffsetDirection {
    None,
    LeftUp,
    Up,
    RightUp,
    Left,
    Mid,
    Right,
    LeftBottom,
    Bottom,
    RightBottom,
}