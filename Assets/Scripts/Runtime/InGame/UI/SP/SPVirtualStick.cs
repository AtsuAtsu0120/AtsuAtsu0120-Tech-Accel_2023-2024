using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SPVirtualStick : DragHandler
{
    [SerializeField] private Image _background;
    [SerializeField] private Image _joystick;

    [SerializeField] private float _joystickSensitivity;
    public Vector2 MoveInput { get; private set; }
    private float _x;
    private float _y;
    private const float JoystickRange = 1.5f;
    

    private void Start()
    {
        MoveInput = Vector2.zero;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        var pos = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_background.rectTransform, eventData.position, eventData.pressEventCamera, out pos))
        {
            var sizeDelta = _background.rectTransform.sizeDelta / JoystickRange;
            pos.x /= sizeDelta.x;
            pos.y /= sizeDelta.y;

            MoveInput = pos * _joystickSensitivity;
            if (MoveInput.sqrMagnitude > 1) {
                MoveInput = MoveInput.normalized;
            }
            
            _joystick.rectTransform.anchoredPosition = 
                new(MoveInput.x * (sizeDelta.x)
                    , MoveInput.y * (sizeDelta.y));
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        MoveInput = Vector2.zero;
        _joystick.rectTransform.anchoredPosition = Vector2.zero;
    }
}
