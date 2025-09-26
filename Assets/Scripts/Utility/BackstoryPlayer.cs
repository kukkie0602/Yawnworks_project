using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

public class BackstoryPlayer : MonoBehaviour
{
    [Header("Video Settings")]
    public VideoPlayer videoPlayer;
    public string nextSceneName = "LevelSelectScene";

    [Header("UI")]
    public GameObject skipButton;

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (skipButton != null)
        {
            skipButton.SetActive(false);
        }

        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.loopPointReached += OnVideoFinished;

        videoPlayer.Prepare();
    }

    void OnVideoPrepared(VideoPlayer source)
    {
        videoPlayer.Play();

        if (skipButton != null)
        {
            skipButton.SetActive(true);
        }
    }

    void OnVideoFinished(VideoPlayer source)
    {
        LoadNextScene();
    }

    public void SkipVideo()
    {
        videoPlayer.Stop();
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}