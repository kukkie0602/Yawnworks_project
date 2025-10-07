using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    [Header("StartUI")]
    public Animator countdownAnimator;

    public bool playerInputEnabled = true;

    [Header("Animation")]
    public Animator stampeffectanimator;

    private List<Envelope> activeEnvelopesInZone = new List<Envelope>();
    public bool isLevel2 = false;
    public void ResetManager()
    {
        activeEnvelopesInZone.Clear();
        playerInputEnabled = true;
    }

    void Update()
    {
        if (isLevel2) return;
        if (playerInputEnabled && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (activeEnvelopesInZone.Count > 0)
            {
                Envelope envelopeToHit = activeEnvelopesInZone[0];
                if (armsController != null)
                {
                    armsController.PlayArmsAnimation(1f);
                }
                if (envelopeToHit != null && (envelopeToHit.noteType == NoteType.Tap || envelopeToHit.noteType == NoteType.HalfTap))
                {
                    Debug.Log("HIT on envelope: " + envelopeToHit.noteType);
                    scoreManager.OnNoteHit();
                    TriggerGoodStamp();
                    TriggerSpriteSwap(envelopeToHit);
                    envelopeToHit.needsStampSwap = false;

                    conveyor.ProcessSuccessfulAction(envelopeToHit.gameObject);
                    activeEnvelopesInZone.Remove(envelopeToHit);
                }
                else if (envelopeToHit != null && (envelopeToHit.noteType == NoteType.TapStamped || envelopeToHit.noteType == NoteType.HalfTapStamped))
                {
                    Debug.Log("Faulty Hit on envelope: " + envelopeToHit.noteType);
                    scoreManager.OnNoteMiss();
                    TriggerBadStamp();
                    activeEnvelopesInZone.Remove(envelopeToHit);
                }
            }
            else
            {
                Debug.Log("MISS! (Pressed too early)");
                TriggerBadStamp();
                scoreManager.OnNoteMiss();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Envelope env = other.GetComponent<Envelope>();
        if (env == null) return;

        if (env.noteType == NoteType.SkipOne)
        {
            return;
        }

        if (env != null && !activeEnvelopesInZone.Contains(env))
        {
            activeEnvelopesInZone.Add(env);
            Debug.Log("Envelope entered hit zone: " + env.name);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Envelope env = other.GetComponent<Envelope>();
        if (env == null) return;

        if (env.noteType == NoteType.SkipOne)
        {
            return;
        }

        if (!activeEnvelopesInZone.Contains(env)) return;

        if (playerInputEnabled)
        {
            bool isStamped = env.noteType == NoteType.HalfTapStamped || env.noteType == NoteType.TapStamped;

            if (isStamped)
            {
                Debug.Log("Passed! (Pressed too late)");
            }
            else
            {
                Debug.Log("MISS! (Pressed too late)");
                scoreManager.OnNoteMiss();
                TriggerBadStamp();
            }
        }
        activeEnvelopesInZone.Remove(env);
    }

    public void TriggerSpriteSwap(Envelope env)
    {
        StartCoroutine(SwapSprite(env));
    }

    private IEnumerator SwapSprite(Envelope env)
    {
        yield return new WaitForSecondsRealtime(0.45f);

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

    public void TriggerGoodStamp(float delay = 0.23f)
    {
        StartCoroutine(PlayGoodStampWithDelay(delay));
    }

    public void TriggerBadStamp(float delay = 0.23f)
    {
        StartCoroutine(PlayBadStampWithDelay(delay));
    }

    private IEnumerator PlayGoodStampWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        stampeffectanimator.SetTrigger("GoodStamp");
    }
    private IEnumerator PlayBadStampWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        stampeffectanimator.SetTrigger("BadStamp");
    }
}