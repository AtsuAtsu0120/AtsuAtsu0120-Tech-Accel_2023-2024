using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VContainer;

public class InGameManager : IDisposable
{
    public IStage Stage { get; }
    public IObservable<float> ConfirmPercent => _confirmPercent;
    public IObservable<bool> IsLookedObservable => _isLooked;

    public bool IsLooked
    {
        get => _isLooked.Value;
        set => _isLooked.Value = value;
    }
    public IObservable<GameState> OnGameStateChange => _gameState;

    private readonly ReactiveProperty<GameState> _gameState = new();
    
    private readonly ReactiveProperty<bool> _isLooked = new();
    private readonly EventManager _eventManager = new();
    private bool _isFound;
    
    /// <summary>
    /// 発見度。NPCの警戒度が一定以上の時に増加する。最大値まで行くとGAME OVER
    /// </summary>
    private readonly ReactiveProperty<float> _confirmPercent = new();

    private const int DecreaseDelay = 2000;
    public const float MaxConfirm = 3;

    [Inject]
    public InGameManager(IStage stage)
    {
        Stage = stage;
        _eventManager.OnDecreaseConfirmPercent
            .Subscribe(_ => DecreaseConfirmPercent());

        _confirmPercent.Where(value => value > MaxConfirm).Subscribe(_ => GameOver());
    }
    /// <summary>
    /// ゴールしようとしたときのメソッド。ミッション（ターゲット）を全て達成していたらそのままクリア。
    /// </summary>
    public void FinishInGame()
    {
        if (Stage.IsComplete)
        {
            _eventManager.Dispose();
            Dispose();

            _gameState.Value = GameState.Complete;
        }
        else
        {
            Debug.Log("ミッション（ターゲット）を達成していません。");
        }
    }

    private void GameOver()
    {
        _gameState.Value = GameState.GameOver;
    }

    public void OnFoundFromEnemy()
    {
        _isFound = true;
        
        // 見つかっているなら発見度を増やす。
        IncreseConfirmPercent();
        // 見つかったときは減少をキャンセル
        _eventManager.ShouldDecreaseConfirmPercent = false;
    }

    public async void OnNotFoundFromEnemy()
    {
        // 見つかっていない時に少し遅延したのちに減少を始める。
        if (_isFound)
        {
            _isFound = false;
            await UniTask.Delay(DecreaseDelay);
            _eventManager.ShouldDecreaseConfirmPercent = true;
        }
    }
    
    private void IncreseConfirmPercent()
    {
        _confirmPercent.Value += Time.deltaTime;
    }
    private void DecreaseConfirmPercent()
    {
        _confirmPercent.Value -= Time.deltaTime;
        if (_confirmPercent.Value < 0)
        {
            _eventManager.ShouldDecreaseConfirmPercent = false;
        }
    }

    public void Dispose()
    {
        // ReactivePropertyの解放
        _confirmPercent?.Dispose();
        
        // イベントマネージャーの解放
        _eventManager?.Dispose();
    }
}

public enum GameState
{
    Playing,
    Complete,
    GameOver
}
