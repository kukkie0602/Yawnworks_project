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

    void Start()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        UpdateTotalCoinsDisplay();
    }
    private void UpdateTotalCoinsDisplay()
    {
        if (totalCoinsText == null)
        {
            Debug.LogWarning("Total Coins Text is not assigned in the inspector!");
            return;
        }

        HighScoresData highScores = SaveSystem.LoadHighScores();
        if (highScores != null)
        {
            int totalCoins = highScores.coins.Sum();
            totalCoinsText.text = totalCoins.ToString();
        }
        else
        {
            totalCoinsText.text = "0";
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