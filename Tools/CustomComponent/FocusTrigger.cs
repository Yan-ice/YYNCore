using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

    public class FocusTrigger : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        public UnityEvent onFocus = new UnityEvent();
        public UnityEvent onExit = new UnityEvent();

        public void OnPointerEnter(PointerEventData eventData)
        {
            onFocus.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onExit.Invoke();
        }

    }


