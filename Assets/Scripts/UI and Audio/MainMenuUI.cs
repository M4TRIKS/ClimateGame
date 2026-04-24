using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    /// <summary>
    /// to just choose the surviing script 
    /// </summary>
    [SerializeField] private string _gameplaySceneName = "Gameplay";

    public void StartGame()
    {
        // load gameplay scene
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene(_gameplaySceneName);
        }
        else
        {
            Debug.LogError("SceneLoader.Instance is null!");
        }
    }

    public void QuitGame()
    {
        // quit application
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.QuitGame();
        }
        else
        {
            Debug.LogError("SceneLoader.Instance is null!");
        }
    }
}