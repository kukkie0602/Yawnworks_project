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

    [Header("BPM Synchronization")]
    [Tooltip("The index in envelopePositions that aligns with the center of the hit zone.")]
    public int hitZonePositionIndex = 4; 
    [Tooltip("How many beats it should take for an envelope to travel from spawn to the hit zone.")]
    public float beatsToHitZone = 4f;

    [Header("Prefabs & References")]
    public Transform[] envelopePositions;
    [Range(0.1f, 2f)]
    public float spacingFactor = 0.5f;
    public GameObject stampedEnvelopePrefab;
    public NotePrefabMapping[] noteMappings;
    public Animator armsAnimator;

    private Dictionary<NoteType, GameObject> envelopePrefabDict;
    private List<GameObject> activeEnvelopes = new List<GameObject>();

    private int sequenceIndex = 0;

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
        Debug.Log("Level Complete!");
    }


    public IEnumerator PlayExamplePhase(EnvelopeSequence seq)
    {
        float spacing = MoveSpeed(seq);
        yield return StartCoroutine(SpawnAndAnimateSequence(seq, autoStamp: true, spacing, seq.moveDuration));
    }

    public IEnumerator PlayPlayerPhase(EnvelopeSequence seq)
    {
        float spacing = MoveSpeed(seq);
        yield return StartCoroutine(SpawnAndAnimateSequence(seq, autoStamp: false, spacing, seq.moveDuration));
    }

    IEnumerator SpawnAndAnimateSequence(EnvelopeSequence seq, bool autoStamp, float interval, float moveDur)
    {
        activeEnvelopes.Clear();

        for (int i = 0; i < seq.pattern.Length; i++)
        {
            NoteType type = seq.pattern[i];

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

            yield return new WaitForSeconds(interval);
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
                const float defaultAnimationDuration = 0.3f;
                float speedMultiplier = defaultAnimationDuration / moveDuration;
                Debug.Log($"Stamp Speed Multiplier: {speedMultiplier}, moveDuration: {moveDuration}");
                armsController.PlayArmsAnimation(speedMultiplier);
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

    public float MoveSpeed(EnvelopeSequence seq)
    {
        float totalTravelTime = envelopePositions.Length * seq.moveDuration;
        float spacing = (totalTravelTime / 3) * spacingFactor + 0.05f;
        return spacing;
    }
}