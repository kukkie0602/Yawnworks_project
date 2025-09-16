using UnityEngine;
using System.Collections;
public class StampTriggerZone : MonoBehaviour
{
    public ArmsController armsController;
    [Tooltip("The stamped version of the envelope prefab (optional).")]
    public Sprite stampedEnvelopeSprite;

    [Tooltip("Delay before swapping to stamped version (seconds).")]
    public float stampDelay = 0.3f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Envelope env = other.GetComponent<Envelope>();
        if (env != null && env.needsStampSwap)
        {
            if (armsController != null)
                armsController.PlayArmsAnimation();
            StartCoroutine(SwapSprite(env));
            env.needsStampSwap = false;
        }
    }
    private IEnumerator SwapSprite(Envelope env)
    {
        yield return new WaitForSeconds(stampDelay);

        if (stampedEnvelopeSprite != null)
        {
            var sr = env.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sprite = stampedEnvelopeSprite;
        }
    }
}