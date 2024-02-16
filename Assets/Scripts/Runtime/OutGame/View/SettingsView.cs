using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UniRx;

public class SettingsView : UIManagerBase
{
    public float MasterVolume
    {
        get => _masterSlider.value;
        set => _masterSlider.value = value;
    }

    public float SEVolume
    {
        get => _seSlider.value;
        set => _seSlider.value = value;
    }

    public float BGMVolume
    {
        get => _bgmSlider.value;
        set => _bgmSlider.value = value;
    }

    public IObservable<float> OnMasterSliderChanged => _masterSlider.onValueChanged.AsObservable();
    public IObservable<float> OnSESliderChanged => _seSlider.onValueChanged.AsObservable();
    public IObservable<float> OnBGMSliderChanged => _bgmSlider.onValueChanged.AsObservable();
    public IObservable<(float, float, float)> OnDecisionEvent => _onDecision;
    public IObservable<Unit> OnCancelEvent => _onCancel;
    
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _seSlider;
    [SerializeField] private Slider _bgmSlider;

    private Subject<(float, float, float)> _onDecision = new();
    private Subject<Unit> _onCancel = new();
    
    public void ShowSettings()
    {
        SetViewActive(!IsActiveSelf);
        
    }

    public void OnDecision()
    {
        _onDecision.OnNext((MasterVolume, SEVolume, BGMVolume));
        
        SetViewActive(false);
    }

    public void OnCancel()
    {
        _onCancel.OnNext(Unit.Default);
        
        SetViewActive(false);
    }
}
