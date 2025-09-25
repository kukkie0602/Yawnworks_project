using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvelopeConveyor : MonoBehaviour
{
    [Header("Mode")]
    [Tooltip("Turn this on if the scene is controlled by the tutorial manager.")]
    public bool isTutorialMode = false;

    [Header("Level Data")]
    public EnvelopeLevel levelData;

    [Header("UI References")]
    public ArmsController armsController;
    public TimingManager timingManager;
    public EndGamePanel endGameManger;

    [Header("BPM Synchronization")]
    [Tooltip("The index in envelopePositions that aligns with the center of the hit zone.")]
    public int hitZonePositionIndex = 4; 
    [Tooltip("How many beats it should take for an envelope to travel from spawn to the hit zone.")]
    public float beatsToHitZone = 4f;

    [Header("Prefabs & References")]
    public Transform[] envelopePositions;
    [Range(0.1f, 2f)]
    
    public GameObject stampedEnvelopePrefab;
    public NotePrefabMapping[] noteMappings;
    public Animator armsAnimator;
    public AudioManager audioManager;
    private Dictionary<NoteType, GameObject> envelopePrefabDict;
    private List<GameObject> activeEnvelopes = new List<GameObject>();

    private int sequenceIndex = 0;

    void Start()
    {
        audioManager.PlayWithDelay(1.4);
        envelopePrefabDict = new Dictionary<NoteType, GameObject>();
        foreach (var mapping in noteMappings)
            envelopePrefabDict[mapping.noteType] = mapping.envelopePrefab;

        if (!isTutorialMode)
        {
            if (levelData == null || levelData.sequences.Length == 0)
            {
                Debug.LogError("No level assigned or empty level data!");
                return;
            }
            if (timingManager == null)
            {
                Debug.LogError("Timing Manager is nog assigned in the EnvelopeConveyor!");
                this.enabled = false;
                return;
            }
            StartCoroutine(PlaySequenceCoroutine());
        }
    }

    IEnumerator PlaySequenceCoroutine()
    {
        if (levelData.beatsPerMinute <= 0)
        {
            Debug.LogError("BPM must be greater than 0!");
            yield break;
        }
        if (hitZonePositionIndex <= 0)
        {
            Debug.LogError("Hit Zone Position Index must be greater than 0!");
            yield break;
        }

        float beatInterval = 60f / levelData.beatsPerMinute;
        float totalTravelTime = beatsToHitZone * beatInterval;
        float dynamicMoveDuration = totalTravelTime / hitZonePositionIndex;

        while (sequenceIndex < levelData.sequences.Length)
        {
            var seq = levelData.sequences[sequenceIndex];
            timingManager.playerInputEnabled = true;

            yield return StartCoroutine(SpawnAndAnimateSequence(seq, autoStamp: false, beatInterval, dynamicMoveDuration));

            sequenceIndex++;
        }

        while (activeEnvelopes.Count > 0)
        {
            yield return null;
        }

        Debug.Log("Level Complete!");
        timingManager.playerInputEnabled = false;
        endGameManger.End();
    }


    public IEnumerator PlayExamplePhase(EnvelopeSequence seq)
    {
        float beatInterval = 60f / levelData.beatsPerMinute;
        float totalTravelTime = beatsToHitZone * beatInterval;
        float dynamicMoveDuration = totalTravelTime / hitZonePositionIndex;
        yield return StartCoroutine(SpawnAndAnimateSequence(seq, autoStamp: true, beatInterval, dynamicMoveDuration));
    }

    public IEnumerator PlayPlayerPhase(EnvelopeSequence seq)
    {
        float beatInterval = 60f / levelData.beatsPerMinute;
        float totalTravelTime = beatsToHitZone * beatInterval;
        float dynamicMoveDuration = totalTravelTime / hitZonePositionIndex;
        yield return StartCoroutine(SpawnAndAnimateSequence(seq, autoStamp: false, beatInterval, dynamicMoveDuration));
    }

    IEnumerator SpawnAndAnimateSequence(EnvelopeSequence seq, bool autoStamp, float interval, float moveDur)
    {
        activeEnvelopes.Clear();

        for (int i = 0; i < seq.pattern.Length; i++)
        {
            NoteType type = seq.pattern[i];
            SpawnEnvelope(type, autoStamp, moveDur);

            float waitTime = interval;

            if (type == NoteType.HalfTap || type == NoteType.HalfTapStamped)
            {
                yield return new WaitForSeconds(interval / 2f);
                SpawnEnvelope(type, autoStamp, moveDur);

                waitTime = interval / 1.3f;
            }
            else if ((type == NoteType.Tap || type == NoteType.TapStamped)
                    && i + 1 < seq.pattern.Length
                    && (seq.pattern[i + 1] == NoteType.HalfTap || seq.pattern[i + 1] == NoteType.HalfTapStamped))
            {
                waitTime = interval / 1.3f;
            }

            yield return new WaitForSeconds(waitTime);
        }
    }

    void SpawnEnvelope(NoteType type, bool autoStamp, float moveDur)
    {
        if (envelopePrefabDict.TryGetValue(type, out GameObject prefab))
        {
            GameObject env = Instantiate(prefab, envelopePositions[0].position, Quaternion.identity);
            env.GetComponent<Envelope>().noteType = type;
            env.GetComponent<Envelope>().moveDuration = moveDur;
            activeEnvelopes.Add(env);

            if (autoStamp)
                env.GetComponent<Envelope>().needsStampSwap = true;

            StartCoroutine(MoveEnvelopeAlongConveyor(env, moveDur));
        }
    }

    IEnumerator MoveEnvelopeAlongConveyor(GameObject envelope, float moveDuration)
    {
        for (int posIndex = 1; posIndex < envelopePositions.Length; posIndex++)
        {
            Vector3 startPos = envelopePositions[posIndex - 1].position;
            Vector3 endPos = envelopePositions[posIndex].position;

            float elapsed = 0f;
            while (elapsed < moveDuration)
            {
                if (envelope == null) yield break;
                envelope.transform.position = Vector3.Lerp(startPos, endPos, elapsed / moveDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (envelope == null) yield break;
            envelope.transform.position = endPos;
        }

        if (envelope != null)
        {
            Destroy(envelope);
        }
        activeEnvelopes.Remove(envelope);
    }

    void StampEnvelope(GameObject env, float moveDuration)
    {
        Envelope e = env.GetComponent<Envelope>();
        if (e != null && e.needsStampSwap)
        {
            e.needsStampSwap = false;

            if (armsController != null)
            {
                armsController.PlayArmsAnimation(1f);
            }
        }
    }

    public void ProcessSuccessfulAction(GameObject envelope)
    {
        Envelope e = envelope.GetComponent<Envelope>();
        StampEnvelope(envelope, e.moveDuration);

        if (activeEnvelopes.Contains(envelope))
        {
            activeEnvelopes.Remove(envelope);
        }
    }
}