using UnityEngine.SceneManagement;

public static class GameManager
{
    private const string StartScreenSceneName = "StartScreen";
    /// <summary>
    /// ゲームを始めるときのシーン切り替え。
    /// </summary>
    public static void StartGame<T>() where T : IStage
    {
        SceneManager.LoadScene(typeof(T).Name);
    }
    
    /// <summary>
    /// ゲーム終了時のシーン切り替え
    /// </summary>
    public static void CompleteGame()
    {
        SceneManager.LoadScene(StartScreenSceneName);
    }
}
