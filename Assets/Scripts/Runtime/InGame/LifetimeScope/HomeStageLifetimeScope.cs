using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class HomeStageLifetimeScope : LifetimeScope
{
    [SerializeField] private List<TargetBase> _targets;
    [SerializeField] private MinimapManager _minimapManager;
    [SerializeField] private IndicatorManager _indicatorManager;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<InGameManager>(Lifetime.Scoped);
        builder.Register<IStage, DebugStage>(Lifetime.Scoped);
        builder.RegisterComponent(_targets);
        builder.RegisterComponent(_indicatorManager);
        builder.RegisterComponent(_minimapManager);
    }
}
