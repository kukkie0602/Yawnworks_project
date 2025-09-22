using UnityEngine;
using TMPro;
using System.Linq;

public class ScoreManager : MonoBehaviour
{
    [Header("Scoring")]
    public int pointsPerNote = 100;
    private int maxScore;
    private int currentScore = 0;
    private int comboCount = 0;
    private int maxCombo = 12;
    private int missesThisAttempt = 0;
    private int hitsThisAttempt = 0;

    private int totalNotesHit = 0;
    private int totalNotesMissed = 0;
    private bool isTutorial = false;

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

        if (currentLevel != null && currentLevel.levelName == "Tutorial")
        {
            isTutorial = true;
        }

        maxScore = CalculateMaxScore();
        ResetScore();
    }
    public int CalculateMaxScore()
    {
        if (currentLevel == null)
        {
            Debug.LogError("CurrentLevel is not assigned in ScoreManager.");
            return 0;
        }

        int calculatedMaxScore = 0;
        int currentCombo = 0;

        foreach (var sequence in currentLevel.sequences)
        {
            foreach (var note in sequence.pattern)
            {
                if (note == NoteType.Tap || note == NoteType.HalfTap)
                {
                    if (currentCombo < maxCombo)
                    {
                        currentCombo++;
                    }
                    calculatedMaxScore += pointsPerNote * currentCombo;
                }
            }
        }

        return calculatedMaxScore;
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

    public int CalculateCoins()
    {
        if (isTutorial)
        {
            int misses = GetTotalNotesMissed();
            if (misses == 0)
            {
                return 3; 
            }
            else if (misses >= 1 && misses <= 5)
            {
                return 2; 
            }
            else
            {
                return 1;
            }
        }
        else
        {
            if (maxScore == 0) return 3;

            float scorePercentage = (float)currentScore / maxScore;
            if (scorePercentage >= 1f)
            {
                return 3;
            }
            else if (scorePercentage >= 0.66f)
            {
                return 2;
            }
            else if (scorePercentage >= 0.33f)
            {
                return 1;
            }
            return 0;
        }
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
        totalNotesMissed = 0;
        totalNotesHit = 0; 
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

        while (highScores.coins.Count < highScores.levelNames.Count)
        {
            highScores.coins.Add(0);
        }

        int levelIndex = highScores.levelNames.IndexOf(currentLevel.name);
        int coins = CalculateCoins();

        if (levelIndex != -1)
        {
            if (currentScore > highScores.scores[levelIndex])
            {
                highScores.scores[levelIndex] = currentScore;
            }
            if (coins > highScores.coins[levelIndex])
            {
                highScores.coins[levelIndex] = coins;
            }
        }
        else
        {
            highScores.levelNames.Add(currentLevel.name);
            highScores.scores.Add(currentScore);
            highScores.coins.Add(coins);
        }

        SaveSystem.SaveHighScores(highScores);
    }
}