using System.Diagnostics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // global instance to call this script from anywhere
    public static SceneLoader Instance;

    // checks if game is paused
    private bool isPaused = false;

    private void Awake()
    {
        // singleton so only one SceneLoader exists
        if(Instance == null)
        {
                Instance = this;
                DontDestroyOnLoad(gameObject); // keep it between scenes
        }
        else
        {
            Destroy(gameObject); // destroy duplicate
        }
    }

    // loads a scene by name
    public void LoadScene (string sceneName)
    {
        Time.timeScale = 1f; // make sure game is running
        isPaused = false;
        SceneManager.LoadScene(sceneName);
    }

    // reload current active scene
    public void ReloadScene()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // switch between pause and resume
    public void TogglePause()
    {
        if(isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    // stops time
    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
    }

    // resumes time
    public void ResumeGame()
    {
        Time.timeScale =1f;
        isPaused = false;
    }

    // closes game
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    // small UI punch animation
    public void Clink()
    {
        transform.DOPunchScale(Vector3.one *0.2f, 0.3f);
    }
}