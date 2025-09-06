using UnityEngine;
using TMPro; // Only needed for TextMeshPro

public class ScoreManager : MonoBehaviour
{
    public int score = 0;

    public TMP_Text scoreText; // Use TMP_Text for TextMeshPro

    // Method called by the button
    public void IncreaseScore()
    {
        score++;
        scoreText.text = "Combo " + score + "x";
    }

    public void ResetScore()
    {
        score = 0;
        scoreText.text = "Combo " + score + "x";
    }
}
