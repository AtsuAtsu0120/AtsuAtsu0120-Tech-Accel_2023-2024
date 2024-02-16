using System;
using UniRx;
using UnityEngine;

public abstract class EnterBase : MonoBehaviour
{
    protected IObservable<Unit> OnEnter => _onEnter;
    private Subject<Unit> _onEnter = new();

    public void OnTriggerEnter(Collider other)
    {
        _onEnter.OnNext(Unit.Default);
    }
}
