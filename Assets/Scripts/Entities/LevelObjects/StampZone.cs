using UnityEngine;
using System.Collections;
public class StampTriggerZone : MonoBehaviour
{
    public ArmsController armsController;
    [Tooltip("The stamped version of the envelope prefab (optional).")]
    public Sprite stampedEnvelopeSprite;
    public Sprite stampedEnvelopeBlue;

    [Tooltip("Delay before swapping to stamped version (seconds).")]
    public float stampDelay = 0.3f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Envelope env = other.GetComponent<Envelope>();
        stampDelay = env.moveDuration;
        if (env != null && env.needsStampSwap && env.noteType != NoteType.SkipOne)
        {
            if (armsController != null)
            {
                float defaultAnimationDuration = 0.3f;
                float speedMultiplier = defaultAnimationDuration / env.moveDuration;
                armsController.PlayArmsAnimation(speedMultiplier);
            }

            StartCoroutine(SwapSprite(env));
            env.needsStampSwap = false;
        }
    }
    private IEnumerator SwapSprite(Envelope env)
    {
        yield return new WaitForSeconds(stampDelay);
        if (stampedEnvelopeSprite != null && stampedEnvelopeBlue != null)
        {
            var sr = env.GetComponent<SpriteRenderer>();
            if (sr != null)
            {

                if (env.noteType == NoteType.Tap)
                {
                    sr.sprite = stampedEnvelopeSprite;
                    Debug.Log("Hallo");
                }


                else if (env.noteType == NoteType.HalfTap)
                {
                    sr.sprite = stampedEnvelopeBlue;
                    Debug.Log("Hallo");
                }

            }


        }
    }
}