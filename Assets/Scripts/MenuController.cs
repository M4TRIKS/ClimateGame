using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MenuController : MonoBehaviour
{
    [Header("Menus")]
    public CanvasGroup mainMenu;
    public CanvasGroup optionsMenu;
    public CanvasGroup controlsMenu;

    [Header("Settings")]
    public float transitionDuration = .25f;
    public bool isPauseController = false;

    private bool _isPaused = false;

    void Start()
    {
        if (isPauseController)
            HideAllInstant();
        else
            ShowMainMenuInstant();
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
    // MAIN MENU / PANEL TRANSITIONS
    // =========================

    public void OpenOptionsMenu()
    {
        StartCoroutine(TransitionMenus(mainMenu, optionsMenu));
    }

    public void OpenControlsMenu()
    {
        StartCoroutine(TransitionMenus(mainMenu, controlsMenu));
    }

    public void BackFromOptions()
    {
        StartCoroutine(TransitionMenus(optionsMenu, mainMenu));
    }

    public void BackFromControls()
    {
        StartCoroutine(TransitionMenus(controlsMenu, mainMenu));
    }

    // =========================
    // PAUSE SYSTEM
    // =========================

    public void OpenPauseMenu()
    {
        _isPaused = true;
        ShowMainMenuInstant();

        if (MusicManager.Instance != null)
            MusicManager.Instance.PauseGameplayMusic();

        if (SceneLoader.Instance != null)
            SceneLoader.Instance.PauseGame();
        else
            Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        _isPaused = false;
        HideAllInstant();

        if (MusicManager.Instance != null)
            MusicManager.Instance.ResumeGameplayMusic();

        if (SceneLoader.Instance != null)
            SceneLoader.Instance.ResumeGame();
        else
            Time.timeScale = 1f;
    }

    public void QuitToMainMenu()
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.ResumeGameplayMusic();

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

        if (controlsMenu != null)
        {
            controlsMenu.alpha = 0;
            controlsMenu.interactable = false;
            controlsMenu.blocksRaycasts = false;
            controlsMenu.gameObject.SetActive(false);
        }
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

        if (controlsMenu != null)
        {
            controlsMenu.alpha = 0;
            controlsMenu.interactable = false;
            controlsMenu.blocksRaycasts = false;
            controlsMenu.gameObject.SetActive(false);
        }
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