using System;

using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.UI;

public class RectSelecter : UIWindow
{
    #region 脚本工具生成的代码
    private RectSelecterMono m_mono;
    private RectTransform m_rectLighter;
    private Button m_btnBlocker;
    private Button m_btnSure;
    private Button m_btnCancel;
    /*该方法会在初始化前被UIWindow自动调用。*/
    protected override void ComponentInit()
    {
        m_rectLighter = FindChildComponent<RectTransform>("m_rectLighter");
        m_btnBlocker = FindChildComponent<Button>("m_btnBlocker");
        m_btnSure = FindChildComponent<Button>("m_btnSure");
        m_btnCancel = FindChildComponent<Button>("m_btnCancel");
        m_btnBlocker.onClick.AddListener(OnClickBlockerBtn);
        m_btnSure.onClick.AddListener(OnClickSureBtn);
        m_btnCancel.onClick.AddListener(OnClickCancelBtn);
    }
    #endregion

    #region 事件
    private void OnClickBlockerBtn()
    {

    }
    private void OnClickSureBtn()
    {
        m_mono.Copy();
    }
    private void OnClickCancelBtn()
    {
        DestroyImmediate();
    }
    #endregion



    protected override void OnDestroy()
    {
        
    }

    protected override void OnInit(GameObject m_go)
    {
        m_mono = FindChildComponent<RectSelecterMono>("m_btnBlocker");
        m_mono.lighter = m_rectLighter;
    }

    protected override int SortOrder()
    {
        return 9999;
    }
}


[ExecuteAlways]
public class RectSelecterMono : MonoBehaviour, IPointerDownHandler, IEndDragHandler, IDragHandler, IBeginDragHandler
{
    public RectTransform lighter;

    Vector2 begin = Vector2.zero;
    Vector2 end = Vector2.zero;

    void OnAwake()
    {
        this.gameObject.SetActive(false);
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        //begin = UIManager.ScreenToCanvas(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        end = UIManager.ScreenToCanvas(eventData.position);
        float l_x = Math.Min(begin.x, end.x);
        float l_y = Math.Min(begin.y, end.y);
        float m_x = Math.Max(begin.x, end.x);
        float m_y = Math.Max(begin.y, end.y);

        lighter.localPosition = new Vector2(l_x,m_y);
        lighter.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_x-l_x);
        lighter.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_y-l_y);
        
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        end = Vector2.zero;

    }

    public void Copy()
    {
        TextEditor te = new TextEditor();
        if (begin == end)
        {
            te.text = string.Format("{0},{1}", (int)lighter.localPosition.x, (int)lighter.localPosition.y);
            te.SelectAll();
        }
        else
        {
            te.text = string.Format("{0},{1},{2},{3}", (int)lighter.localPosition.x, (int)(lighter.localPosition.x + lighter.rect.width),
                (int)(lighter.localPosition.y - lighter.rect.height), (int)(lighter.localPosition.y));
            te.SelectAll();
        }
        te.Copy();
        lighter.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        lighter.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        begin = UIManager.ScreenToCanvas(eventData.position);
        end = UIManager.ScreenToCanvas(eventData.position);
        lighter.localPosition = begin;
        lighter.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 50);
        lighter.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50);

    }
    
}
