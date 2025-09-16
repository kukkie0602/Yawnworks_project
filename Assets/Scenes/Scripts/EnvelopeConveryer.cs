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

    [Header("Prefabs & References")]
    public Transform[] envelopePositions;
    public float moveDuration = 0.1f;
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
        while (sequenceIndex < levelData.sequences.Length)
        {
            var seq = levelData.sequences[sequenceIndex];

            timingManager.playerInputEnabled = false;
            yield return StartCoroutine(SpawnAndAnimateSequence(seq, autoStamp: true));

            timingManager.playerInputEnabled = true;
            yield return StartCoroutine(SpawnAndAnimateSequence(seq, autoStamp: false));

            sequenceIndex++;
        }
        timingManager.playerInputEnabled = false;
        Debug.Log("Level Complete!");
    }

    public IEnumerator PlayExamplePhase(EnvelopeSequence seq)
    {
        yield return StartCoroutine(SpawnAndAnimateSequence(seq, autoStamp: true));
    }

    public IEnumerator PlayPlayerPhase(EnvelopeSequence seq)
    {
        yield return StartCoroutine(SpawnAndAnimateSequence(seq, autoStamp: false));
    }

    IEnumerator SpawnAndAnimateSequence(EnvelopeSequence seq, bool autoStamp)
    {
        activeEnvelopes.Clear();
        float timeBetweenNotes = 0.62f;

        for (int i = 0; i < seq.pattern.Length; i++)
        {
            NoteType type = seq.pattern[i];

            if (envelopePrefabDict.TryGetValue(type, out GameObject prefab))
            {
                GameObject env = Instantiate(prefab, envelopePositions[0].position, Quaternion.identity);
                env.GetComponent<Envelope>().noteType = type;
                activeEnvelopes.Add(env);

                if (autoStamp)
                {
                    env.GetComponent<Envelope>().needsStampSwap = true;
                }
                StartCoroutine(MoveEnvelopeAlongConveyor(env, i));
            }
            yield return new WaitForSeconds(timeBetweenNotes);
        }

        float conveyorClearTime = envelopePositions.Length * moveDuration;
        yield return new WaitForSeconds(conveyorClearTime);
    }

    IEnumerator MoveEnvelopeAlongConveyor(GameObject envelope, int sequenceIndex)
    {
        for (int posIndex = 0; posIndex < envelopePositions.Length; posIndex++)
        {
            Vector3 startPos = envelope.transform.position;
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

    void StampEnvelope(GameObject env)
    {
        Envelope e = env.GetComponent<Envelope>();
        if (e != null && e.needsStampSwap)
        {
            e.needsStampSwap = false;

            if (armsController != null)
            {
                armsController.PlayArmsAnimation();
            }
        }
    }

    public void ProcessSuccessfulAction(GameObject envelope)
    {
        StampEnvelope(envelope);

        if (activeEnvelopes.Contains(envelope))
        {
            activeEnvelopes.Remove(envelope);
        }
    }
}