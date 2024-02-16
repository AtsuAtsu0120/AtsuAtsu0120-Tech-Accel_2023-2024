using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class StartScreenManager : MonoBehaviour
{
    private AccelActions actions;
    private void Start()
    {
        actions = new AccelActions();
        actions.UI.Enable();
        
        actions.UI.Click.Enable();
        actions.UI.Click.performed += OnStartGame;
    }

    private void OnDestroy()
    {
        actions.UI.Click.Dispose();
    }

    private void OnStartGame(InputAction.CallbackContext _)
    {
        GameManager.StartGame<HomeStage>();
    }
}
