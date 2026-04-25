using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _musicSlider;

    void Start()
    {
        SetupSlider(_masterSlider, "MasterVolume", "MasterSave");
        SetupSlider(_musicSlider, "MusicVolume", "MusicSave");
    }

    void SetupSlider(Slider slider, string parameter, string saveKey)
    {
        float value = PlayerPrefs.GetFloat(saveKey, 1f);

        slider.value = value;
        SetVolume(parameter, value);

        slider.onValueChanged.AddListener(v =>
        {
            SetVolume(parameter, v);
            PlayerPrefs.SetFloat(saveKey, v);
            PlayerPrefs.Save();
        });
    }

    void SetVolume(string parameter, float value)
    {
        float db = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        _mixer.SetFloat(parameter, db);
    }
}