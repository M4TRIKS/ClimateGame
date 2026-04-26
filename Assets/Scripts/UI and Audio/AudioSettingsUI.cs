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
        float masterValue = PlayerPrefs.GetFloat("MasterVolumeValue", 1f);
        float musicValue = PlayerPrefs.GetFloat("MusicVolumeValue", 1f);

        _masterSlider.value = masterValue;
        _musicSlider.value = musicValue;

        SetMasterVolume(masterValue);
        SetMusicVolume(musicValue);

        _masterSlider.onValueChanged.AddListener(SetMasterVolume);
        _musicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;

        PlayerPrefs.SetFloat("MasterVolumeValue", value);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float value)
    {
        float db = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;

        _mixer.SetFloat("MusicVolume", db);

        PlayerPrefs.SetFloat("MusicVolumeValue", value);
        PlayerPrefs.Save();
    }
}