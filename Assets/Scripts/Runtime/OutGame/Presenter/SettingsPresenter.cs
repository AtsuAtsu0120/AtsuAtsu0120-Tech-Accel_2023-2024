using System;
using InGame;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsPresenter : MonoBehaviour
{
    public IObservable<(float, float, float)> OnResetVolumesEvent => _onResetVolumesEventEvent;

    public float MasterVolume => _model.MasterVolume;
    public float SEVolume => _model.SEVolume;
    public float BGMVolume => _model.BGMVolume;
    
    [SerializeField] private SettingsView _view;
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioSource _source;
    
    private Subject<(float, float, float)> _onResetVolumesEventEvent = new();
    private SettingsModel _model;
    
    private const string _masterVolumePropertyName = "MasterVolume";
    private const string _seVolumePropertyName = "SEVolume";
    private const string _bgmVolumePropertyName = "BGMVolume";

    private void Start()
    {
        _model = new();
        _model.OnCancelEvent.Subscribe(volumes => OnResetVolumes(volumes));
        
        InitView();
        InitUIInputAction();
        InitSliders();
        
        _view.OnDecisionEvent.Subscribe(value => 
            OnDecision(value.Item1, value.Item2, value.Item3));
        _view.OnCancelEvent.Subscribe(_ => OnCancel());
    }

    private void InitSliders()
    {
        _view.OnMasterSliderChanged.Subscribe(value => VolumeChange(_masterVolumePropertyName, value));
        _view.OnSESliderChanged.Subscribe(value => VolumeChange(_seVolumePropertyName, value));
        _view.OnBGMSliderChanged.Subscribe(value => VolumeChange(_bgmVolumePropertyName, value));
    }
    private void InitUIInputAction()
    {
        PlayerController.Actions.UI.Enable();
        PlayerController.Actions.UI.ECS.Enable();
        PlayerController.Actions.UI.ECS.performed += _ => _view.ShowSettings();
    }
    private void InitView()
    {
        OnResetVolumesEvent.Subscribe(volumes => ResetVolumes(volumes));

        _view.MasterVolume = MasterVolume;
        _view.SEVolume = SEVolume;
        _view.BGMVolume = BGMVolume;
    }
    private void VolumeChange(string propertyName, float value)
    {
        _audioMixer.SetFloat(propertyName, value);
    }

    private void OnDecision(float newMasterVolume, float newSEVolume, float newBGMVolume)
    {
        _model.OnDecision(newMasterVolume, newSEVolume, newBGMVolume);
        _source.Play();
    }

    private void OnCancel()
    {
        _model.OnCancel();
        _source.Play();
    }

    private void OnResetVolumes((float, float, float) volumes)
    {
        _onResetVolumesEventEvent.OnNext(volumes);
    }
    private void ResetVolumes((float, float, float) volumes)
    {
        _view.MasterVolume = volumes.Item1;
        _view.SEVolume = volumes.Item2;
        _view.BGMVolume = volumes.Item3;
    }
}
