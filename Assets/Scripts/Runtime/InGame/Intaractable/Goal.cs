using UnityEngine;
using VContainer;

public class Goal : MonoBehaviour, IIntaract
{
    private InGameManager _inGameManager;
    [Inject]
    public void Constructor(InGameManager inGameManager)
    {
        _inGameManager = inGameManager;
    }
    public void OnIntaract()
    {
        _inGameManager.FinishInGame();
    }

    public string InteractName { get => $"Goal:{name}"; }
}