using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class NoteSpawner : MonoBehaviour
{
    public GameObject notePrefab;

    public AudioSource audioSource;

    public float noteTravelTime = 2.922f;

    public float countdownTime = 3.0f;

    public GameObject endGamePanel;
    public GameObject scoreText;

    private bool songFinished = false;

    public GameObject[] notePrefabs;

    private float[] beatTimestamps = new float[]
    {
        0.7707f, 1.3307f, 2.0907f, 3.5200f, 4.1707f, 4.8320f, 6.3200f, 7.0107f, 7.7200f, 9.1307f, 9.8107f, 10.5307f, 11.2400f, 11.5920f, 12.0000f, 12.6800f, 13.4507f, 13.8400f, 14.1600f, 14.5200f, 14.9013f, 15.5413f, 16.3200f, 16.6507f, 17.0000f, 17.3413f, 17.7200f, 18.4213f, 19.1200f, 19.4907f, 19.7707f, 20.1520f, 20.5520f, 21.2800f, 21.9813f, 22.3307f, 22.6613f, 23.0213f, 23.3600f, 24.0613f, 24.7600f, 25.1013f, 25.4800f, 25.8613f, 26.2000f, 26.8400f, 27.6320f, 27.9307f, 28.2400f, 28.6613f, 29.0000f, 29.6800f, 30.3813f, 30.7307f, 31.0907f, 31.4213f, 31.8213f, 32.5200f, 33.2400f, 33.5920f, 33.9307f, 34.2800f, 34.6400f, 35.3707f, 36.0907f, 36.4213f, 36.7600f, 37.1307f, 37.4720f, 38.1600f, 38.9413f, 39.2720f, 39.5707f, 39.9120f, 40.2907f, 40.9707f, 41.6907f, 42.0400f, 42.4000f, 42.7413f, 43.1200f, 43.8000f, 44.5120f, 44.8613f, 45.2507f, 45.5520f, 45.9200f, 46.6213f, 47.3707f, 47.7013f, 48.0213f, 48.4107f, 48.7600f, 49.4213f, 50.1920f, 50.5307f, 50.8613f, 51.2107f, 51.5813f, 52.2320f, 52.9600f, 53.2907f, 53.6320f, 54.0320f, 54.3920f, 55.1120f, 55.8213f, 56.1707f, 56.5200f, 56.8907f, 57.2613f, 57.8800f, 58.6400f, 58.9813f, 59.3413f, 59.6800f, 60.0800f, 60.7707f, 61.4800f, 61.8213f, 62.1707f, 62.5013f, 62.8720f, 63.5307f, 64.2507f, 64.5920f, 65.0213f, 65.3413f, 65.7200f, 66.3707f, 67.0907f, 67.4107f, 67.7707f, 68.1307f, 68.5120f, 69.1520f, 69.8907f, 70.2507f, 70.6213f, 71.0107f, 71.3813f, 72.0800f, 72.7920f, 73.1520f, 73.4507f, 73.8107f, 74.1600f, 74.8000f, 75.5707f, 75.9200f, 76.2613f, 76.6320f, 76.9600f, 77.6507f, 78.4720f, 78.8000f, 79.1120f, 79.4720f, 79.8107f, 80.4800f, 81.1920f, 81.5920f, 81.9413f, 82.3413f, 82.7013f, 83.3520f, 84.0800f, 84.4400f, 84.7200f, 85.0907f, 85.4507f, 86.1413f, 86.8613f, 87.2107f, 87.5520f, 87.9200f, 88.2400f, 88.9813f, 89.7600f, 90.0800f, 90.3920f, 90.7920f, 91.1013f, 91.7600f, 92.4907f, 92.8507f, 93.1920f, 93.6000f, 93.9413f, 94.5813f, 95.2800f, 95.6907f, 96.0613f, 96.4320f, 96.7600f, 97.3707f, 98.1120f, 98.4800f, 98.8507f, 99.2320f, 99.5813f, 100.2400f, 101.0000f, 101.3200f, 101.6400f, 101.9920f, 102.4000f, 103.1013f, 103.8213f, 104.1413f, 104.5120f, 104.8907f, 105.2507f, 105.9307f, 106.6907f, 107.0320f, 107.3520f, 107.6800f, 108.0213f, 108.6907f, 109.3920f, 109.7600f, 110.0907f, 110.5013f, 110.8720f, 111.5520f, 112.3200f, 112.6400f, 112.9813f, 113.3600f, 113.6907f, 114.3920f, 115.1413f, 115.4613f, 115.8000f, 116.1413f, 116.5707f, 117.1707f, 117.9600f, 118.2800f, 118.6000f, 118.9307f, 119.3520f, 119.9813f, 120.7520f, 121.0800f, 121.4400f, 121.8107f, 122.1813f, 122.8400f, 123.5920f, 123.9413f, 124.2400f, 124.6613f, 124.9813f, 125.6000f, 126.3813f, 126.7600f, 127.1120f, 127.4907f, 127.8613f, 128.4800f, 129.2320f, 129.5920f, 129.8907f, 130.2507f, 130.6107f, 131.2720f, 131.9920f, 132.3520f, 132.7520f, 133.1200f, 133.4907f, 134.1813f, 134.9200f, 135.2320f, 135.5707f, 135.9307f, 136.2800f, 136.9307f, 137.7120f, 138.0507f, 138.4000f, 138.7413f, 139.1200f, 139.7920f, 140.5013f, 140.8720f, 141.2400f, 141.6000f, 141.9600f, 142.6107f, 143.4000f, 143.7200f, 144.0720f, 144.4400f, 144.7707f, 145.4213f, 146.1520f, 146.5013f, 146.8320f, 147.2320f, 147.5813f, 148.2507f, 149.0213f, 149.3600f, 149.7307f, 150.0907f, 150.4613f, 151.0800f, 151.7813f, 152.1200f, 152.4800f, 152.8507f, 153.2213f, 153.9307f, 154.6507f, 155.0000f, 155.3200f, 155.6613f, 156.0507f, 156.7520f, 157.4320f, 157.7813f, 158.1307f, 158.5200f, 158.8720f, 159.5013f, 160.2000f, 160.6400f, 160.9600f, 161.3200f, 161.7120f, 162.3707f, 163.1013f, 163.4720f, 163.8000f, 164.1707f, 164.5520f, 165.1707f, 165.9413f, 166.2800f, 166.6213f, 167.0400f, 167.5520f, 168.0800f, 168.8213f,
    };

    private int timestampIndex = 0;
    private double songStartTimeDSP;

    private double pauseStartedTimeDSP = 0;

    void Start()
    {
        if (endGamePanel != null) { endGamePanel.SetActive(false); }
        songStartTimeDSP = AudioSettings.dspTime + countdownTime;
        audioSource.PlayScheduled(songStartTimeDSP);
    }

    void Update()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        float songPosition = (float)(AudioSettings.dspTime - songStartTimeDSP);

        if (timestampIndex < beatTimestamps.Length)
        {
            if (songPosition >= (beatTimestamps[timestampIndex] - noteTravelTime))
            {
                SpawnNote();
                timestampIndex++;
            }
        }

        if (songPosition > 0 && !audioSource.isPlaying && timestampIndex >= beatTimestamps.Length && !songFinished)
        {
            songFinished = true;
            StartCoroutine(ShowEndGameOptions());
        }
    }

    public void OnGamePaused()
    {
        pauseStartedTimeDSP = AudioSettings.dspTime;
    }

    public void OnGameResumed()
    {
        double pausedDuration = AudioSettings.dspTime - pauseStartedTimeDSP;
        songStartTimeDSP += pausedDuration;
    }

    //Randomly spawn one of the note prefabs
    void SpawnNote()
    {
        int randomIndex = Random.Range(0, notePrefabs.Length);
        GameObject noteToSpawn = notePrefabs[randomIndex];

        GameObject noteObject = Instantiate(noteToSpawn, transform.position, Quaternion.identity);
        NoteController noteController = noteObject.GetComponent<NoteController>();
    }
    //void SpawnNote()
    //{
    //    Instantiate(notePrefab, transform.position, Quaternion.identity);
    //}

    IEnumerator ShowEndGameOptions()
    {
        Debug.Log("Song has finished. Waiting 5 seconds...");
        yield return new WaitForSeconds(2f);

        Debug.Log("Showing end game options panel.");
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);
            scoreText.SetActive(false);
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void QuitGame()
    {
        Debug.Log("Player has quit the game.");
        Application.Quit();
    }
}
