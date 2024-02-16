using System;
using TMPro;
using UniRx;
using UnityEngine;


public abstract class TargetBase : MonoBehaviour
{
    public ReadOnlyReactiveProperty<bool> IsAchieved => isAchieved.ToReadOnlyReactiveProperty();
    public abstract string TargetName { get; }
    public TextMeshProUGUI Text { get; set; }
    protected ReactiveProperty<bool> isAchieved { get; private set; } = new();
}
