using TMPro;
using UniRx;
using UnityEngine;
using VContainer;

public class ResultUIManager : UIManagerBase
{
    [SerializeField] private AudioSource _source;
    [SerializeField] private TextMeshProUGUI _text;
    
    [Inject]
    public void Constructor(InGameManager inGameManager)
    {
        inGameManager.OnGameStateChange.Where(gameState => gameState == GameState.Complete).
            Subscribe(_ => OnComplete());
        inGameManager.OnGameStateChange.Where(gameState => gameState == GameState.GameOver)
            .Subscribe(_ => OnGameOver());
    }
    
    private void OnComplete()
    {
        SetViewActive(true);
    }

    private void OnGameOver()
    {
        _text.text = "Game Over";
        SetViewActive(true);   
    }
    public void OnBackToHome()
    {
        _source.Play();
        GameManager.CompleteGame();
    }
}
