using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndGamePanel : MonoBehaviour
{
    public TMP_Text finalScoreText;
    public ScoreManager scoreManager;
    public TMP_Text currentScoreText;
    public GameObject endGamePanel;
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void Continue()
    {
        SceneManager.LoadScene("LevelSelectorScene");
    }

    public void End()
    {
        endGamePanel.SetActive(true);
        scoreManager.disableScoreDisplay();
        if (scoreManager != null && finalScoreText != null)
        {
            finalScoreText.text = scoreManager.GetCurrentScore().ToString("D6");
        }
    }

    void Start()
    {
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(false);
        }
    }
}