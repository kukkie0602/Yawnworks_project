using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource;

    public void PlayWithDelay(double delaySeconds)
    {
        if (musicSource != null)
        {
            musicSource.playOnAwake = false;
            musicSource.PlayScheduled(AudioSettings.dspTime + delaySeconds);
        }
    }
}
