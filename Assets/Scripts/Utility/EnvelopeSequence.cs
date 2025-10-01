using UnityEngine;

[System.Serializable]
public class EnvelopeSequence
{
    [Tooltip("Each element represents a beat. For half-beat pairs, assign two notes to spawn in this slot.")]
    public Beat[] pattern;

    [System.Serializable]
    public class Beat
    {
        [Tooltip("If only one note, second can remain None or SkipOne.")]
        public NoteType first;
        public NoteType second; // optional, use None/SkipOne if unused
    }
}