using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//操縱桿
public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {

    #region 父類接口
    /*直線操縱桿*/
    public float Horizontal { get { return (snapX) ? SnapFloat(input.x, AxisOptions.Horizontal) : input.x; } }  //水平型 - 操縱桿
    public float Vertical { get { return (snapY) ? SnapFloat(input.y, AxisOptions.Vertical) : input.y; } }      //垂直型 - 操縱桿
    public Vector2 Direction { get { return new Vector2(Horizontal, Vertical); } }  //當前操縱桿的V2方向

    /*圓形操縱桿*/
    public float HandleRange { get { return handleRange; } set { handleRange = Mathf.Abs(value); } }    //設置操縱桿可以動範圍（以中心計算）
    public float DeadZone { get { return deadZone; } set { deadZone = Mathf.Abs(value); } }             //設置操縱桿移動部件最大出界範圍（0為移動部件的中心為出界極限）
 
    public AxisOptions AxisOptions { get { return AxisOptions; } set { axisOptions = value; } } //設置操縱桿類型
    public bool SnapX { get { return snapX; } set { snapX = value; } }  //水平對齊
    public bool SnapY { get { return snapY; } set { snapY = value; } }  //垂直對齊

    public bool IsControl { get; set; }   //是否有操控
    #endregion

    #region 預設參數
    //參數預設
    [SerializeField] private float handleRange = 1f;
    [SerializeField] private float deadZone = 0f;
    [SerializeField] private AxisOptions axisOptions = AxisOptions.Both;    //預設為圓形操縱桿
    [SerializeField] private bool snapX = false;
    [SerializeField] private bool snapY = false;

    //Graph預設
    [SerializeField] protected RectTransform background = null; //操縱桿邊界背景
    [SerializeField] private RectTransform handle = null;       //操縱桿可動背景
    private RectTransform baseRect = null;  //自己的Rect

    private Canvas canvas;
    private Camera cam;

    private Vector2 input = Vector2.zero;   //預設操縱桿方向為0
    #endregion

    #region 方法
    protected virtual void Start() {

        IsControl = false;

        //讀取外部設置的數值並保存到內部
        HandleRange = handleRange;
        DeadZone = deadZone;
        baseRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        //判斷是否掛上了父類操縱桿
        if (canvas == null)
            Debug.LogError("The Joystick is not placed inside a canvas");

        //操縱桿設置
        Vector2 center = new Vector2(0.5f, 0.5f);
        background.pivot = center;
        handle.anchorMin = center;
        handle.anchorMax = center;
        handle.pivot = center;
        handle.anchoredPosition = Vector2.zero;  
    }

    /// <summary>
    /// 按下控制桿時執行
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerDown(PointerEventData eventData) {
        IsControl = true;
        OnDrag(eventData);
    }

    /// <summary>
    /// 拖拽控制桿時執行
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData) {
        cam = null;
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            cam = canvas.worldCamera;

        Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);   //獲取觸控位置投射在操縱桿背景平面上的世界座標點
        Vector2 radius = background.sizeDelta / 2;  //計算操縱桿背景半徑(圓形操縱桿背景是由正方形框住的,所以正方形邊長的一半,就是其圓形的半徑)
        input = (eventData.position - position) / (radius * canvas.scaleFactor);    //計算 拖拽距離( (v操縱桿座標 - v操縱桿邊界背景座標)/(v半徑 * 比例) = 拖拽程度 ),其中v = v2向量
        FormatInput();  //檢查是否單軸控制(是則將其軸x的另一條軸y歸0,不納入計算即可實現單軸)
        HandleInput(input.magnitude, input.normalized, radius, cam);    //維持拖拽值在 0～1間
        handle.anchoredPosition = input * radius * handleRange; //操縱桿拖拽臨界點,超出則操縱桿背景追隨
    }

    /// <summary>
    /// 判斷拖拽值(即:判斷用戶是否有拖拽行為 0～1 )
    /// </summary>
    /// <param name="magnitude">量級</param>
    /// <param name="normalised">正常化</param>
    /// <param name="radius">半徑</param>
    /// <param name="cam"></param>
    protected virtual void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam) {
        if (magnitude > deadZone) { //判斷向量是否大於拖著啊臨界值(預設為0)
            if (magnitude > 1)
                input = normalised; //將向量規範成 0～1
        } else
            input = Vector2.zero;
    }

    /// <summary>
    /// 操縱桿樣式(若為直向操縱桿,則引用此Func)
    /// </summary>
    private void FormatInput() {
        //將雙軸向鎖定為單軸
        if (axisOptions == AxisOptions.Horizontal)
            input = new Vector2(input.x, 0f);
        else if (axisOptions == AxisOptions.Vertical)
            input = new Vector2(0f, input.y);
    }

    /// <summary>
    /// 斷裂浮動
    /// </summary>
    /// <param name="value"></param>
    /// <param name="snapAxis">斷裂浮動的軸</param>
    /// <returns></returns>
    private float SnapFloat(float value, AxisOptions snapAxis) {

        //如果輸入方向為0,則立刻返回(即操縱桿沒有被操控)
        if (value == 0)
            return value;

        if (axisOptions == AxisOptions.Both) {
            float angle = Vector2.Angle(input, Vector2.up); //角度計算（計算: 控制值{input(x,y)} 與 固定值{Vector(0,1)} 基於中心0的夾角）
            if (snapAxis == AxisOptions.Horizontal) //計算水平值
                return (angle < 22.5f || angle > 157.5f) ? 0 : ((value > 0) ? 1 : -1);      

            if (snapAxis == AxisOptions.Vertical)   //計算垂直值
                return (angle > 67.5f && angle < 112.5f) ? 0 : ((value > 0) ? 1 : -1);

            return value;
        }

        #region 方向值控制 (-1為反方向)
        if (value > 0)
            return 1;

        if (value < 0)
            return -1;
        #endregion

        return 0;
    }

    //放開點擊時,控制桿歸位
    public virtual void OnPointerUp(PointerEventData eventData) {

        IsControl = false;

        Network.Packet.Send send = new Network.Packet.Send();
        if(StartOnline.MyClient.Player.Ready)
            //send.VectorData(StartOnline.MyClient.Player, CustomEnumType.PackageType.SyncCharacterState, 0f, 0f, 0f);

        input = Vector2.zero;   //輸入值歸0
        handle.anchoredPosition = Vector2.zero; //操縱桿歸位
        background.position = new Vector2(256.0f, 256.0f + 240f);  //放手後操縱桿整體歸位
    }

    //座標轉換
    protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition) {
        Vector2 localPoint = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, screenPosition, cam, out localPoint)) {
            Vector2 pivotOffset = baseRect.pivot * baseRect.sizeDelta;
            return localPoint - (background.anchorMax * baseRect.sizeDelta) + pivotOffset;
        }
        return Vector2.zero;
    }
    #endregion
}

//菜單枚舉變量
public enum AxisOptions { Both, Horizontal, Vertical }