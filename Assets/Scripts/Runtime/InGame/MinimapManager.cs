using UniRx;
using UnityEngine;

public class MinimapManager : MonoBehaviour
{
    [SerializeField] private Camera _minimapCamera;

    public MinimapMode NowMinimapMode
    {
        get => _minimapMode.Value;
        set => _minimapMode.Value = value;
    }

    private ReactiveProperty<MinimapMode> _minimapMode = new();

    private void Start()
    {
        _minimapMode.Subscribe(ChangeMinimap);
    }

    /// <summary>
    /// ミニマップの表示を変える
    /// </summary>
    /// <param name="mode">どこに到達したか。</param>
    private void ChangeMinimap(MinimapMode mode)
    {
        var layerMask = mode switch
        {
            MinimapMode.Outside => 10,
            MinimapMode.FirstFloor => 7,
            MinimapMode.SecondFloor => 8,
            _ => 10
        };
        _minimapCamera.cullingMask = 1 << layerMask | 1 << 9;
    }
}
public enum MinimapMode
{
    Outside,
    FirstFloor,
    SecondFloor
}