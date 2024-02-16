using System;
using UniRx;

public class EventManager : IDisposable
{
    public bool ShouldDecreaseConfirmPercent { get; set; }
    public IObservable<Unit> OnDecreaseConfirmPercent => _onDecreaseConfirmPercent;
    private Subject<Unit> _onDecreaseConfirmPercent = new();
    private IDisposable _updateDisposable;
    public EventManager()
    {
        _updateDisposable = Observable.EveryUpdate().Where(_ => ShouldDecreaseConfirmPercent)
            .Subscribe(_ => _onDecreaseConfirmPercent.OnNext(Unit.Default));
    }

    public void Dispose()
    {
        _onDecreaseConfirmPercent?.Dispose();
        _updateDisposable?.Dispose();
    }
}