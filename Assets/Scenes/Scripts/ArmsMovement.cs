using UnityEngine;

public class ArmsController : MonoBehaviour
{
    public Animator armsAnimator; // assign in Inspector

    public void PlayArmsAnimation()
    {
        if (armsAnimator != null)
        {
            // Play the animation from the first frame every time
            armsAnimator.Play("ArmsAnimation", -1, 0f);
        }
    }
}