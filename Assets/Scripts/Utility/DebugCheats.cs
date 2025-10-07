using System.Collections.Generic;
using UnityEngine;

public class DebugCheats : MonoBehaviour
{
    private const int MAX_SCORE_VALUE = 999999;
    private const int MAX_COIN_VALUE = 3;

    public List<EnvelopeLevel> allGameLevels;

    public void MaxOutAllScoresAndCoins()
    {
        HighScoresData highScores = SaveSystem.LoadHighScores();

        foreach (EnvelopeLevel level in allGameLevels)
        {
            if (!highScores.levelNames.Contains(level.name))
            {
                highScores.levelNames.Add(level.name);
                highScores.scores.Add(0);
                highScores.coins.Add(0);
            }
        }

        for (int i = 0; i < highScores.scores.Count; i++)
        {
            highScores.scores[i] = MAX_SCORE_VALUE;
        }

        for (int i = 0; i < highScores.coins.Count; i++)
        {
            highScores.coins[i] = MAX_COIN_VALUE;
        }

        SaveSystem.SaveHighScores(highScores);
    }
}
