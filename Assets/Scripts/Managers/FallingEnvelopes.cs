using NUnit.Framework;
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

    [Header("Note Sounds")]
    public AudioClip e4Sound;
    public AudioClip g4Sound;
    public AudioClip c5Sound;
    public AudioClip d5Sound;

    private Dictionary<NoteType, AudioClip> noteSounds;
    private Dictionary<NoteType, int> noteToLane;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        // Map musical note types to sounds
        noteSounds = new Dictionary<NoteType, AudioClip>()
        {
            { NoteType.E4, e4Sound },
            { NoteType.G4, g4Sound },
            { NoteType.C5, c5Sound },
            { NoteType.D5, d5Sound }
        };

        // Map musical note types to lanes (left-to-right)
        noteToLane = new Dictionary<NoteType, int>()
        {
            { NoteType.E4, 0 }, // leftmost lane
            { NoteType.G4, 1 },
            { NoteType.C5, 2 },
            { NoteType.D5, 3 },  // rightmost lane

            { NoteType.E4Half, 0 },
            { NoteType.G4Half, 1 },
            { NoteType.C5Half, 2 },
            { NoteType.D5Half, 3 }
        };

        envelopePrefabDict = new Dictionary<NoteType, GameObject>();
        foreach (var mapping in noteMappings)
            envelopePrefabDict[mapping.noteType] = mapping.envelopePrefab;

        StartMainLevel(currentLevelData);
    }

    // Override spawn to spawn at the correct lane for the note
    protected void SpawnEnvelope(NoteType type, bool autoStamp, double spawnTime)
    {
        if (!envelopePrefabDict.TryGetValue(type, out GameObject prefab))
            return;

        if (!noteToLane.TryGetValue(type, out int laneIndex))
        {
            Debug.LogWarning($"NoteType {type} has no lane mapping!");
            return;
        }

        Transform spawnPoint = spawnPoints[laneIndex];

        if (spawnPoint == null)
        {
            Debug.LogError($"Spawn point at index {laneIndex} is not assigned!");
            return; 
        }
        Transform tablePos = tablePositions[laneIndex];
        Transform boxPos = boxPositions[laneIndex];

        GameObject env = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        Envelope e = env.GetComponent<Envelope>();
        e.noteType = type;

        // Determine if the envelope should fall faster (half-note)
        e.isHalfNote = (type == NoteType.E4Half || type == NoteType.G4Half ||
                        type == NoteType.C5Half || type == NoteType.D5Half);

        activeEnvelopes.Add(env);

        StartCoroutine(FallToTableAndShoot(env, tablePos, boxPos));
    }


    // Movement and tap/shoot logic
    private IEnumerator FallToTableAndShoot(GameObject envelope, Transform tablePos, Transform boxPos)
    {
        if (envelope == null) yield break;
        Envelope e = envelope.GetComponent<Envelope>();
        if (e == null) yield break;

        float duration = e.isHalfNote ? fallDuration / 2f : fallDuration;

        // --- Fall to table ---
        Vector3 start = envelope.transform.position;
        Vector3 end = tablePos.position;
        float t = 0f;

        while (t < 1f)
        {
            if (envelope == null) yield break;
            t += Time.deltaTime / duration;
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

        if (envelope != null) Destroy(envelope);
        activeEnvelopes.Remove(envelope);
    }

    protected override IEnumerator SpawnAndAnimateSequence(EnvelopeSequence seq, bool autoStamp, bool isTutorial=false, int currentSequenceIndex=0)
    {
        activeEnvelopes.Clear();

        for (int i = 0; i < seq.pattern.Length; i++)
        {
            EnvelopeSequence.Beat beat = seq.pattern[i];

            // --- First note ---
            if (beat.first != NoteType.SkipOne && beat.first != NoteType.None)
            {
                double spawnTime = songStartDspTime + (sequenceIndex * seq.pattern.Length + i) * beatInterval;
                yield return new WaitUntil(() => CurrentSongTime >= spawnTime);
                SpawnEnvelope(beat.first, autoStamp, spawnTime);
            }

            // --- Second note (half-beat) ---
            if (beat.second != NoteType.SkipOne && beat.second != NoteType.None)
            {
                double spawnTime = songStartDspTime + (sequenceIndex * seq.pattern.Length + i + 0.5) * beatInterval;
                yield return new WaitUntil(() => CurrentSongTime >= spawnTime);
                SpawnEnvelope(beat.second, autoStamp, spawnTime);
            }
        }
    }

    // Connect Level 1 hit detection to shooting
    public override void ProcessSuccessfulAction(GameObject envelope)
    {
        Envelope e = envelope.GetComponent<Envelope>();
        if (e != null)
        {
            e.isTapped = true; // signal coroutine to shoot
            StampEnvelope(envelope);

            // Play sound only if note type has an assigned clip
            if (noteSounds.TryGetValue(e.noteType, out AudioClip clip) && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }
}
