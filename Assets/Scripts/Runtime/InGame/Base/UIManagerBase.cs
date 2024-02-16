using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIManagerBase : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    
    public bool IsActiveSelf { get; private set; }

    public void SetViewActive(bool isActive)
    {
        _canvasGroup.alpha = isActive ? 1 : 0;
        _canvasGroup.blocksRaycasts = isActive;

        IsActiveSelf = isActive;
    }
    
    #if UNITY_EDITOR

    private void Reset()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    #endif
}
