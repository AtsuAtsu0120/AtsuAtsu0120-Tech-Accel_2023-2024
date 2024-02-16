using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class DebugStageLigetimeScope : LifetimeScope
{
    [SerializeField] private List<TargetBase> _targets;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<InGameManager>(Lifetime.Scoped);
        builder.Register<IStage, DebugStage>(Lifetime.Scoped);
        builder.RegisterComponent(_targets);
    }
}
