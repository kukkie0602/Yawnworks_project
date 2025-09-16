using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvelopeConveyor : MonoBehaviour
{
    [Header("Level Data")]
    public EnvelopeLevel levelData;
    [Header("UI References")]
    public ArmsController armsController;
    [Header("Prefabs & References")]
    public Transform[] envelopePositions;
    public float moveDuration = 0.1f;
    public GameObject stampedEnvelopePrefab;
    public NotePrefabMapping[] noteMappings;
    public Animator armsAnimator;
    private Dictionary<NoteType, GameObject> envelopePrefabDict;
    private List<GameObject> activeEnvelopes = new List<GameObject>();

    private int sequenceIndex = 0;
    private bool isExamplePhase = true;

    void Start()
    {
        envelopePrefabDict = new Dictionary<NoteType, GameObject>();
        foreach (var mapping in noteMappings)
            envelopePrefabDict[mapping.noteType] = mapping.envelopePrefab;

        if (levelData == null || levelData.sequences.Length == 0)
        {
            Debug.LogError("No level assigned or empty level data!");
            return;
        }

        StartCoroutine(PlaySequenceCoroutine());
    }

    IEnumerator PlaySequenceCoroutine()
    {
        while (sequenceIndex < levelData.sequences.Length)
        {
            var seq = levelData.sequences[sequenceIndex];

            // --- Example Phase ---
            isExamplePhase = true;
            yield return StartCoroutine(SpawnAndAnimateSequence(seq, autoStamp: true));

            // --- Player Phase ---
            isExamplePhase = false;
            yield return StartCoroutine(SpawnAndAnimateSequence(seq, autoStamp: false));

            sequenceIndex++;
        }

        Debug.Log("Level Complete!");
    }

    IEnumerator SpawnAndAnimateSequence(EnvelopeSequence seq, bool autoStamp)
    {
        activeEnvelopes.Clear();

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

            yield return new WaitForSeconds(0.62f);
        }

        yield return new WaitForSeconds(moveDuration);
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
                envelope.transform.position = Vector3.Lerp(startPos, endPos, elapsed / moveDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            envelope.transform.position = endPos;
        }

        Destroy(envelope);
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
        // Stamp the envelope (visual feedback)
        StampEnvelope(envelope);

        // Optionally: remove it from active list
        if (activeEnvelopes.Contains(envelope))
        {
            activeEnvelopes.Remove(envelope);
        }

        // If you still want movement like before:
        // StartCoroutine(AdvanceConveyorCoroutine());
    }
}
