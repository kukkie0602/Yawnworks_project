using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingEnvelopeLevel : EnvelopeConveyor
{
    [Header("Level 2 Settings")]
    public Transform[] spawnPoints; 
    public Transform[] tablePositions;
    public Transform[] boxPositions;   
    public float fallDuration = 1f;
    public float shootDuration = 0.5f;
    public float tapRadius = 0.5f;

    // Override spawn to spawn at top of screen
    protected void SpawnEnvelope(NoteType type, bool autoStamp, double spawnTime, int laneIndex)
    {
        if (!envelopePrefabDict.TryGetValue(type, out GameObject prefab)) return;

        Transform spawnPoint = spawnPoints[laneIndex];
        Transform tablePos = tablePositions[laneIndex];
        Transform boxPos = boxPositions[laneIndex];

        GameObject env = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        Envelope e = env.GetComponent<Envelope>();
        e.noteType = type;

        activeEnvelopes.Add(env);

        StartCoroutine(FallToTableAndShoot(env, tablePos, boxPos));
    }

    // Movement and tap/shoot logic
    private IEnumerator FallToTableAndShoot(GameObject envelope, Transform tablePos, Transform boxPos)
    {
        if (envelope == null) yield break;
        Envelope e = envelope.GetComponent<Envelope>();
        if (e == null) yield break;

        // --- Fall to table ---
        Vector3 start = envelope.transform.position;
        Vector3 end = tablePos.position;
        float t = 0f;
        while (t < 1f)
        {
            if (envelope == null) yield break;
            t += Time.deltaTime / fallDuration;
            envelope.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
        envelope.transform.position = end;

        // --- Wait for tap or successful action ---
        yield return new WaitUntil(() => e.isTapped);

        // --- Shoot to box ---
        start = envelope.transform.position;
        end = boxPos.position;
        t = 0f;
        while (t < 1f)
        {
            if (envelope == null) yield break;
            t += Time.deltaTime / shootDuration;
            envelope.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        if (envelope != null)
            Destroy(envelope);

        activeEnvelopes.Remove(envelope);
    }
    protected override IEnumerator SpawnAndAnimateSequence(EnvelopeSequence seq, bool autoStamp)
    {
        activeEnvelopes.Clear();

        for (int i = 0; i < seq.pattern.Length; i++)
        {
            NoteType type = seq.pattern[i];
            double spawnTime = songStartDspTime + (sequenceIndex * seq.pattern.Length + i) * beatInterval;

            yield return new WaitUntil(() => CurrentSongTime >= spawnTime);

            // Pick lane deterministically from envelope index
            int laneIndex = i % spawnPoints.Length;

            SpawnEnvelope(type, autoStamp, spawnTime, laneIndex);
        }
    }
    // Connect Level 1 hit detection to shooting
    public override void ProcessSuccessfulAction(GameObject envelope)
    {
        Envelope e = envelope.GetComponent<Envelope>();
        if (e != null)
        {
            e.isTapped = true; // signal coroutine to shoot
            StampEnvelope(envelope, e.moveDuration);
        }
    }
}
