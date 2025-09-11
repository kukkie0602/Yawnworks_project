using UnityEngine;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [Header("Level Data")]
    [Tooltip("The BeatmapData asset this button represents.")]
    public BeatmapData associatedBeatmap;

    [Header("UI References")]
    [Tooltip("The Text element that will display the song name.")]
    public TMP_Text songNameText;

    [Tooltip("The Text element that will display the high score.")]
    public TMP_Text highScoreText;

    [Tooltip("The Text element that will display the difficulty level.")]
    public TMP_Text difficultyText;

    void Start()
    {
        Debug.Log("Save File Path: " + Application.persistentDataPath);
        LoadAndDisplayData();
    }

    void LoadAndDisplayData()
    {
        if (associatedBeatmap == null)
        {
            Debug.LogError("No BeatmapData assigned to this button!");
            return;
        }

        if (songNameText != null)
        {
            songNameText.text = associatedBeatmap.levelName;
        }

        if (difficultyText != null)
        {
            difficultyText.text = "Difficulty: " + associatedBeatmap.difficulty;
        }

        if (highScoreText != null)
        {
            HighScoresData highScores = SaveSystem.LoadHighScores();

            int levelIndex = highScores.levelNames.IndexOf(associatedBeatmap.name);
            int score = 0;

            if (levelIndex != -1)
            {
                score = highScores.scores[levelIndex];
            }

            highScoreText.text = "High Score: " + score.ToString("D6");
        }
    }
}