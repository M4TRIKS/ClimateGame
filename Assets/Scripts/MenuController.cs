using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour
{
    [Header("Menus")]
    public CanvasGroup mainMenu;      // Main menu OR Pause main panel
    public CanvasGroup optionsMenu;   // Options panel

    [Header("Settings")]
    public float transitionDuration = .25f;
    public bool isPauseController = false;   // tick this ONLY in Gameplay scene

    private bool _isPaused = false;

    void Start()
    {
        HideAllInstant();
    }

    void Update()
    {
        if (!isPauseController) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_isPaused)
                OpenPauseMenu();
            else
                ResumeGame();
        }
    }

    // =========================
    // MAIN MENU TRANSITIONS
    // =========================

    public void OpenOptionsMenu()
    {
        StartCoroutine(TransitionMenus(mainMenu, optionsMenu));
    }

    public void BackTot()
    {
        StartCoroutine(TransitionMenus(optionsMenu, mainMenu));
    }

    // =========================
    // PAUSE SYSTEM
    // =========================

    public void OpenPauseMenu()
    {
        _isPaused = true;
        ShowMainMenuInstant();

        if (SceneLoader.Instance != null)
            SceneLoader.Instance.PauseGame();
        else
            Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        _isPaused = false;
        HideAllInstant();

        if (SceneLoader.Instance != null)
            SceneLoader.Instance.ResumeGame();
        else
            Time.timeScale = 1f;
    }

    public void QuitToMainMenu()
    {
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.ResumeGame();
            SceneLoader.Instance.LoadScene("MainMenu");
        }
        else
        {
            Time.timeScale = 1f;
            Debug.LogError("SceneLoader.Instance is null");
        }
    }

    // =========================
    // VISIBILITY HELPERS
    // =========================

    public void ShowMainMenuInstant()
    {
        mainMenu.alpha = 1;
        mainMenu.interactable = true;
        mainMenu.blocksRaycasts = true;
        mainMenu.gameObject.SetActive(true);

        optionsMenu.alpha = 0;
        optionsMenu.interactable = false;
        optionsMenu.blocksRaycasts = false;
        optionsMenu.gameObject.SetActive(false);
    }

    public void HideAllInstant()
    {
        mainMenu.alpha = 0;
        mainMenu.interactable = false;
        mainMenu.blocksRaycasts = false;
        mainMenu.gameObject.SetActive(false);

        optionsMenu.alpha = 0;
        optionsMenu.interactable = false;
        optionsMenu.blocksRaycasts = false;
        optionsMenu.gameObject.SetActive(false);
    }

    IEnumerator TransitionMenus(CanvasGroup currentMenu, CanvasGroup nextMenu)
    {
        float timer = 0f;
        nextMenu.gameObject.SetActive(true);

        while (timer < transitionDuration)
        {
            timer += Time.unscaledDeltaTime;
            float progress = timer / transitionDuration;

            currentMenu.alpha = 1 - progress;
            nextMenu.alpha = progress;

            yield return null;
        }

        currentMenu.alpha = 0;
        nextMenu.alpha = 1;

        currentMenu.interactable = false;
        currentMenu.blocksRaycasts = false;
        currentMenu.gameObject.SetActive(false);

        nextMenu.interactable = true;
        nextMenu.blocksRaycasts = true;
    }
}