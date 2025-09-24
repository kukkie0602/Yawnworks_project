using UnityEngine;

public class ArmsController : MonoBehaviour
{
    public Animator armsAnimator;
    public string armsAnimationStateName = "ArmsAnimation";

    public void PlayArmsAnimation(float speedMultiplier)
    {
        if (armsAnimator != null)
        {
            // Set animation speed
            armsAnimator.SetFloat("StampSpeed", speedMultiplier * 1f);

            // Force the animation to restart from the beginning
            armsAnimator.Play(armsAnimationStateName, 0, 0f);
        }
    }
}