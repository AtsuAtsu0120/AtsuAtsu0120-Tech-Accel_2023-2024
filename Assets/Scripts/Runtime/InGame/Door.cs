using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class Door : MonoBehaviour, IIntaract
{
    [SerializeField] private Animator _doorAnimator;
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _openSound;
    [SerializeField] private AudioClip _closeSound;
    
    [Header("パラメータ")] [SerializeField] private int _doorOpenTime;
    private static readonly int OnDoorOpen = Animator.StringToHash("onDoorOpen");
    private static readonly int OnDoorClose = Animator.StringToHash("onDoorClose");
    
    private static readonly int OnDoorOpenInside = Animator.StringToHash("onDoorOpenInside");
    private static readonly int OnDoorCloseInside = Animator.StringToHash("onDoorCloseInside");
    
    [SerializeField] private Vector3 _rayHalfExraytents;
    [SerializeField] private float _rayDistance;
    private int _layer = 1 << 6;
    private Vector3 _doorOffset = new(0.5f, 1.0f, 0.0f);
    
    public string InteractName { get; } = "Door";
    public void OnIntaract()
    {
        var scale = transform.localScale.x;
        var isFront = GetSide();
        
        // Scaleによって逆方向にされている場合、アニメーションを反転させる。
        if (Math.Abs(scale - (-1.0f)) == 0)
        {
            isFront = !isFront;
        }
        
        if (isFront)
        {
            PlayDoorAnimation(OnDoorOpen, OnDoorClose);
        }
        else
        {
            PlayDoorAnimation(OnDoorOpenInside, OnDoorCloseInside);
        }
    }

    /// <summary>
    /// ドアのどっち側にいるか
    /// </summary>
    /// <returns>true:正面, false:裏面</returns>
    private bool GetSide()
    {
        var raycastPosition = transform.position + _doorOffset;
        
        //プレイヤーが正面にいたら
        if (Physics.BoxCast(raycastPosition, _rayHalfExraytents, transform.forward, out var hit,
                Quaternion.identity, _rayDistance, _layer, QueryTriggerInteraction.Ignore))
        {
            return true;
        }
        
        // いなかったら裏側にいるはず
        return false;
    }

    private async void PlayDoorAnimation(int doorOpenHash, int doorCloseHash)
    {
        _doorAnimator.SetTrigger(doorOpenHash);
        _source.PlayOneShot(_openSound);
        
        await UniTask.Delay(_doorOpenTime);
        
        _doorAnimator.SetTrigger(doorCloseHash);
        _source.PlayOneShot(_closeSound);
    }
    
#if UNITY_EDITOR
    private void Reset()
    {
        _doorAnimator = GetComponent<Animator>();
        _source = GetComponent<AudioSource>();
    }
#endif
}
