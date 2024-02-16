using System.Collections.Generic;

public interface IStage
{
    public void OnComplete();
    public bool IsComplete { get; }
    public IReadOnlyList<TargetBase> Targets { get; }
}
