using UnityEngine;

public class ArmsController : MonoBehaviour
{
    public Animator armsAnimator;
    public string armsAnimationStateName = "ArmsAnimation";

    public void PlayArmsAnimation(float speedMultiplier)
    {
        if (armsAnimator != null)
        {
            armsAnimator.SetFloat("StampSpeed", speedMultiplier * 1f);

            armsAnimator.Play(armsAnimationStateName, 0, 0f);
        }
    }
}