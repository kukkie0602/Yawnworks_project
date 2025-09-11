using UnityEngine;
using System.Collections.Generic;

public class TimingManager : MonoBehaviour
{
    [Header("Game References")]
    public EnvelopeConveyor conveyor;
    public ScoreManager scoreManager;
    public Animator armsAnimator;
    private List<TimingIndicator> activeIndicators = new List<TimingIndicator>();

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (activeIndicators.Count > 0)
            {
                TimingIndicator indicatorToHit = activeIndicators[0];
                Envelope centerEnvelope = conveyor.GetCenterEnvelope();

                if (centerEnvelope != null && centerEnvelope.noteType == NoteType.Tap)
                {
                    Debug.Log("HIT!");
                    scoreManager.OnNoteHit();
                    conveyor.ProcessSuccessfulAction(centerEnvelope.gameObject);

                    activeIndicators.Remove(indicatorToHit);
                    Destroy(indicatorToHit.gameObject);
                    if (armsAnimator != null)
                    {
                        armsAnimator.Play("ArmsAnimation", -1, 0f);
                    }
                }
            }
            else
            {
                Debug.Log("MISS! (Pressed too early)");
                scoreManager.OnNoteMiss();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TimingIndicator indicator = other.GetComponent<TimingIndicator>();
        if (indicator != null && !activeIndicators.Contains(indicator))
        {
            activeIndicators.Add(indicator);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        TimingIndicator indicator = other.GetComponent<TimingIndicator>();
        if (indicator != null && activeIndicators.Contains(indicator))
        {
            Debug.Log("MISS! (Pressed too late)");
            scoreManager.OnNoteMiss();
            activeIndicators.Remove(indicator);
        }
    }
}