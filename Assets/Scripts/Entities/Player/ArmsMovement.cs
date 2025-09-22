using UnityEngine;

public class ArmsController : MonoBehaviour
{
    public Animator armsAnimator;

    public void PlayArmsAnimation(float speedMultiplier)
    {
        if (armsAnimator != null)
        {
            armsAnimator.SetFloat("StampSpeed", speedMultiplier);
            armsAnimator.SetTrigger("PlayArms");
        }
    }
}