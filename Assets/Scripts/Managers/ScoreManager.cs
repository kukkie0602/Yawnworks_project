using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections;

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

    [Header("Combo Display")]
    public UnityEngine.UI.Image StreakImage;
    public Sprite[] streakSprites;

    [Header("Hit/Miss Feedback Prefabs")]
    public GameObject hitPrefab;
    public GameObject missPrefab;
    public float feedbackDuration = 0.8f;
    public float riseDistance = 150f;
    public float feedbackDelay = 0.2f;

    [Header("UI Elements")]
    public TMP_Text scoreText;

    [Header("Level Info")]
    public EnvelopeLevel currentLevel;

    [Header("Feedback Container")]
    public Transform feedbackContainer; 

    public bool scoreDisplayEnabled = true;

    void Start()
    {
        if (!scoreDisplayEnabled)
            disableScoreDisplay();

        if (currentLevel != null && currentLevel.levelName == "Tutorial")
            isTutorial = true;

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
                        currentCombo++;

                    calculatedMaxScore += pointsPerNote * currentCombo;
                }
            }
        }

        return calculatedMaxScore;
    }

    public void OnNoteHit()
    {
        if (comboCount < maxCombo)
            comboCount++;

        currentScore += pointsPerNote * comboCount;
        hitsThisAttempt++;
        totalNotesHit++;

        SpawnFeedback(true, feedbackDelay);
        UpdateDisplay(comboCount);
    }

    public void OnNoteMiss()
    {
        comboCount = 0;
        missesThisAttempt++;
        totalNotesMissed++;

        SpawnFeedback(false, feedbackDelay);
        UpdateDisplay(comboCount);
    }

    private void SpawnFeedback(bool isHit, float delay = 0f)
    {
        GameObject prefabToUse = isHit ? hitPrefab : missPrefab;
        if (prefabToUse == null || feedbackContainer == null) return;

        GameObject feedbackGO = Instantiate(prefabToUse, feedbackContainer);
        RectTransform rect = feedbackGO.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;

        StartCoroutine(AnimateFeedback(feedbackGO, delay));
    }

    private IEnumerator AnimateFeedback(GameObject feedbackGO, float delay)
    {
        if (delay > 0f)
            yield return new WaitForSecondsRealtime(delay);

        if (feedbackGO == null) yield break;

        CanvasGroup cg = feedbackGO.GetComponent<CanvasGroup>();
        if (cg == null) cg = feedbackGO.AddComponent<CanvasGroup>();

        cg.alpha = 1f;

        Vector2 startPos = feedbackGO.GetComponent<RectTransform>().anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0, riseDistance);

        float elapsed = 0f;
        while (elapsed < feedbackDuration)
        {
            if (feedbackGO == null) yield break;
            elapsed += Time.unscaledDeltaTime;

            float t = elapsed / feedbackDuration;

            feedbackGO.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            cg.alpha = 1f - t;

            yield return null;
        }

        if (feedbackGO != null)
            Destroy(feedbackGO);
    }

    private void UpdateDisplay(int Combo)
    {
        if (!scoreDisplayEnabled) return;

        if (scoreText != null)
            scoreText.text = currentScore.ToString("D6");

        if (StreakImage != null && streakSprites.Length > 0)
        {
            if (Combo > 1 && Combo <= streakSprites.Length)
            {
                StreakImage.sprite = streakSprites[Combo - 1];
                StreakImage.gameObject.SetActive(true);
            }
            else
            {
                StreakImage.gameObject.SetActive(false);
            }
        }
    }

    public int GetCurrentScore() => currentScore;
    public void disableScoreDisplay() { if (scoreText) scoreText.gameObject.SetActive(false); if (StreakImage) StreakImage.gameObject.SetActive(false); }
    public void enableScoreDisplay() { if (scoreText) scoreText.gameObject.SetActive(true); if (StreakImage) StreakImage.gameObject.SetActive(true); }
    public void ResetScore() { currentScore = 0; comboCount = 0; totalNotesMissed = 0; totalNotesHit = 0; ResetAttemptStats(); UpdateDisplay(comboCount); }
    public void ResetAttemptStats() { missesThisAttempt = 0; hitsThisAttempt = 0; }
    public int GetMissesThisAttempt() => missesThisAttempt;
    public int GetHitsThisAttempt() => hitsThisAttempt;
    public int GetTotalNotesHit() => totalNotesHit;
    public int GetTotalNotesMissed() => totalNotesMissed;
    public int GetComboCount() => comboCount;
    public bool IsScoreDisplayEnabled() => scoreDisplayEnabled;

    void OnDestroy()
    {
        StopAllCoroutines();

        if (feedbackContainer != null)
        {
            foreach (Transform child in feedbackContainer)
                Destroy(child.gameObject);
        }
    }

    public int CalculateCoins()
    {
        if (isTutorial)
        {
            int misses = GetTotalNotesMissed();
            if (misses == 0) return 3;
            else if (misses >= 1 && misses <= 5) return 2;
            else return 1;
        }
        else
        {
            if (maxScore == 0) return 3;

            float scorePercentage = (float)currentScore / maxScore;
            if (scorePercentage >= 1f) return 3;
            else if (scorePercentage >= 0.66f) return 2;
            else if (scorePercentage >= 0.33f) return 1;
            return 0;
        }
    }

    public void CheckForHighScore()
    {
        if (currentLevel == null) return;

        HighScoresData highScores = SaveSystem.LoadHighScores();

        while (highScores.coins.Count < highScores.levelNames.Count)
            highScores.coins.Add(0);

        int levelIndex = highScores.levelNames.IndexOf(currentLevel.name);
        int coins = CalculateCoins();

        if (levelIndex != -1)
        {
            if (currentScore > highScores.scores[levelIndex])
                highScores.scores[levelIndex] = currentScore;
            if (coins > highScores.coins[levelIndex])
                highScores.coins[levelIndex] = coins;
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