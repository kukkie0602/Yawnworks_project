using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [Header("Level Data")]
    [Tooltip("The BeatmapData asset this button represents.")]
    public EnvelopeLevel associatedLevel;

    [Header("UI References")]
    [Tooltip("The Text element that will display the song name.")]
    public TMP_Text songNameText;

    [Tooltip("The Text element that will display the high score.")]
    public TMP_Text highScoreText;

    [Tooltip("The Text element that will display the difficulty level.")]
    public TMP_Text difficultyText;

    public Image[] coins;
    public Color earnedCoinColor = Color.yellow;
    public Color unearnedCoinColor = Color.grey;

    void Start()
    {
        Debug.Log("Save File Path: " + Application.persistentDataPath);
        LoadAndDisplayData();
    }

    void LoadAndDisplayData()
    {
        if (associatedLevel == null)
        {
            Debug.LogError("No LevelData assigned to this button!");
            return;
        }

        if (songNameText != null)
        {
            songNameText.text = associatedLevel.levelName;
        }

        if (difficultyText != null)
        {
            difficultyText.text = "Difficulty: " + associatedLevel.difficulty;
        }

        if (highScoreText != null)
        {
            HighScoresData highScores = SaveSystem.LoadHighScores();

            while (highScores.coins.Count < highScores.levelNames.Count)
            {
                highScores.coins.Add(0); 
            }

            int levelIndex = highScores.levelNames.IndexOf(associatedLevel.name);
            int score = 0;
            int coinsEarned = 0;

            if (levelIndex != -1)
            {
                score = highScores.scores[levelIndex];
                coinsEarned = highScores.coins[levelIndex]; 
            }

            highScoreText.text = "High Score: " + score.ToString("D6");
            UpdateCoinDisplay(coinsEarned);
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