using System;
using UniRx;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Audio;
using VContainer;

namespace InGame.Base
{
    // もはやMoveBaseとはという感じになってしまった。今後の実装は気を付ける。
    [RequireComponent(typeof(Rigidbody))]
    public abstract class MoveBase : MonoBehaviour
    {
        [Header("移動系パラメーター")] 
        [SerializeField] private float _playerMoveSpeed;

        [SerializeField] private float _playerSprintBonus;
        [Header("視点系パラメーター")]
        [SerializeField] private float _sensibility;

        [SerializeField] private float _maxAngle;
        [SerializeField] private float _minAngle;

        [Header("その他パラメーター")]
        [SerializeField] private Vector3 _rayHalfExraytents;

        [SerializeField] private float _onGroundRayDistance;

        [SerializeField] private float _rayDistance;

        [Header("音関係")] 
        [SerializeField] private AudioResource _grassAudio;
        [SerializeField] private AudioResource _woodAudio;
        [SerializeField] private AudioResource _stoneAudio;
        
        [Header("必要なコンポーネント")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Transform _camTransform;
        [SerializeField] private Animator _animator;
        [SerializeField] private InteractUIManager _interactUIManager;
        [SerializeField] private AudioSource _stepSource;
        [SerializeField] private AudioSource _heartbeatSource;

        private bool _isPlayingHeartbeat = false;

        protected Subject<Unit> IntaractEvent { get; private set; }
        
        private int layer = 1 << 3;
        private IDisposable _subscribedDisposable;
        
        public bool IsSprint { get; set; }
        //この書き方初めて知った…
        private static readonly int Speed = Animator.StringToHash("Speed");
        
        [Inject]
        public void Constructor(InGameManager inGameManager)
        {
            inGameManager.IsLookedObservable.Where(isLooked => isLooked)
                .Subscribe(_ => PlayHeartbeatSound());
            inGameManager.IsLookedObservable.Where(isLooked => !isLooked)
                .Subscribe(_ => StopHeartbeatSound());
        }
        protected virtual void OnEnable()
        {
            IntaractEvent = new();
        }

        protected virtual void Update()
        {
            SetInteractEvent();
            PlayStepSound();
        }

        private void PlayHeartbeatSound()
        {
            if (!_isPlayingHeartbeat)
            {
                _heartbeatSource.Play();
                _isPlayingHeartbeat = true;   
            }
        }

        private void StopHeartbeatSound()
        {
            _heartbeatSource.Pause();
            _isPlayingHeartbeat = false;
        }
        private void PlayStepSound()
        {
            if (_rigidbody.velocity.sqrMagnitude > 1 && OnGround(out var hit))
            {
                var tag = hit.collider.tag;
                var audioContainer = tag switch
                {
                    "Grass" => _grassAudio,
                    "Wood" => _woodAudio,
                    _ => _stoneAudio
                };
                if (audioContainer == _stepSource.resource) return;
                _stepSource.resource = audioContainer;

                _stepSource.Play();
            }
            else
            {
                _stepSource.resource = null;
                _stepSource.Stop();
            }
        }
        
        [BurstCompile]
        public void Move(in Vector2 vector)
        {
            if (vector.sqrMagnitude > 0)
            {
                //カメラのむきにプレイヤーの向きを合わせる。
                var camForward = _camTransform.forward;
                camForward.y = 0;

                var newAngle = Quaternion.LookRotation(camForward, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, newAngle, 0.2f);

                var gravityVector = OnGround() ? Vector3.zero 
                    : new Vector3(0, _rigidbody.velocity.y, 0);
                
                var bonus = IsSprint ? _playerSprintBonus : 1;
                //C# JobSystemを使いたい演算してるけど、流石に1つだけ(=Playerだけ)は逆効果か
                _rigidbody.velocity = vector.x * _playerMoveSpeed * transform.right +
                                     //NEXT: 演算の順序でパフォーマンス変わるらしい！！
                                     transform.forward * (vector.y * _playerMoveSpeed * bonus) +
                                     gravityVector;
            }
            else
            {
                _rigidbody.velocity = new(0, _rigidbody.velocity.y, 0);
            }
            _animator.SetFloat(Speed, _rigidbody.velocity.sqrMagnitude);
        }
        
        [BurstCompile]
        public void Look(in Vector2 vector)
        {
            //縦回転はカメラを動かす
            var camLocalAngle = _camTransform.localEulerAngles;
            //マイナスは反転させてる
            camLocalAngle.x += -vector.y * _sensibility;
            //縦回転は制限あり
            camLocalAngle.x = Mathf.Clamp(camLocalAngle.x, _minAngle, _maxAngle);
            
            _camTransform.localEulerAngles = camLocalAngle;
            
            //横回転
            var newAngle = vector.x * _sensibility;
            _camTransform.RotateAround(transform.position, Vector3.up, newAngle);
        }

        protected void SetInteractEvent()
        {
            //Raycastを飛ばす
            if (Physics.BoxCast(transform.position, _rayHalfExraytents, transform.forward, out var hit,
                    Quaternion.identity, _rayDistance, layer, QueryTriggerInteraction.Ignore))
            {
                //もしオブジェクトがIIntaractを持っていたらそのイベントを保持
                if (hit.transform.TryGetComponent<IIntaract>(out var component))
                {
                    _subscribedDisposable?.Dispose();
                    _subscribedDisposable = IntaractEvent.Subscribe(_ => component.OnIntaract());
                    _interactUIManager.SetViewActive(true);

                    _interactUIManager.InteractDescription = component.InteractName;
                }
            }
            else
            {
                _subscribedDisposable?.Dispose();
                _interactUIManager.SetViewActive(false);
            }
        }

        /// <summary>
        /// 現在地面に接地しているか。
        /// </summary>
        private bool OnGround(out RaycastHit hit)
        {
            // Raycastを飛ばす
            var onGround = Physics.Raycast(transform.position,
                -Vector3.up, out hit, _onGroundRayDistance);

            return onGround;
        }        
        private bool OnGround()
        {
            // Raycastを飛ばす
            var onGround = Physics.Raycast(transform.position,
                -Vector3.up, _onGroundRayDistance);

            return onGround;
        }

        //これってNPCに反映させられるんだろうか
        public void Interact()
        {
            IntaractEvent.OnNext(Unit.Default);
        }

        //ミニマムにするならいらないかも
        protected void Sprint()
        {
            
        }

        protected void Crouch()
        {
            
        }

        protected void Cover()
        {
            
        }
        //ここまで
        
        #if UNITY_EDITOR
        private void Reset()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        #endif
    }
   
}