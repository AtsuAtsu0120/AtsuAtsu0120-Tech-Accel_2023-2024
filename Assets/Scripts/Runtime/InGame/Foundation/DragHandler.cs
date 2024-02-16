#if PLATFORM_IOS || PLATFORM_ANDROID || PLATFORM_WEBGL
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class DragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public abstract void OnDrag(PointerEventData eventData);
    public abstract void OnBeginDrag(PointerEventData eventData);
    public abstract void OnEndDrag(PointerEventData eventData);
}
#endif
