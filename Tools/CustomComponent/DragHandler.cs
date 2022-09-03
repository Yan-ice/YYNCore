using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
[AddComponentMenu("CustomUI/LongPressButton")]
#endif
public class DragHandler : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    public int keep_msec = 1000;

    private Image progress;

    public UnityEvent onLongPress = new UnityEvent();

    private DateTime pressTime;
    private bool clicking = false;

    private void LongPress()
    {
        if (onLongPress != null)
            //调用所有注册的事件
            onLongPress.Invoke();
        ResetTime();
    }
    //重置时间
    private void ResetTime()
    {
        pressTime = default(DateTime);
        progress.fillAmount = 0;
    }

    private void Start()
    {
        progress = transform.Find("Progress").GetComponent<Image>();
        progress.fillAmount = 0;
    }
    private void Update()
    {
        if(clicking)
        {
            DateTime upTime = DateTime.Now;
            int keep_mili = (upTime - pressTime).Seconds*1000 + (upTime - pressTime).Milliseconds;
            if (keep_mili > keep_msec+300)
            {
                clicking = false;
                LongPress();
                ResetTime();
            }else if(keep_mili>250){
                progress.fillAmount = (keep_mili - 250) / (float)keep_msec;
            }
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        pressTime = DateTime.Now;
        clicking = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        clicking = false;
        ResetTime();
    }
}
