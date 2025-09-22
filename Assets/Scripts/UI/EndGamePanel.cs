using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndGamePanel : MonoBehaviour
{
    public TMP_Text ScoreNumber;
    public TMP_Text HitNumber;
    public TMP_Text MissNumber;
    public TMP_Text ComboNumber;
    public ScoreManager scoreManager;
    public TMP_Text currentScoreText;
    public GameObject endGamePanel;
    public TMP_Text endGameMessageText;
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void Continue()
    {
        SceneManager.LoadScene("PostOfficeLevelsScene");
    }

    public void End()
    {
        endGamePanel.SetActive(true);
        scoreManager.disableScoreDisplay();
        scoreManager.CheckForHighScore();
        if (scoreManager != null && ScoreNumber != null)
        {
            ScoreNumber.text = scoreManager.GetCurrentScore().ToString("D6");
        }
        if (scoreManager != null && HitNumber != null)
        {
            HitNumber.text = scoreManager.GetTotalNotesHit().ToString();
        }
        if (scoreManager != null && MissNumber != null)
        {
            MissNumber.text = scoreManager.GetTotalNotesMissed().ToString();
        }
        if (scoreManager != null && ComboNumber != null)
        {
            ComboNumber.text = scoreManager.GetComboCount().ToString();
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