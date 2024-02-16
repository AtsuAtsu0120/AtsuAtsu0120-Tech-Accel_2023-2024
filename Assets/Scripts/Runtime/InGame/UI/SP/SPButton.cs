using System;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SPButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private UnityEvent _pointerDown;
    [SerializeField] private UnityEvent _pointerUp;

    /// <summary>
    /// 押された時 = true
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        _pointerDown.Invoke();
    }
    
    /// <summary>
    /// 離したとき = false
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        _pointerUp.Invoke();
    }
}
