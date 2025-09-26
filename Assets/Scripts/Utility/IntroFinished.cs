using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI; 

public class IntroFinished : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Image blackCover;
    public string nextSceneName = "MainMenuScene";

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        blackCover.color = Color.black;
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.loopPointReached += OnVideoFinished;

        videoPlayer.Prepare();
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        videoPlayer.Play();
        StartCoroutine(FadeOutCover());
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FadeOutCover()
    {
        float fadeDuration = 0.5f; 
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = 1.0f - (timer / fadeDuration);
            blackCover.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        blackCover.color = new Color(0, 0, 0, 0);
        blackCover.gameObject.SetActive(false);
    }
}