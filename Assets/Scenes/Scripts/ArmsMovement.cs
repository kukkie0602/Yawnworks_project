using UnityEngine;

public class ArmsController : MonoBehaviour
{
    public Animator armsAnimator;

    public void PlayArmsAnimation(float speedMultiplier = 1f)
    {
        if (armsAnimator != null)
        {
            armsAnimator.SetFloat("StampSpeed", speedMultiplier*1.4f);
            armsAnimator.SetTrigger("PlayArms");
        }
    }
}