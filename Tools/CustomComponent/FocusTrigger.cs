using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

    public class FocusTrigger : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public UnityEvent onFocus = new UnityEvent();
        public UnityEvent onExit = new UnityEvent();
        public UnityEvent onUp = new UnityEvent();

        public void OnPointerEnter(PointerEventData eventData)
        {
            onFocus.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onExit.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onFocus.Invoke();
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            onUp.Invoke();
        }
}


