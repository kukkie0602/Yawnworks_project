using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnvelopeConveyor : MonoBehaviour
{
    public BeatmapData currentBeatmap;
    public float hitWindow = 0.15f;
    public Transform[] envelopePositions;
    public float moveDuration = 0.2f;
    public GameObject stampedEnvelopePrefab;
    public NotePrefabMapping[] noteMappings;

    public EndGamePanel EndGamePanelManger;
    public TimingIndicatorSpawner indicatorSpawner;
    public Transform targetIndicatorTransform;
    public GameObject pauseMenuButton;

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
            Debug.LogError("No Beatmap Loaded! Assign one in the Inspector.");
            return;
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
            float noteLeaveTime = currentBeatmap.notes[conveyorMoveIndex].timestamp + hitWindow;

            if (songPosition >= noteLeaveTime)
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
        yield return new WaitForSeconds(2f);

        Debug.Log("Level complete! Showing end screen.");
        if (EndGamePanelManger.endGamePanel != null)
        {
            EndGamePanelManger.End();
            pauseMenuButton.SetActive(false);
        }
        songStarted = false;
    }

    public void ProcessSuccessfulAction(GameObject envelope)
    {
        StampEnvelope(envelope);
        AdvanceConveyor();
        conveyorMoveIndex++;
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
        Envelope env = envelopeToStamp.GetComponent<Envelope>();
        if (env != null)
        {
            env.needsStampSwap = true;
        }
    }

    public void AdvanceConveyor()
    {
        if (activeEnvelopes.Count > 8 && activeEnvelopes[8] != null)
        {
            Destroy(activeEnvelopes[8]);
        }
        if (activeEnvelopes.Count > 8) activeEnvelopes.RemoveAt(8);

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

        StartCoroutine(SwapStampedAfterMove());

        SpawnNewEnvelope(0);
    }

    IEnumerator SwapStampedAfterMove()
    {
        yield return new WaitForSeconds(moveDuration);

        for (int i = 0; i < activeEnvelopes.Count; i++)
        {
            if (activeEnvelopes[i] == null) continue;

            Envelope env = activeEnvelopes[i].GetComponent<Envelope>();
            if (env != null && env.needsStampSwap)
            {
                Vector3 pos = activeEnvelopes[i].transform.position;
                Quaternion rot = activeEnvelopes[i].transform.rotation;

                Destroy(activeEnvelopes[i]);
                GameObject stamped = Instantiate(stampedEnvelopePrefab, pos, rot);
                activeEnvelopes[i] = stamped;
            }
        }
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
