using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnvelopeConveyor : MonoBehaviour
{
    [Header("Level Data")]
    public BeatmapData currentBeatmap;

    [Header("Conveyor Setup")]
    public Transform[] envelopePositions;
    public float moveDuration = 0.2f;
    public NotePrefabMapping[] noteMappings;

    [Tooltip("The prefab for an envelope after it has been successfully stamped.")]
    public GameObject stampedEnvelopePrefab;

    [Header("Game References")]
    public ScoreManager scoreManager;
    public AudioSource audioSource;

    private Dictionary<NoteType, GameObject> notePrefabDict;
    private List<GameObject> activeEnvelopes = new List<GameObject>();
    private int beatmapIndex = 0;
    private float songStartTime;
    private bool songStarted = false;

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
            Debug.LogError("No Beatmap Loaded!");
            return;
        }

        audioSource.clip = currentBeatmap.songClip;
        audioSource.Play();
        songStartTime = Time.time;
        songStarted = true;

        InitializeConveyor();
    }

    void InitializeConveyor()
    {
        for (int i = 0; i < envelopePositions.Length; i++)
        {
            GameObject prefabToSpawn;
            NoteData noteDataForThisSlot = null;

            if (i >= 5)
            {
                prefabToSpawn = stampedEnvelopePrefab;
            }
            else
            {
                if (beatmapIndex < currentBeatmap.notes.Length)
                {
                    noteDataForThisSlot = currentBeatmap.notes[beatmapIndex];
                    notePrefabDict.TryGetValue(noteDataForThisSlot.noteType, out prefabToSpawn);
                    beatmapIndex++;
                }
                else
                {
                    prefabToSpawn = stampedEnvelopePrefab;
                }
            }

            if (prefabToSpawn != null)
            {
                GameObject newEnvelope = Instantiate(prefabToSpawn, envelopePositions[i].position, Quaternion.identity);
                if (noteDataForThisSlot != null)
                {
                    newEnvelope.GetComponent<Envelope>().noteType = noteDataForThisSlot.noteType;
                }
                activeEnvelopes.Add(newEnvelope);
            }
        }
    }

    void Update()
    {
        if (!songStarted || scoreManager == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (activeEnvelopes.Count > 4)
            {
                GameObject centerEnvelope = activeEnvelopes[4];
                if (centerEnvelope != null)
                {
                    Envelope env = centerEnvelope.GetComponent<Envelope>();

                    if (env.noteType == NoteType.Tap)
                    {
                        Debug.Log("Correctly Tapped Envelope!");
                        scoreManager.IncreaseScore();
                        
                        StampEnvelope(centerEnvelope);

                        Invoke("AdvanceConveyor", 0.1f);
                    }
                }
            }
        }
    }

    void StampEnvelope(GameObject envelopeToStamp)
    {
        int index = activeEnvelopes.IndexOf(envelopeToStamp);
        Vector3 position = envelopeToStamp.transform.position;

        activeEnvelopes.Remove(envelopeToStamp);
        Destroy(envelopeToStamp);

        GameObject stampedVersion = Instantiate(stampedEnvelopePrefab, position, Quaternion.identity);

        activeEnvelopes.Insert(index, stampedVersion);
    }

    public void AdvanceConveyor()
    {
        if (activeEnvelopes.Count > 8)
        {
            GameObject envelopeToEnd = activeEnvelopes[8];
            Destroy(envelopeToEnd);
            activeEnvelopes.RemoveAt(8);
        }

        for (int i = activeEnvelopes.Count - 1; i >= 0; i--)
        {
            int newPositionIndex = i + 1;
            if (newPositionIndex < envelopePositions.Length)
            {
                StartCoroutine(MoveEnvelope(activeEnvelopes[i], envelopePositions[newPositionIndex].position));
            }
        }

        SpawnNewEnvelope(0);
    }

    void SpawnNewEnvelope(int positionIndex)
    {
        if (beatmapIndex >= currentBeatmap.notes.Length) return;

        NoteData noteData = currentBeatmap.notes[beatmapIndex];

        if (notePrefabDict.TryGetValue(noteData.noteType, out GameObject prefab))
        {
            GameObject newEnvelope = Instantiate(prefab, envelopePositions[positionIndex].position, Quaternion.identity);
            newEnvelope.GetComponent<Envelope>().noteType = noteData.noteType;
            activeEnvelopes.Insert(positionIndex, newEnvelope);
            beatmapIndex++;
        }
    }

    IEnumerator MoveEnvelope(GameObject envelope, Vector3 targetPosition)
    {
        float time = 0;
        Vector3 startPosition = envelope.transform.position;

        while (time < moveDuration)
        {
            envelope.transform.position = Vector3.Lerp(startPosition, targetPosition, time / moveDuration);
            time += Time.deltaTime;
            yield return null;
        }
        envelope.transform.position = targetPosition;
    }
}