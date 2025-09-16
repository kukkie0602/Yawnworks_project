using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("Scoring")]
    public int pointsPerNote = 100;
    private int currentScore = 0;
    private int comboCount = 0;
    private int maxCombo = 12;

    [Header("UI Elements")]
    public TMP_Text scoreText;
    public TMP_Text comboText;

    void Start()
    {
        enableScoreDisplay();
        ResetScore();
    }

    public void OnNoteHit()
    {
        if (comboCount < maxCombo)
            comboCount++;

        currentScore += pointsPerNote * comboCount;

        UpdateDisplay();
    }

    public void OnNoteMiss()
    {
        comboCount = 0;
        UpdateDisplay();
    }

    public void ResetScore()
    {
        currentScore = 0;
        comboCount = 0;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " +  currentScore.ToString("D6");
        }

        if (comboText != null)
        {
            if (comboCount > 1)
            {
                comboText.gameObject.SetActive(true);
                comboText.text = comboCount + "x Combo";
            }
            else
            {
                comboText.gameObject.SetActive(false);
            }
        }
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public void disableScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(false);
        }
        if (comboText != null)
        {
            comboText.gameObject.SetActive(false);
        }
    }

    public void enableScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(true);
        }
        if (comboText != null)
        {
            comboText.gameObject.SetActive(true);
        }
    }
}