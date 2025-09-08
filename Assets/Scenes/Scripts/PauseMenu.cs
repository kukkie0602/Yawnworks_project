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
    public GameObject scoreText;

    [Header("Audio")]
    public AudioMixer mainMixer;
    public AudioSource musicSource;


    private bool isPaused = false;

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
        scoreText.SetActive(false);
        pauseMenuButton.SetActive(false);

        Time.timeScale = 0f;

        musicSource.Pause();
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        scoreText.SetActive(true);
        pauseMenuButton.SetActive(true);

        Time.timeScale = 1f;


        musicSource.UnPause();
    }

    public void SetVolume(float volume)
    {
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
}