using UnityEngine;
using System.Collections;

public class StampTriggerZone : MonoBehaviour
{
    public ArmsController armsController;
    public TimingManager timingManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Envelope env = other.GetComponent<Envelope>();

        if (env != null && env.needsStampSwap && env.noteType != NoteType.SkipOne)
        {
            if (armsController != null)
            {
                const float defaultAnimationDuration = 0.3f;
                float speedMultiplier = defaultAnimationDuration / env.moveDuration;
                armsController.PlayArmsAnimation(speedMultiplier);
            }

            if (timingManager != null)
            {
                timingManager.TriggerSpriteSwap(env);
            }

            env.needsStampSwap = false;
        }
    }

}