using System;
using UniRx;
using UnityEngine;

public class SettingsModel
{
    public float MasterVolume { get; private set; }
    public float SEVolume { get; private set; }
    public float BGMVolume { get; private set; }

    public IObservable<(float, float, float)> OnCancelEvent => _onCancel;
    private Subject<(float, float, float)> _onCancel = new();

    private const string masterVolumesPrefsID = "Settings-MasterVolume";
    private const string seVolumesPrefsID = "Settings-SEVolume";
    private const string bgmVolumesPrefsID = "Settings-BGMVolume";

    public SettingsModel()
    {
        GetVolumes();
    }
    public void OnDecision(float newMasterVolume, float newSEVolume, float newBGMVolume)
    {
        MasterVolume = newMasterVolume;
        SEVolume = newSEVolume;
        BGMVolume = newBGMVolume;
        
        SaveVolumes();
    }
    public void OnCancel()
    {
        _onCancel.OnNext((MasterVolume, SEVolume, BGMVolume));
    }

    private void SaveVolumes()
    {
        PlayerPrefs.SetFloat(masterVolumesPrefsID, MasterVolume);
        PlayerPrefs.SetFloat(seVolumesPrefsID, SEVolume);
        PlayerPrefs.SetFloat(bgmVolumesPrefsID, BGMVolume);
    }

    private void GetVolumes()
    {
        MasterVolume = PlayerPrefs.GetFloat(masterVolumesPrefsID, MasterVolume);
        SEVolume = PlayerPrefs.GetFloat(seVolumesPrefsID, SEVolume);
        BGMVolume = PlayerPrefs.GetFloat(bgmVolumesPrefsID, BGMVolume);
    }
}
