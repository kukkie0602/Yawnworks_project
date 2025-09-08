using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class NotePrefabMapping
{
    public NoteType noteType;
    public GameObject prefab;
}

public class NoteSpawner : MonoBehaviour
{
    [Header("Beatmap Data")]
    public BeatmapData currentBeatmap;

    [Header("Note Prefab Mappings")]

    public NotePrefabMapping[] noteMappings;

    [Header("Core Components")]
    public AudioSource audioSource;
    public GameObject endGamePanel;
    public GameObject scoreText;

    [Header("Timing Configuration")]
    public float noteTravelTime = 2.922f;
    public float countdownTime = 3.1f;

    private Dictionary<NoteType, GameObject> notePrefabDict;

    private int timestampIndex = 0;
    private double songStartTimeDSP;
    private double pauseStartedTimeDSP = 0;
    private bool songFinished = false;

    void Awake()
    {
        notePrefabDict = new Dictionary<NoteType, GameObject>();
        foreach (var mapping in noteMappings)
        {
            notePrefabDict[mapping.noteType] = mapping.prefab;
        }
    }

    void Start()
    {
        if (currentBeatmap == null)
        {
            Debug.LogError("No BeatmapData assigned!");
            return;
        }

        audioSource.clip = currentBeatmap.songClip;

        if (endGamePanel != null) { endGamePanel.SetActive(false); }
        songStartTimeDSP = AudioSettings.dspTime + countdownTime;
        audioSource.PlayScheduled(songStartTimeDSP);
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        if (timestampIndex < currentBeatmap.notes.Length)
        {
            NoteData nextNoteData = currentBeatmap.notes[timestampIndex];
            float songPosition = (float)(AudioSettings.dspTime - songStartTimeDSP);

            if (songPosition >= (nextNoteData.timestamp - noteTravelTime))
            {
                SpawnNote(nextNoteData);
                timestampIndex++;
            }
        }
        else
        {
            float songPosition = (float)(AudioSettings.dspTime - songStartTimeDSP);
            if (songPosition > 0 && !audioSource.isPlaying && !songFinished)
            {
                songFinished = true;
                StartCoroutine(ShowEndGameOptions());
            }
        }
    }
    void SpawnNote(NoteData data)
    {
        if (notePrefabDict.TryGetValue(data.noteType, out GameObject notePrefab))
        {
            GameObject noteObject = Instantiate(notePrefab, transform.position, Quaternion.identity);
            NoteController noteController = noteObject.GetComponent<NoteController>();
            
            if (noteController != null && data.noteType == NoteType.Hold)
            {
                noteController.holdDuration = data.holdDuration;
            }
        }
        else
        {
            Debug.LogWarning("No prefab mapping found for note type: " + data.noteType);
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
