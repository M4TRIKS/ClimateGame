using System.Diagnostics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static SceneLoader Instance;

    private bool isPaused = false;
    private void Awake()
    {
        if(Instance == null)
        {
                Instance = this;
                DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
            
    }
    public void LoadScene (string sceneName)
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(sceneName);

    }
    public void ReloadScene()
    {
        Time.timeScale = 1f;
        isPaused =false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
    public void TogglePause()
    {
        if(isPaused)
        ResumeGame();
        else
        PauseGame();

    }
   public void PauseGame()
    {
    Time.timeScale = 0f;
    isPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale =1f;
        isPaused = false;
     }

     public void QuitGame()
{
    #if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
    #else
    Application.Quit();
    #endif
}

public void Clink()
    {
        transform.DOPunchScale(Vector3.one *0.2f, 0.3f);
    }
}

