using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class LevelSelector : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("The settings panel to toggle")]
    public GameObject settingsPanel;

    [Header("Coin Display")]
    [Tooltip("The Text element to display the total coins")]
    public TMP_Text totalCoinsText;

    [Header("Progress Bar")]
    public ProgressBarManager progressBar;

    void Start()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        int testCoins = 2;
        if (progressBar != null)
        {
            progressBar.playerCoins = testCoins;
            progressBar.UpdateProgressBar();
        }
        UpdateTotalCoinsDisplay();
    }
    private void UpdateTotalCoinsDisplay()
    {
        HighScoresData highScores = SaveSystem.LoadHighScores();
        int totalCoins = 0;

        if (highScores != null)
        {
            totalCoins = highScores.coins.Sum();
        }

        if (progressBar != null)
        {
            progressBar.playerCoins = totalCoins+2;
            progressBar.UpdateProgressBar();
        }
        else
        {
            Debug.LogWarning("ProgressBar reference not assigned in the inspector!");
        }
    }


    public void SelectLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleSettingsPanel()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
        else
        {
            Debug.LogError("Settings Panel is not assigned in the inspector!");
        }
    }
}