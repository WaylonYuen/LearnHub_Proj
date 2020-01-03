using UnityEngine;
using UnityEngine.EventSystems;

public enum DragAxisOptions { Both, Horizontal, Vertical }  //菜單

public class Drag : MonoBehaviour, IPointerDownHandler, IDragHandler {

    #region 屬性接口
    public DragAxisOptions DragAxisOptions { get { return DragAxisOptions; } set { dragAxisOptions = value; } }
    public bool LockX { get { return lockX; } set { lockX = value; } }
    public bool LockY { get { return lockY; } set { lockY = value; } }
    public float Horizontal { get { return dragBoxPosition.x; } }
    public float Vertical { get { return dragBoxPosition.y; } }
    public Vector2 Direction { get { return new Vector2(Horizontal, Vertical); } }
    public Vector2 DragRange { get { return dragRange; } }
    public Vector2 BackgroundSize { get { return backgroundOfSize; } set { backgroundOfSize = value; } }
    public Vector2 DragBoxSize { get { return dragBoxOfSize; } set { dragBoxOfSize = value; } }
    public Vector2 TouchPoint_First { get { return touchPoint_First; } }
    #endregion

    #region 參數
    [Header("DragBox Setting")]
    [SerializeField] Vector2 dragRangeOffset;                                           //若screen隨比例而無法得到確切大小,則需要手動調節偏差值
    [SerializeField] private DragAxisOptions dragAxisOptions = DragAxisOptions.Both;    //拖拽軸選項    
    [SerializeField] private bool lockX = false;                                        //x軸鎖定
    [SerializeField] private bool lockY = false;                                        //y軸鎖定

    private Vector2 dragRange;                                                          //拖拽範圍
    private Vector2 distanceOfDrag;                                                     //觸控後拖拽距離
    private Vector2 touchPoint_First;                                                   //第一個觸控座標
    private Vector2 dragBoxPosition;                                                    //拖拽物件起始座標
    private Vector2 backgroundOfSize;                                                   //父類物件尺寸
    private Vector2 dragBoxOfSize;                                                      //物件尺寸

    [Header("Object Setting")]
    [SerializeField] private RectTransform background = null;           //顯示窗口
    [SerializeField] private RectTransform swipeBox = null;             //需拖拽的物件(外部接口)
    #endregion

    /// <summary>
    /// 初始化預設值(必須在Start前,避免外部讀取到空值)
    /// </summary>
    protected virtual void OnEnable() {
        backgroundOfSize = background.sizeDelta;
        dragBoxOfSize = swipeBox.sizeDelta;
    }

    #region 方法
    /// <summary>
    /// 設定初始參數
    /// </summary>
    protected virtual void Start() {
        swipeBox.anchoredPosition = Vector2.zero;
        SetSwipeRange();
    }

    /// <summary>
    /// 按下時執行
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerDown(PointerEventData eventData) {
        touchPoint_First = eventData.position;          //紀錄點下的位置
        dragBoxPosition = swipeBox.anchoredPosition;    //紀錄當前座標位置
    }

    /// <summary>
    /// 拖拽時執行(拖拽會打斷長按)
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnDrag(PointerEventData eventData) {
        distanceOfDrag = eventData.position - touchPoint_First; //計算拖拽距離
        distanceOfDrag += dragBoxPosition;  //Offset DragBox起始位置(即:偏差值)
        CheckSwipeRange();
        swipeBox.anchoredPosition = distanceOfDrag; //輸出座標
    }
    #endregion

    #region 其他功能
    /// <summary>
    /// 拖拽範圍初始化設定
    /// </summary>
    public virtual void SetSwipeRange() {
        backgroundOfSize += dragRangeOffset;    //加上偏差值
        dragRange = (dragBoxOfSize - backgroundOfSize) / 2;  //計算窗口大小(假設锚點置中)

        //軸向鎖定
        lockX |= dragRange.x < 0;
        lockY |= dragRange.y < 0;
        lockY |= dragAxisOptions == DragAxisOptions.Horizontal;
        lockX |= dragAxisOptions == DragAxisOptions.Vertical;
    }

    /// <summary>
    /// 檢查並設置拖拽滑動範圍
    /// </summary>
    public virtual void CheckSwipeRange() {

        //檢查x軸是否可以滑動
        if (!lockX) {
            if (Mathf.Abs(distanceOfDrag.x) > Mathf.Abs(dragRange.x)) { //檢查x軸滑動是否超出許可範圍
                if (distanceOfDrag.x < 0)
                    distanceOfDrag.x = dragRange.x * -1;    //左側限制
                else
                    distanceOfDrag.x = dragRange.x;         //右側限制
            }
        } else
            distanceOfDrag.x = dragBoxPosition.x;   //鎖定值

        //檢查y軸是否可以滑動
        if (!lockY) {
            if (Mathf.Abs(distanceOfDrag.y) > Mathf.Abs(dragRange.y)) { //檢查y軸滑動是否超出許可範圍
                if (distanceOfDrag.y < 0)
                    distanceOfDrag.y = dragRange.y * -1;    //下方限制
                else
                    distanceOfDrag.y = dragRange.y;         //上方限制
            }
        } else
            distanceOfDrag.y = dragBoxPosition.y;   //鎖定值
    }
    #endregion
}
