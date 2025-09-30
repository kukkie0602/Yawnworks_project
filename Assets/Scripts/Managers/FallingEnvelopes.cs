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
    public float tapWindowBeats = 4f;
    public float beatsBetweenSets = 4f;
    public float spawnSpeedMultiplier = 1f;

    private HashSet<GameObject> tappableEnvelopes = new HashSet<GameObject>();
    private HashSet<GameObject> landedEnvelopes = new HashSet<GameObject>();

    protected override IEnumerator SpawnAndAnimateSequence(EnvelopeSequence seq, bool autoStamp)
    {
        activeEnvelopes.Clear();

        bool firstSet = true;

        for (int i = 0; i < seq.pattern.Length; i += 4)
        {
            List<GameObject> currentSet = new List<GameObject>();

            for (int j = 0; j < 4 && i + j < seq.pattern.Length; j++)
            {
                NoteType type = seq.pattern[i + j];
                int laneIndex = (i + j) % spawnPoints.Length;
                GameObject env = SpawnEnvelope(type, autoStamp, 0, laneIndex);
                if (env != null) currentSet.Add(env);

                yield return new WaitForSeconds(beatInterval * spawnSpeedMultiplier);
            }

            StartCoroutine(HandleTapWindow(currentSet));

            // Wait until all envelopes in this set have landed
            yield return new WaitUntil(() => AllEnvelopesLanded(currentSet));

            // Only then apply the delay between sets
            yield return new WaitForSeconds(beatsBetweenSets * beatInterval);
        }
    }

    private IEnumerator HandleTapWindow(List<GameObject> set)
    {
        yield return new WaitUntil(() => AllEnvelopesLanded(set));

        double tapWindowSeconds = tapWindowBeats * beatInterval;
        double startTime = CurrentSongTime;

        foreach (var env in set)
            if (env != null) tappableEnvelopes.Add(env);

        while (CurrentSongTime < startTime + tapWindowSeconds &&
               set.Exists(env => env != null && !env.GetComponent<Envelope>().isTapped))
            yield return null;

        foreach (var env in set)
        {
            if (env == null) continue;
            Envelope e = env.GetComponent<Envelope>();
            tappableEnvelopes.Remove(env);
            if (e != null && !e.isTapped) e.isTapped = true;
        }
    }

    protected GameObject SpawnEnvelope(NoteType type, bool autoStamp, double spawnTime, int laneIndex)
    {
        if (!envelopePrefabDict.TryGetValue(type, out GameObject prefab)) return null;

        Transform spawnPoint = spawnPoints[laneIndex];
        Transform tablePos = tablePositions[laneIndex];
        Transform boxPos = boxPositions[laneIndex];

        GameObject env = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        Envelope e = env.GetComponent<Envelope>();
        e.noteType = type;
        e.moveDuration = (beatsToHitZone * beatInterval) / hitZonePositionIndex;

        activeEnvelopes.Add(env);
        StartCoroutine(FallToTable(env, tablePos, boxPos));
        return env;
    }

    private IEnumerator FallToTable(GameObject envelope, Transform tablePos, Transform boxPos)
    {
        if (envelope == null) yield break;
        Envelope e = envelope.GetComponent<Envelope>();
        if (e == null) yield break;

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

        landedEnvelopes.Add(envelope);

        yield return new WaitUntil(() => e.isTapped);

        yield return StartCoroutine(ShootToBox(envelope, boxPos));
    }

    private IEnumerator ShootToBox(GameObject envelope, Transform boxPos)
    {
        if (envelope == null) yield break;
        Vector3 start = envelope.transform.position;
        Vector3 end = boxPos.position;
        float t = 0f;
        while (t < 1f)
        {
            if (envelope == null) yield break;
            t += Time.deltaTime / shootDuration;
            envelope.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
        if (envelope != null) Destroy(envelope);
        activeEnvelopes.Remove(envelope);
        landedEnvelopes.Remove(envelope);
    }

    private bool AllEnvelopesLanded(List<GameObject> set)
    {
        foreach (var env in set)
        {
            if (env == null) continue;
            if (!landedEnvelopes.Contains(env)) return false;
        }
        return true;
    }

    public override void ProcessSuccessfulAction(GameObject envelope)
    {
        if (envelope == null) return;
        if (!tappableEnvelopes.Contains(envelope)) return;

        Envelope e = envelope.GetComponent<Envelope>();
        if (e == null) return;

        e.isTapped = true;
        StampEnvelope(envelope, e.moveDuration);
    }
}
