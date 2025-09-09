using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnvelopeConveyor : MonoBehaviour
{
    public BeatmapData currentBeatmap;

    public Transform[] envelopePositions;
    public float moveDuration = 0.2f;
    public GameObject stampedEnvelopePrefab;
    public NotePrefabMapping[] noteMappings;

    public GameObject endGamePanel;
    public TimingIndicatorSpawner indicatorSpawner;
    public Transform targetIndicatorTransform;

    public AudioSource audioSource;

    private Dictionary<NoteType, GameObject> envelopePrefabDict;
    private Dictionary<NoteType, GameObject> indicatorPrefabDict;
    private List<GameObject> activeEnvelopes = new List<GameObject>();

    private int beatmapIndex = 0;
    private int conveyorMoveIndex = 0;
    private int indicatorSpawnIndex = 0;

    private float indicatorTravelTime;
    private float songStartTime;
    private bool songStarted = false;

    private bool endGameSequenceStarted = false;


    void Awake()
    {
        envelopePrefabDict = new Dictionary<NoteType, GameObject>();
        indicatorPrefabDict = new Dictionary<NoteType, GameObject>();
        foreach (var mapping in noteMappings)
        {
            envelopePrefabDict[mapping.noteType] = mapping.envelopePrefab;
            indicatorPrefabDict[mapping.noteType] = mapping.indicatorPrefab;
        }
    }

    void Start()
    {
        if (currentBeatmap == null)
        {
            Debug.LogError("No Beatmap Loaded! Assign one in the Inspector or select it from the menu.");
            return;
        }

        if (endGamePanel != null)
        {
            endGamePanel.SetActive(false);
        }

        CalculateIndicatorTravelTime();

        audioSource.clip = currentBeatmap.songClip;
        audioSource.Play();
        songStartTime = Time.time;

        InitializeConveyor();
        songStarted = true;

    }

    void Update()
    {
        if (!songStarted) return;

        float songPosition = Time.time - songStartTime;

        SpawnTimingIndicators(songPosition);

        if (conveyorMoveIndex < currentBeatmap.notes.Length)
        {
            if (songPosition >= currentBeatmap.notes[conveyorMoveIndex].timestamp)
            {
                AdvanceConveyor();
                conveyorMoveIndex++;
            }
        }

        if (!endGameSequenceStarted && currentBeatmap.notes.Length > 0)
        {
            float lastNoteTimestamp = currentBeatmap.notes[currentBeatmap.notes.Length - 1].timestamp;

            if (songPosition >= lastNoteTimestamp)
            {
                endGameSequenceStarted = true;

                Debug.Log("Last note time has passed. Starting 5-second end-game timer.");
                StartCoroutine(EndGameCoroutine());
            }
        }
    }

    IEnumerator EndGameCoroutine()
    {
        audioSource.Stop();
        yield return new WaitForSeconds(5f);

        Debug.Log("Level complete! Showing end screen.");
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);
        }
        songStarted = false;
    }

    public void ProcessSuccessfulAction(GameObject envelope)
    {
        StampEnvelope(envelope);
    }

    public Envelope GetCenterEnvelope()
    {
        if (activeEnvelopes.Count > 4 && activeEnvelopes[4] != null)
        {
            return activeEnvelopes[4].GetComponent<Envelope>();
        }
        return null;
    }

    void InitializeConveyor()
    {
        for (int i = 0; i < envelopePositions.Length; i++)
        {
            if (i >= 5)
            {
                if (stampedEnvelopePrefab != null)
                {
                    GameObject stampedEnv = Instantiate(stampedEnvelopePrefab, envelopePositions[i].position, Quaternion.identity);
                    activeEnvelopes.Add(stampedEnv);
                }
            }
            else
            {
                SpawnNewEnvelope(i);
            }
        }
    }

    void StampEnvelope(GameObject envelopeToStamp)
    {
        if (stampedEnvelopePrefab == null) return;
        int index = activeEnvelopes.IndexOf(envelopeToStamp);
        if (index == -1) return;
        Vector3 position = envelopeToStamp.transform.position;

        activeEnvelopes.RemoveAt(index);
        Destroy(envelopeToStamp);

        GameObject stampedVersion = Instantiate(stampedEnvelopePrefab, position, Quaternion.identity);
        activeEnvelopes.Insert(index, stampedVersion);
    }

    public void AdvanceConveyor()
    {
        if (activeEnvelopes.Count > 8 && activeEnvelopes[8] != null)
        {
            Destroy(activeEnvelopes[8]);
        }
        activeEnvelopes.RemoveAt(8);

        for (int i = activeEnvelopes.Count - 1; i >= 0; i--)
        {
            if (activeEnvelopes[i] != null)
            {
                int newPositionIndex = i + 1;
                if (newPositionIndex < envelopePositions.Length)
                {
                    StartCoroutine(MoveEnvelope(activeEnvelopes[i], envelopePositions[newPositionIndex].position));
                }
            }
        }

        SpawnNewEnvelope(0);
    }

    void SpawnNewEnvelope(int positionIndex)
    {
        if (beatmapIndex < currentBeatmap.notes.Length)
        {
            NoteData noteData = currentBeatmap.notes[beatmapIndex];
            if (envelopePrefabDict.TryGetValue(noteData.noteType, out GameObject prefab))
            {
                GameObject newEnvelope = Instantiate(prefab, envelopePositions[positionIndex].position, Quaternion.identity);
                newEnvelope.GetComponent<Envelope>().noteType = noteData.noteType;
                activeEnvelopes.Insert(positionIndex, newEnvelope);
            }
            beatmapIndex++;
        }
        else
        {
            activeEnvelopes.Insert(positionIndex, null);
        }
    }

    void SpawnTimingIndicators(float songPosition)
    {
        if (indicatorSpawnIndex < currentBeatmap.notes.Length)
        {
            NoteData nextIndicatorNote = currentBeatmap.notes[indicatorSpawnIndex];
            float spawnTime = nextIndicatorNote.timestamp - indicatorTravelTime;
            if (songPosition >= spawnTime)
            {
                if (indicatorSpawner != null && indicatorPrefabDict.TryGetValue(nextIndicatorNote.noteType, out GameObject prefabToSpawn))
                {
                    indicatorSpawner.SpawnIndicator(prefabToSpawn);
                }
                indicatorSpawnIndex++;
            }
        }
    }

    void CalculateIndicatorTravelTime()
    {
        if (indicatorSpawner == null || targetIndicatorTransform == null) return;
        if (indicatorPrefabDict.TryGetValue(NoteType.Tap, out GameObject sampleIndicatorPrefab))
        {
            TimingIndicator indicatorScript = sampleIndicatorPrefab.GetComponent<TimingIndicator>();
            if (indicatorScript != null)
            {
                float distance = Mathf.Abs(targetIndicatorTransform.position.x - indicatorSpawner.transform.position.x);
                float speed = indicatorScript.speed;
                if (speed > 0) indicatorTravelTime = distance / speed;
            }
        }
    }

    IEnumerator MoveEnvelope(GameObject envelope, Vector3 targetPosition)
    {
        float time = 0;
        Vector3 startPosition = envelope.transform.position;
        while (time < moveDuration)
        {
            if (envelope == null) yield break;
            envelope.transform.position = Vector3.Lerp(startPosition, targetPosition, time / moveDuration);
            time += Time.deltaTime;
            yield return null;
        }
        if (envelope != null) envelope.transform.position = targetPosition;
    }
}