using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource;

    public void PlayScheduled(double dspStartTime)
    {
        if (musicSource == null) return;
        musicSource.playOnAwake = false;
        musicSource.PlayScheduled(dspStartTime);
    }

    public void Stop()
    {
        if (musicSource != null)
            musicSource.Stop();
    }
}
