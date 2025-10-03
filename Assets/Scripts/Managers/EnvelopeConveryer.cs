using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvelopeConveyor : MonoBehaviour
{
    [Header("UI References")]
    public ArmsController armsController;
    public TimingManager timingManager;
    public EndGamePanel endGameManger;
    public Animator countdownAnimator;
    public Animator finishAnimator;
    public StampTriggerZone stampZone;

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
	public EnvelopeLevel currentLevelData;
    protected int sequenceIndex = 0;
    protected double songStartDspTime;
    protected float beatInterval;
    private float moveDuration;
    private bool levelIsPlaying = false;
    private bool countdownHasBeenScheduled = false;

    private bool isPaused = false;
    private double pauseStartedTime = 0.0;
    private double totalTimePaused = 0.0;
    protected double CurrentSongTime => isPaused ? pauseStartedTime - totalTimePaused : AudioSettings.dspTime - totalTimePaused;

    void Awake()
    {
        if (stampZone != null)
        {
            stampZone.gameObject.SetActive(false);
        }
        envelopePrefabDict = new Dictionary<NoteType, GameObject>();
        foreach (var mapping in noteMappings)
            envelopePrefabDict[mapping.noteType] = mapping.envelopePrefab;
    }

    private void SetupConveyorForLevel(EnvelopeLevel levelData)
    {
        StopAllCoroutines();

        foreach (var env in activeEnvelopes)
        {
            if (env != null) Destroy(env);
        }
        activeEnvelopes.Clear();

        if (timingManager != null) timingManager.ResetManager();

        this.currentLevelData = levelData;

        beatInterval = 60f / this.currentLevelData.beatsPerMinute;
        float totalTravelTime = beatsToHitZone * beatInterval;
        moveDuration = totalTravelTime / hitZonePositionIndex;

        sequenceIndex = 0;
        totalTimePaused = 0;
        songStartDspTime = AudioSettings.dspTime;

        countdownHasBeenScheduled = false;
    }

    public void StartMainLevel(EnvelopeLevel mainLevelData)
    {
        if (levelIsPlaying) return;

        SetupConveyorForLevel(mainLevelData);
        levelIsPlaying = true;

        StartCoroutine(MainLevelCoroutine());

        double audioDspTime = AudioSettings.dspTime + (moveDuration * hitZonePositionIndex) - 0.5f;
        if (audioManager != null) audioManager.PlayScheduled(audioDspTime);
    }

    public IEnumerator PlayTutorialSequence(EnvelopeSequence seq, bool autoStamp, float bpm)
    {
        beatInterval = 60f / bpm;
        float totalTravelTime = beatsToHitZone * beatInterval;
        moveDuration = totalTravelTime / hitZonePositionIndex;
        songStartDspTime = AudioSettings.dspTime;

        yield return StartCoroutine(SpawnAndAnimateSequence(seq, autoStamp, true, 0));
    }

    private IEnumerator MainLevelCoroutine()
    {
        while (sequenceIndex < currentLevelData.sequences.Length)
        {
            var seq = currentLevelData.sequences[sequenceIndex];
            timingManager.playerInputEnabled = true;
            yield return StartCoroutine(SpawnAndAnimateSequence(seq, false, false, sequenceIndex));
            sequenceIndex++;
        }

        while (activeEnvelopes.Count > 0) yield return null;
        timingManager.playerInputEnabled = false;

        if (finishAnimator != null)
        {
            finishAnimator.SetTrigger("FinishStart");
        }
        yield return new WaitForSeconds(3.0f);
        endGameManger.End();
    }

    protected virtual IEnumerator SpawnAndAnimateSequence(EnvelopeSequence seq, bool autoStamp, bool isTutorial, int currentSequenceIndex)
    {
        if (isTutorial && stampZone != null)
        {
            stampZone.gameObject.SetActive(autoStamp);
        }

        for (int i = 0; i < seq.pattern.Length; i++)
        {
            EnvelopeSequence.Beat beat = seq.pattern[i];
            double spawnOffset = (isTutorial ? i : (currentSequenceIndex * seq.pattern.Length + i)) * beatInterval;
            double spawnTime = songStartDspTime + spawnOffset;

            if (beat.first == NoteType.SkipOne && !countdownHasBeenScheduled && !isTutorial)
            {
                double timeToHitZone = beatsToHitZone * beatInterval;
                double animationTriggerTime = spawnTime + timeToHitZone;
                StartCoroutine(ScheduleAnimationTrigger(animationTriggerTime));
                countdownHasBeenScheduled = true;
            }

            yield return new WaitUntil(() => CurrentSongTime >= spawnTime);
            if (beat.first == NoteType.None) { break; }
            SpawnEnvelope(beat.first, autoStamp, spawnTime);

            if (beat.first == NoteType.HalfTap || beat.first == NoteType.HalfTapStamped)
            {
                double halfSpawnTime = spawnTime + beatInterval / 2f;
                yield return new WaitUntil(() => CurrentSongTime >= halfSpawnTime);
                SpawnEnvelope(beat.first, autoStamp, halfSpawnTime);
            }
        }

        if (isTutorial)
        {
            double sequenceDuration = seq.pattern.Length * beatInterval;
            double clearTime = beatsToHitZone * beatInterval;
            yield return new WaitUntil(() => CurrentSongTime >= songStartDspTime + sequenceDuration + clearTime);
        }
    }

    private IEnumerator ScheduleAnimationTrigger(double triggerDspTime)
    {
        yield return new WaitUntil(() => AudioSettings.dspTime >= triggerDspTime -1);

        if (countdownAnimator != null)
        {
            countdownAnimator.SetTrigger("StartCountdown");
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
        Envelope e = envelope.GetComponent<Envelope>();
        for (int posIndex = 1; posIndex < envelopePositions.Length; posIndex++)
        {
            Vector3 startPos = envelopePositions[posIndex - 1].position;
            Vector3 endPos = envelopePositions[posIndex].position;

            double segmentStart = spawnTime + (posIndex - 1) * moveDuration;
            double segmentEnd = spawnTime + posIndex * moveDuration;

            while (CurrentSongTime < segmentEnd)
            {
                if (envelope == null) yield break;
                float t = (float)((CurrentSongTime - segmentStart) / moveDuration);
                envelope.transform.position = Vector3.Lerp(startPos, endPos, Mathf.Clamp01(t));
                yield return null;
            }

            if (envelope == null) yield break;
            envelope.transform.position = endPos;

            if (posIndex == hitZonePositionIndex && e != null && e.needsStampSwap)
            {
                StampEnvelope(envelope);
                timingManager.TriggerSpriteSwap(e);
            }
        }

        if (envelope != null) Destroy(envelope);
        activeEnvelopes.Remove(envelope);
    }

    protected virtual void StampEnvelope(GameObject env)
    {
        Envelope e = env.GetComponent<Envelope>();
        if (e != null && e.needsStampSwap && armsController != null)
        {
            e.needsStampSwap = false;
            timingManager.TriggerGoodStamp();
            armsController.PlayArmsAnimation(1f);
        }
    }

    public virtual void ProcessSuccessfulAction(GameObject envelope)
    {
        if (activeEnvelopes.Contains(envelope))
        {
            StampEnvelope(envelope);
            activeEnvelopes.Remove(envelope);
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
}