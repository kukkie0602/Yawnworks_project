using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    public AudioSource sfxSource;

    public AudioClip uiClickSound;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayUIClick()
    {
        if (sfxSource == null)
        {
            return; 
        }

        if (uiClickSound == null)
        {
            return;
        }

        if (!sfxSource.enabled)
        {
            Debug.LogWarning("SFXManager WAARSCHUWING: De AudioSource-component staat uitgeschakeld!");
        }
        sfxSource.PlayOneShot(uiClickSound);
    }
}