using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimingManager : MonoBehaviour
{
    [Header("Game References")]
    public EnvelopeConveyor conveyor;
    public ScoreManager scoreManager;
    public ArmsController armsController;

    [Header("Settings")]
    [Tooltip("Stamped version of the envelope (for player phase).")]
    public Sprite stampedEnvelopeSprite;
    public Sprite stampedEnvelopeBlue;
    [Tooltip("Delay before swapping to stamped version (seconds).")]
    public float stampDelay = 0.3f;

    public bool playerInputEnabled = true;

    private List<Envelope> activeEnvelopesInZone = new List<Envelope>();

    void Update()
    {
        if (playerInputEnabled && Input.GetMouseButtonDown(0))
        {
            if (activeEnvelopesInZone.Count > 0)
            {
                Envelope envelopeToHit = activeEnvelopesInZone[0];

                if (envelopeToHit != null && (envelopeToHit.noteType == NoteType.Tap || envelopeToHit.noteType == NoteType.HalfTap))
                {
                    Debug.Log("HIT on envelope: " + envelopeToHit.noteType);
                    scoreManager.OnNoteHit();

                    if (armsController != null)
                    {
                        const float defaultAnimationDuration = 0.3f;
                        float speedMultiplier = defaultAnimationDuration / envelopeToHit.moveDuration;
                        armsController.PlayArmsAnimation(speedMultiplier);
                    }

                    TriggerSpriteSwap(envelopeToHit);
                    envelopeToHit.needsStampSwap = false;

                    conveyor.ProcessSuccessfulAction(envelopeToHit.gameObject);
                    activeEnvelopesInZone.Remove(envelopeToHit);
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
        Envelope env = other.GetComponent<Envelope>();
        if (env != null && !activeEnvelopesInZone.Contains(env))
        {
            activeEnvelopesInZone.Add(env);
            Debug.Log("Envelope entered hit zone: " + env.name);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Envelope env = other.GetComponent<Envelope>();
        if (env != null && activeEnvelopesInZone.Contains(env))
        {
            if (playerInputEnabled)
            {
                Debug.Log("MISS! (Pressed too late)");
                scoreManager.OnNoteMiss();
            }
            activeEnvelopesInZone.Remove(env);
        }
    }
    public void TriggerSpriteSwap(Envelope env)
    {
        StartCoroutine(SwapSprite(env));
    }

    private IEnumerator SwapSprite(Envelope env)
    {
        yield return new WaitForSeconds(env.moveDuration);

        if (stampedEnvelopeSprite != null && stampedEnvelopeBlue != null)
        {
            var sr = env.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                if (env.noteType == NoteType.Tap)
                {
                    sr.sprite = stampedEnvelopeSprite;
                }
                else if (env.noteType == NoteType.HalfTap)
                {
                    sr.sprite = stampedEnvelopeBlue;
                }
            }
        }
    }
}