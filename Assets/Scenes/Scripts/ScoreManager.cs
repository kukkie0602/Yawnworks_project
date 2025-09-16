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

    public void CheckForHighScore(EnvelopeLevel beatmap)
    {
        if (beatmap == null) return;

        HighScoresData highScores = SaveSystem.LoadHighScores();

        int levelIndex = highScores.levelNames.IndexOf(beatmap.name);

        if (levelIndex != -1)
        {
            if (currentScore > highScores.scores[levelIndex])
            {
                highScores.scores[levelIndex] = currentScore;
            }
        }
        else
        {
            highScores.levelNames.Add(beatmap.name);
            highScores.scores.Add(currentScore);
        }

        SaveSystem.SaveHighScores(highScores);
    }
}