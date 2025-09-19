using UnityEngine;

[System.Serializable]
public class EnvelopeSequence
{
    [Tooltip("Always exactly 4 moves per sequence (Tap, Swipe, etc.)")]
    public NoteType[] pattern = new NoteType[4];
}