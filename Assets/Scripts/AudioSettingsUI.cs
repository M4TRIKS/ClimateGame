using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider _volumeSlider;

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        AudioListener.volume = savedVolume;

        if (_volumeSlider != null)
        {
            _volumeSlider.value = savedVolume;
            _volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
    }
}