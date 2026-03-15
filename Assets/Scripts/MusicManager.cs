using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Music Clips")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip gameplayMusic;

    private AudioSource _audio;
    private bool _isGameplayScene = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _audio = GetComponent<AudioSource>();
        _audio.loop = true;
        _audio.playOnAwake = false;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            _isGameplayScene = false;
            PlayMusic(mainMenuMusic);
        }
        else
        {
            _isGameplayScene = true;
            PlayMusic(gameplayMusic);
        }
    }

    void PlayMusic(AudioClip clip)
    {
        if (_audio.clip == clip) return;

        _audio.clip = clip;
        _audio.Play();
    }

    // ⭐ called by pause menu
    public void PauseGameplayMusic()
    {
        if (_isGameplayScene && _audio.isPlaying)
            _audio.Pause();
    }

    public void ResumeGameplayMusic()
    {
        if (_isGameplayScene)
            _audio.UnPause();
    }
}