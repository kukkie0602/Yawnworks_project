using UnityEngine;

public class ArmsController : MonoBehaviour
{
    public Animator armsAnimator;

    public void PlayArmsAnimation()
    {
        if (armsAnimator != null)
        {
            armsAnimator.SetTrigger("PlayArms");
        }
    }
}