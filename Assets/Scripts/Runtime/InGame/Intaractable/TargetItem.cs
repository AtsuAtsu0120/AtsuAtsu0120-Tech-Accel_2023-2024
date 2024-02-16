using UniRx;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TargetItem : TargetBase, IIntaract
{
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _clip;
    
    public string InteractName => $"Target: {TargetName}";
    public override string TargetName => "Get the Plant";
    public void OnIntaract()
    {
        isAchieved.Value = true;
        Destroy(transform.gameObject);
        
        _source.PlayOneShot(_clip);
    }
}