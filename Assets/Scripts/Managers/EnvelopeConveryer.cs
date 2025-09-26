using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvelopeConveyor : MonoBehaviour
{
    [Header("Mode")]
    public bool isTutorialMode = false;

    [Header("Level Data")]
    public EnvelopeLevel levelData;

    [Header("UI References")]
    public ArmsController armsController;
    public TimingManager timingManager;
    public EndGamePanel endGameManger;

    [Header("BPM Synchronization")]
    public int hitZonePositionIndex = 4;
    public float beatsToHitZone = 4f;

    [Header("Prefabs & References")]
    public Transform[] envelopePositions;
    public GameObject stampedEnvelopePrefab;
    public NotePrefabMapping[] noteMappings;
    public Animator armsAnimator;
    public AudioManager audioManager;

    private Dictionary<NoteType, GameObject> envelopePrefabDict;
    private List<GameObject> activeEnvelopes = new List<GameObject>();

    private int sequenceIndex = 0;
    private double songStartDspTime;
    private float beatInterval;
    private float moveDuration;

    void Start()
    {
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
                Debug.LogError("Timing Manager is not assigned in the EnvelopeConveyor!");
                this.enabled = false;
                return;
            }

            beatInterval = 60f / levelData.beatsPerMinute;
            float totalTravelTime = beatsToHitZone * beatInterval;
            moveDuration = totalTravelTime / hitZonePositionIndex;

            // ENVELOPES START IMMEDIATELY
            songStartDspTime = AudioSettings.dspTime;

            // Start spawning envelopes
            StartCoroutine(PlaySequenceCoroutine());

            float totalTravelTimeToHitZone = moveDuration * hitZonePositionIndex;
            float audioAdvance = 0.3f; // start audio 0.1 seconds earlier
            double audioDspTime = AudioSettings.dspTime + totalTravelTimeToHitZone - audioAdvance;
            audioManager.PlayScheduled(audioDspTime);
        }
    }


    IEnumerator PlaySequenceCoroutine()
    {
        while (sequenceIndex < levelData.sequences.Length)
        {
            var seq = levelData.sequences[sequenceIndex];
            timingManager.playerInputEnabled = true;

            yield return StartCoroutine(SpawnAndAnimateSequence(seq, false));

            sequenceIndex++;
        }

        while (activeEnvelopes.Count > 0)
            yield return null;

        timingManager.playerInputEnabled = false;
        endGameManger.End();
    }

    public IEnumerator PlayExamplePhase(EnvelopeSequence seq)
    {
        yield return StartCoroutine(SpawnAndAnimateSequence(seq, true));
    }

    public IEnumerator PlayPlayerPhase(EnvelopeSequence seq)
    {
        yield return StartCoroutine(SpawnAndAnimateSequence(seq, false));
    }

    IEnumerator SpawnAndAnimateSequence(EnvelopeSequence seq, bool autoStamp)
    {
        activeEnvelopes.Clear();

        for (int i = 0; i < seq.pattern.Length; i++)
        {
            NoteType type = seq.pattern[i];
            double spawnTime = songStartDspTime + (sequenceIndex * seq.pattern.Length + i) * beatInterval;

            yield return new WaitUntil(() => AudioSettings.dspTime >= spawnTime);
            SpawnEnvelope(type, autoStamp, spawnTime);

            if (type == NoteType.HalfTap || type == NoteType.HalfTapStamped)
            {
                double halfSpawn = spawnTime + beatInterval / 2f;
                yield return new WaitUntil(() => AudioSettings.dspTime >= halfSpawn);
                SpawnEnvelope(type, autoStamp, halfSpawn);
            }
        }
    }

    void SpawnEnvelope(NoteType type, bool autoStamp, double spawnTime)
    {
        if (envelopePrefabDict.TryGetValue(type, out GameObject prefab))
        {
            GameObject env = Instantiate(prefab, envelopePositions[0].position, Quaternion.identity);
            Envelope e = env.GetComponent<Envelope>();
            e.noteType = type;
            e.moveDuration = moveDuration;
            if (autoStamp) e.needsStampSwap = true;

            activeEnvelopes.Add(env);
            StartCoroutine(MoveEnvelopeAlongConveyor(env, spawnTime));
        }
    }

    IEnumerator MoveEnvelopeAlongConveyor(GameObject envelope, double spawnTime)
    {
        for (int posIndex = 1; posIndex < envelopePositions.Length; posIndex++)
        {
            Vector3 startPos = envelopePositions[posIndex - 1].position;
            Vector3 endPos = envelopePositions[posIndex].position;

            double segmentStart = spawnTime + (posIndex - 1) * moveDuration;
            double segmentEnd = spawnTime + posIndex * moveDuration;

            while (AudioSettings.dspTime < segmentEnd)
            {
                if (envelope == null) yield break;

                float t = (float)((AudioSettings.dspTime - segmentStart) / (segmentEnd - segmentStart));
                t = Mathf.Clamp01(t);
                envelope.transform.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }

            if (envelope == null) yield break;
            envelope.transform.position = endPos;
        }

        if (envelope != null)
            Destroy(envelope);

        activeEnvelopes.Remove(envelope);
    }

    void StampEnvelope(GameObject env, float moveDuration)
    {
        Envelope e = env.GetComponent<Envelope>();
        if (e != null && e.needsStampSwap && armsController != null)
        {
            e.needsStampSwap = false;
            armsController.PlayArmsAnimation(1f);
        }
    }

    public void ProcessSuccessfulAction(GameObject envelope)
    {
        Envelope e = envelope.GetComponent<Envelope>();
        StampEnvelope(envelope, e.moveDuration);

        if (activeEnvelopes.Contains(envelope))
            activeEnvelopes.Remove(envelope);
    }
}
