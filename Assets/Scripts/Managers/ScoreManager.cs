using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("Scoring")]
    public int pointsPerNote = 100;  
    private int currentScore = 0;
    private int comboCount = 0;
    private int maxCombo = 12;
    private int missesThisAttempt = 0;
    private int hitsThisAttempt = 0;

    private int totalNotesHit = 0;
    private int totalNotesMissed = 0;

    [Header("UI Elements")]
    public TMP_Text scoreText;  
    public TMP_Text comboText;  

    [Header("Level Info")]
    public EnvelopeLevel currentLevel;

    public bool scoreDisplayEnabled = true;
    void Start()
    {
        if (!scoreDisplayEnabled)
        {
            disableScoreDisplay();
        }
        ResetScore();  
    }

    public bool IsScoreDisplayEnabled()
    {
        return scoreDisplayEnabled;
    }

    public void OnNoteHit()
    {
        if (comboCount < maxCombo)
            comboCount++;

        currentScore += pointsPerNote * comboCount;
        hitsThisAttempt++;
        totalNotesHit++;
        UpdateDisplay();
    }

    public void OnNoteMiss()
    {
        comboCount = 0;
        missesThisAttempt++;
        totalNotesMissed++; 
        UpdateDisplay();
    }

    public void ResetAttemptStats()
    {
        missesThisAttempt = 0;
        hitsThisAttempt = 0;
    }

    public int GetMissesThisAttempt()
    {
        return missesThisAttempt;
    }

    public int GetHitsThisAttempt()
    {
        return hitsThisAttempt;
    }

    public int GetTotalNotesHit()
    {
        return totalNotesHit;
    }

    public int GetTotalNotesMissed()
    {
        return totalNotesMissed;
    }

    public int GetComboCount()
    {
        return comboCount;
    }


    public void ResetScore()
    {
        currentScore = 0;
        comboCount = 0;
        ResetAttemptStats();
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (!scoreDisplayEnabled) return;
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString("D6");  
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

    public void CheckForHighScore()
    {
        if (currentLevel == null) return;  

        HighScoresData highScores = SaveSystem.LoadHighScores();

        int levelIndex = highScores.levelNames.IndexOf(currentLevel.name);  

        if (levelIndex != -1)
        {
            if (currentScore > highScores.scores[levelIndex])
            {
                highScores.scores[levelIndex] = currentScore;
            }
        }
        else
        {
            highScores.levelNames.Add(currentLevel.name);
            highScores.scores.Add(currentScore);
        }

        SaveSystem.SaveHighScores(highScores);
    }
}