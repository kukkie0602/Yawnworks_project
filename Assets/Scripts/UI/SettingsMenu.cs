using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("The main audio mixer for the game")]
    public AudioMixer mainMixer;

    [Tooltip("The slider for controlling music volume")]
    public Slider musicVolumeSlider;

    void OnEnable()
    {
        LoadVolume();
    }

    void Start()
    {
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }
    }

    public void SetMusicVolume(float volume)
    {
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void SaveVolume()
    {
        if (musicVolumeSlider != null)
        {
            SettingsData data = new SettingsData();
            data.musicVolume = musicVolumeSlider.value;
            SaveSystem.SaveSettings(data);
        }
    }

    public void LoadVolume()
    {
        SettingsData data = SaveSystem.LoadSettings();
        if (data != null && musicVolumeSlider != null)
        {
            musicVolumeSlider.value = data.musicVolume;
            SetMusicVolume(data.musicVolume);
        }
    }

    public void CloseSettings()
    {
        SaveVolume();
        gameObject.SetActive(false);
    }
}