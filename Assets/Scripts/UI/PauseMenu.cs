using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pauseMenuPanel;
    public Slider volumeSlider;
    public GameObject pauseMenuButton;

    public ScoreManager scoreManager;
    public EnvelopeConveyor envelopeConveyor;

    [Header("Audio")]
    public AudioMixer mainMixer;
    public AudioSource musicSource;

    private bool isPaused = false;

    void OnEnable()
    {
        LoadVolume();
    }

    void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenuPanel.SetActive(true);
        if (scoreManager.scoreDisplayEnabled)
        {
            scoreManager.disableScoreDisplay();
        }
        pauseMenuButton.SetActive(false);

        Time.timeScale = 0f;
        musicSource.Pause();
        envelopeConveyor.Pause();
    }

    public void ResumeGame()
    {
        SaveVolume();
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        if (scoreManager.scoreDisplayEnabled)
        {
            scoreManager.enableScoreDisplay();
        }
        pauseMenuButton.SetActive(true);

        Time.timeScale = 1f;
        musicSource.UnPause();
        envelopeConveyor.Resume();
    }

    public void SetMusicVolume(float volume)
    {
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void SaveVolume()
    {
        if (volumeSlider != null)
        {
            SettingsData data = new SettingsData();
            data.musicVolume = volumeSlider.value;
            SaveSystem.SaveSettings(data);
        }
    }

    public void LoadVolume()
    {
        SettingsData data = SaveSystem.LoadSettings();
        if (data != null && volumeSlider != null)
        {
            volumeSlider.value = data.musicVolume;
            SetMusicVolume(data.musicVolume);
        }
    }

    public void RetryLevel()
    {
        SaveVolume();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMainMenu()
    {
        SaveVolume();
        Time.timeScale = 1f;
        SceneManager.LoadScene("PostOfficeLevelsScene");
    }
}