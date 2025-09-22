using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EndGamePanel : MonoBehaviour
{
    public TMP_Text ScoreNumber;
    public TMP_Text HitNumber;
    public TMP_Text MissNumber;
    public TMP_Text ComboNumber;
    public ScoreManager scoreManager;
    public GameObject endGamePanel;
    public Image[] coins;
    public Color earnedCoinColor = Color.yellow;
    public Color unearnedCoinColor = Color.grey;

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
        UpdateCoinDisplay(scoreManager.CalculateCoins());
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

    public void UpdateCoinDisplay(int coinsEarned)
    {
        for (int i = 0; i < coins.Length; i++)
        {
            if (i < coinsEarned)
            {
                coins[i].color = earnedCoinColor;
            }
            else
            {
                coins[i].color = unearnedCoinColor;
            }
        }
    }
}