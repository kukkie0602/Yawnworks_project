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
                armsController.PlayArmsAnimation(1f);
            }

            if (timingManager != null)
            {
                timingManager.TriggerGoodStamp();
                timingManager.TriggerSpriteSwap(env);
            }

            env.needsStampSwap = false;
        }
    }
}
