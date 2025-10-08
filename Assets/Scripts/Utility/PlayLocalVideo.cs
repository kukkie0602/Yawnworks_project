using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class PlayLocalVideo : MonoBehaviour
{
    public string videoFileName = "intro.mp4"; // Make sure this matches your video's name
    private VideoPlayer videoPlayer;

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        // The path to StreamingAssets is different depending on the platform.
        // This code sets the correct path for any platform.
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);

        // Optional: Log the path to make sure it's correct
        Debug.Log("Loading video from: " + videoPlayer.url);

        videoPlayer.Play();
    }
}