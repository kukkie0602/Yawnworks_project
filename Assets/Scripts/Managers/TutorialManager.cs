using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Game Components")]
    public EnvelopeConveyor envelopeConveyor;
    public ScoreManager scoreManager;
    public EndGamePanel endGamePanel;
    public TimingManager timingManager;

    [Header("Level Settings")]
    public EnvelopeLevel tutorialLevelData;
    public EnvelopeLevel mainLevelData;

    [Header("UI Elements")]
    public TMP_Text instructionText;
    public Image tutorialImage;

    private EnvelopeSequence tutorialSequence;

    void Start()
    {
        if (tutorialLevelData == null || tutorialLevelData.sequences.Length == 0)
        {
            Debug.LogError("Tutorial Level Data is not assigned!");
            gameObject.SetActive(false);
            return;
        }

        tutorialSequence = tutorialLevelData.sequences[0];
        StartCoroutine(TutorialFlow());
    }

    private IEnumerator TutorialFlow()
    {
        while (true)
        {
            instructionText.text = "Look and <color=#C14520>learn</color> the rhythm...";
            timingManager.playerInputEnabled = false;
            yield return new WaitForSeconds(1.0f);
            yield return StartCoroutine(envelopeConveyor.PlayTutorialSequence(tutorialSequence, true, tutorialLevelData.beatsPerMinute));

            instructionText.text = "Get <color=#C14520>ready</color> to tap!";
            yield return new WaitForSeconds(1.5f);

            instructionText.text = "Now, <color=#C14520>you</color> try...";
            scoreManager.ResetAttemptStats();
            timingManager.playerInputEnabled = true;
            yield return StartCoroutine(envelopeConveyor.PlayTutorialSequence(tutorialSequence, false, tutorialLevelData.beatsPerMinute));

            yield return new WaitForSeconds(1.0f);

            int hits = scoreManager.GetHitsThisAttempt();
            int misses = scoreManager.GetMissesThisAttempt();
            int requiredHits = tutorialSequence.pattern.Length;

            if (misses == 0 && hits >= requiredHits)
            {
                instructionText.text = "Well done!";
                yield return new WaitForSeconds(1.5f);
                break;
            }
            else
            {
                instructionText.text = "Oops, let's try that again!";
                yield return new WaitForSeconds(2.0f);
            }
        }

        timingManager.playerInputEnabled = false;
        instructionText.text = "Here we go!";

        yield return new WaitForSeconds(1.5f);

        scoreManager.ResetScore();
        envelopeConveyor.StartMainLevel(mainLevelData);

        tutorialImage.gameObject.SetActive(false);
        gameObject.SetActive(false); 
    }
}