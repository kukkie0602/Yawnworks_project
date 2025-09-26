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
    private SettingsData settingsData;

    void Start()
    {
        settingsData = SaveSystem.LoadSettings();

        mainMixer.SetFloat("MusicVolume", Mathf.Log10(settingsData.musicVolume) * 20);
        if (volumeSlider != null)
        {
            volumeSlider.value = settingsData.musicVolume;
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

    public void SetVolume(float volume)
    {
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        settingsData.musicVolume = volume;
        SaveSystem.SaveSettings(settingsData);
    }

    public void RetryLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
}