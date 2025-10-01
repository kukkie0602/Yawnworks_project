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

    protected Dictionary<NoteType, GameObject> envelopePrefabDict;
    protected List<GameObject> activeEnvelopes = new List<GameObject>();

    protected int sequenceIndex = 0;
    protected double songStartDspTime;
    protected float beatInterval;
    private float moveDuration;

    private bool isPaused = false;
    private double pauseStartedTime = 0.0;
    private double totalTimePaused = 0.0;
    protected double CurrentSongTime
    {
        get
        {
            if (isPaused)
            {
                return pauseStartedTime - totalTimePaused;
            }
            else
            {
                return AudioSettings.dspTime - totalTimePaused;
            }
        }
    }

    void Start()
    {
        envelopePrefabDict = new Dictionary<NoteType, GameObject>();
        foreach (var mapping in noteMappings)
            envelopePrefabDict[mapping.noteType] = mapping.envelopePrefab;

        if (!isTutorialMode)
        {
            if (levelData == null || levelData.sequences.Length == 0) {  return; }
            if (timingManager == null) {  return; }

            beatInterval = 60f / levelData.beatsPerMinute;
            float totalTravelTime = beatsToHitZone * beatInterval;
            moveDuration = totalTravelTime / hitZonePositionIndex;

            songStartDspTime = AudioSettings.dspTime;
            StartCoroutine(PlaySequenceCoroutine());

            double audioDspTime = AudioSettings.dspTime + (moveDuration * hitZonePositionIndex) - 0.3f;
            audioManager.PlayScheduled(audioDspTime);
        }
    }

    public void Pause()
    {
        if (isPaused) return;
        isPaused = true;
        pauseStartedTime = AudioSettings.dspTime;
    }

    public void Resume()
    {
        if (!isPaused) return;
        double pauseDuration = AudioSettings.dspTime - pauseStartedTime;
        totalTimePaused += pauseDuration;
        isPaused = false;
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

    protected virtual IEnumerator SpawnAndAnimateSequence(EnvelopeSequence seq, bool autoStamp)
    {
        activeEnvelopes.Clear();

        for (int i = 0; i < seq.pattern.Length; i++)
        {
            EnvelopeSequence.Beat beat = seq.pattern[i];

            if (beat.first != NoteType.None && beat.first != NoteType.SkipOne)
            {
                double spawnTime = songStartDspTime + (sequenceIndex * seq.pattern.Length + i) * beatInterval;
                yield return new WaitUntil(() => CurrentSongTime >= spawnTime);
                SpawnEnvelope(beat.first, autoStamp, spawnTime);
            }

            if (beat.second != NoteType.None && beat.second != NoteType.SkipOne)
            {
                double halfSpawnTime = songStartDspTime + (sequenceIndex * seq.pattern.Length + i + 0.5) * beatInterval;
                yield return new WaitUntil(() => CurrentSongTime >= halfSpawnTime);
                SpawnEnvelope(beat.second, autoStamp, halfSpawnTime);
            }
        }
    }

    protected virtual void SpawnEnvelope(NoteType type, bool autoStamp, double spawnTime)
    {
        if (!envelopePrefabDict.TryGetValue(type, out GameObject prefab))
            return;

        GameObject env = Instantiate(prefab, envelopePositions[0].position, Quaternion.identity);
        Envelope e = env.GetComponent<Envelope>();
        e.noteType = type;
        e.moveDuration = moveDuration;

        e.isHalfNote = (type == NoteType.E4Half || type == NoteType.G4Half || type == NoteType.C5Half || type == NoteType.D5Half);

        if (autoStamp)
            e.needsStampSwap = true;

        activeEnvelopes.Add(env);
        StartCoroutine(MoveEnvelopeAlongConveyor(env, spawnTime));
    }

    protected virtual IEnumerator MoveEnvelopeAlongConveyor(GameObject envelope, double spawnTime)
    {
        for (int posIndex = 1; posIndex < envelopePositions.Length; posIndex++)
        {
            Vector3 startPos = envelopePositions[posIndex - 1].position;
            Vector3 endPos = envelopePositions[posIndex].position;

            double segmentStart = spawnTime + (posIndex - 1) * moveDuration;
            double segmentEnd = spawnTime + posIndex * moveDuration;
            while (CurrentSongTime < segmentEnd)
            {
                if (envelope == null) yield break;

                float t = (float)((CurrentSongTime - segmentStart) / (segmentEnd - segmentStart));
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

    public void StampEnvelope(GameObject env, float moveDuration)
    {
        Envelope e = env.GetComponent<Envelope>();
        if (e != null && e.needsStampSwap && armsController != null)
        {
            e.needsStampSwap = false;
            armsController.PlayArmsAnimation(1f);
        }
    }

    public virtual void ProcessSuccessfulAction(GameObject envelope)
    {
        Envelope e = envelope.GetComponent<Envelope>();
        StampEnvelope(envelope, e.moveDuration);

        if (activeEnvelopes.Contains(envelope))
            activeEnvelopes.Remove(envelope);
    }
}