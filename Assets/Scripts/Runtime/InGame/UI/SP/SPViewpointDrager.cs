#if PLATFORM_IOS || PLATFORM_ANDROID || PLATFORM_WEBGL
using InGame.Base;
using UnityEngine;
using UnityEngine.EventSystems;

public class SPViewpointDrager : DragHandler
{
    [SerializeField] private Canvas _canvas;

    public Vector2 LookInput { get; private set; }
    private Vector2 _lookPointerPosPre;
    
    public override void OnDrag(PointerEventData eventData)
    {
        var pointerPos = GetPositionOnCanvas(eventData.position);
        
        // 最初の位置と比較
        LookInput = pointerPos - _lookPointerPosPre;
        
        _lookPointerPosPre = pointerPos;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        _lookPointerPosPre = GetPositionOnCanvas(eventData.position);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        LookInput = Vector2.zero;
    }

    private Vector2 GetPositionOnCanvas(Vector2 pointerPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            pointerPos,
            _canvas.worldCamera,
            out var localPointerPos
        );
        return localPointerPos;
    }
}
#endif
