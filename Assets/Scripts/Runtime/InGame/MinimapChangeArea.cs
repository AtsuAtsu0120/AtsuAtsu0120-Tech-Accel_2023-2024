using UniRx;
using UnityEngine;
using VContainer;

public class MinimapChangeArea : EnterBase
{
    [SerializeField] private MinimapManager _minimapManager;
    [SerializeField] private MinimapMode _changeModeA;
    [SerializeField] private MinimapMode _changeModeB;

    [Inject]
    public void Constructor(MinimapManager minimapManager)
    {
        _minimapManager = minimapManager;
    }
    public void Start()
    {
        OnEnter.Subscribe(_ => OnEnterEvent()).AddTo(this);
    }

    private void OnEnterEvent()
    {
        _minimapManager.NowMinimapMode = _minimapManager.NowMinimapMode != _changeModeA 
            ? _changeModeA : _changeModeB;
    }
}
