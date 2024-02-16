using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using VContainer;

public class HomeStage : IStage
{
    public bool IsComplete => !_targets.Exists(v => !v.IsAchieved.Value);
    public IReadOnlyList<TargetBase> Targets => _targets;
    private List<TargetBase> _targets;
    
    public void OnComplete()
    {
        
    }
    [Inject]
    public HomeStage(List<TargetBase> targets)
    {
        _targets = targets;
        
        // ターゲットがそれぞれに設定されている目標を達成したら発火するイベントを登録。
        foreach (var target in _targets)
        {
            target.IsAchieved.Where(_ => IsComplete).Subscribe(_ => OnComplete());
        }
    }
}
