using System;
using InGame;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using VContainer;

public class NPC : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private bool _isEnemy;

    [SerializeField] private Vector3 _boxSize;
    [SerializeField] private float _raycastDistance;
    
    [Header("コンポーネント")]
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private NPCRoundAsset _roundAsset;
    [SerializeField] private Animator _animator;

    public float VigilancePercent => _vigilancePercent;
    
    private bool _isVigilance;
    private int _nextGoalIndex;
    /// <summary>
    /// 警戒度
    /// </summary>
    [Range(0, 100)]
    private float _vigilancePercent;

    private InGameManager _inGameManager;
    private IndicatorManager _indicatorManager;

    private static readonly int Speed = Animator.StringToHash("Speed");

    [Inject]
    public void Constructor(InGameManager inGameManager, IndicatorManager indicatorManager)
    {
        _inGameManager = inGameManager;
        _indicatorManager = indicatorManager;
    }
    #region UnityMethods
    
    private void Start()
    {
        _agent.updatePosition = false;
    }
    
    private void Update()
    {
        ConfirmPlayer();
    }

    private void FixedUpdate()
    {
        Move();
    }

    #endregion
    #region Methods

    private void ConfirmPlayer()
    {
        // 最適化のため、近くにいないとRaycastをうたないように。+ 敵として設定されていない場合はなし。
        if (!_isVigilance && !_isEnemy)
        {
            return;
        }
        
        var isWatched = false;
        if (Physics.BoxCast(transform.position + Vector3.up * 0.5f,
                _boxSize, transform.forward, out var hit,
                Quaternion.identity, _raycastDistance, 1 << 6, QueryTriggerInteraction.Ignore))
        {
            // 当たったプレイヤーが扇状にいるのか（NPCの視野に入っているか確認）
            if (AccelUtility.IsHitInAngle(hit.transform, transform, 1.4f, 0.6f))
            {
                isWatched = true;
                _vigilancePercent += Time.deltaTime;

                if (!_indicatorManager.IsIndicatorActive(this))
                {
                    //Indicatorを追加する。
                    _indicatorManager.AddIndicator(this);
                    _inGameManager.IsLooked = true;
                }
                // 警戒度が上がると、見つけたということにする。
                if (_vigilancePercent > 3)
                {
                    _inGameManager.OnFoundFromEnemy();
                    _agent.isStopped = true;

                    _agent.updateRotation = false;
                    var direction = hit.transform.position - transform.position;
                    direction.y = 0;

                    var lookRotation = Quaternion.LookRotation(direction, Vector3.up);
                    transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 0.2f);
                }
            }
        }
        if(!isWatched)
        {
            _inGameManager.OnNotFoundFromEnemy();
            _agent.isStopped = false;
            _agent.updateRotation = true;
            // 0より大きい時で見つかっていない時は少しずつ下げていく。
            if (_vigilancePercent > 0)
            {
                _vigilancePercent -= Time.deltaTime;
            }
            
            if (_indicatorManager.IsIndicatorActive(this))
            {
                //Indicatorを削除
                _indicatorManager.RemoveIndicator(this);
                _inGameManager.IsLooked = false;
            }
        }
    }

    private void Move()
    {
        if(_roundAsset is null) return;
        if (_agent.remainingDistance < 0.1f)
        {
            if (!_roundAsset.GetableGoal(_nextGoalIndex))
            {
                _nextGoalIndex = 0;
            }
            _agent.destination = _roundAsset.GetGoal(_nextGoalIndex);
            _nextGoalIndex++;
        }
        
        //// アニメーション
        // nextPositionからdeltaPositionを算出
        var worldDeltaPosition = _agent.nextPosition - transform.position;

        // キャラクターを基点にしたxz平面に射影したdeltaPosition
        var dx = Vector3.Dot(transform.right, worldDeltaPosition);
        var dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        var deltaPosition = new Vector2(dx, dy);

        // Time.deltaTimeから速度を算出
        var velocity = deltaPosition / Time.deltaTime;

        _animator.SetFloat(Speed, velocity.y);
        
        // 実際に移動させる
        transform.position = _agent.nextPosition;
    }
    #endregion
    #region Events
    
    public void OnCloseToPlayer()
    {
        _isVigilance = true;
    }

    public void OnLeavePlayer()
    {
        _isVigilance = false;
    }
    
    #endregion
    
    #region UnityEditor
    #if UNITY_EDITOR
    
    private void Reset()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }
    
    #endif
    #endregion
}
