using System.Collections.Generic;
using UnityEngine;
using VContainer;
using UniRx;

public class DebugStage : IStage
{
    public bool IsComplete => !_targets.Exists(v => !v.IsAchieved.Value);
    public IReadOnlyList<TargetBase> Targets => _targets;
    private List<TargetBase> _targets;
    
    [Inject]
    public DebugStage(List<TargetBase> targets)
    {
        _targets = targets;
        
        // ターゲットがそれぞれに設定されている目標を達成したら発火するイベントを登録。
        foreach (var target in _targets)
        {
            target.IsAchieved.Where(_ => IsComplete).Subscribe(_ => OnComplete());
        }
    }
    
    /// <summary>
    /// クリアしたときのメソッド
    /// </summary>
    /// <returns></returns>
    public void OnComplete()
    {
        Debug.Log($"クリア!");
    }
}
