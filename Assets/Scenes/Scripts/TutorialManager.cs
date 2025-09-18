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

    [Header("Tutorial settings")]
    public EnvelopeLevel tutorialLevelData;
    public int targetSuccesses = 5;
    private int currentSuccesses = 0;

    [Header("UI Elements")]
    public TMP_Text instructionText;
    public TMP_Text progressText;
    public Image tutorialImage;

    private EnvelopeSequence tutorialSequence;

    void Start()
    {
        if (tutorialLevelData == null || tutorialLevelData.sequences.Length == 0)
        {
            Debug.LogError("Tutorial Level Data is niet toegewezen of bevat geen sequenties!");
            this.enabled = false;
            return;
        }

        if (timingManager == null)
        {
            Debug.LogError("Timing Manager is niet toegewezen in de TutorialManager!");
            this.enabled = false;
            return;
        }

        tutorialSequence = tutorialLevelData.sequences[0];

        StartCoroutine(TutorialFlow());
    }

    private IEnumerator TutorialFlow()
    {
        while (currentSuccesses < targetSuccesses)
        {
            instructionText.text = "Look and learn the rhythm...";
            progressText.text = $"Succes: {currentSuccesses} / {targetSuccesses}";
            timingManager.playerInputEnabled = false;
            yield return new WaitForSeconds(2.5f);
            yield return StartCoroutine(envelopeConveyor.PlayExamplePhase(tutorialSequence));

            instructionText.text = "Get Ready...";
            yield return new WaitForSeconds(2.5f);

            instructionText.text = "Your turn! Play the rhythm.";
            scoreManager.ResetAttemptStats(); 
            timingManager.playerInputEnabled = true;
            yield return StartCoroutine(envelopeConveyor.PlayPlayerPhase(tutorialSequence));

            yield return new WaitForSeconds(2f);

            int hits = scoreManager.GetHitsThisAttempt();
            int misses = scoreManager.GetMissesThisAttempt();
            int requiredHits = tutorialSequence.pattern.Length;

            if (misses == 0 && hits == requiredHits)
            {
                currentSuccesses++;
                instructionText.text = "Well done!";
                yield return new WaitForSeconds(1.5f);
            }
            else
            {
                instructionText.text = "Oops, try again!";
                yield return new WaitForSeconds(1.5f);
            }
        }

        timingManager.playerInputEnabled = false;
        instructionText.text = "Tutorial completed!";
        progressText.text = $"Succes: {currentSuccesses} / {targetSuccesses}";

        if (endGamePanel != null)
        {
            endGamePanel.End();
        }

        endGamePanel.finalScoreText.text = progressText.text;
        endGamePanel.endGameMessageText.text = instructionText.text;
        tutorialImage.gameObject.SetActive(false);
    }
}