using UnityEngine;

[System.Serializable]
public class EnvelopeSequence
{
    [Tooltip("Always exactly 4 moves per sequence (Tap, Swipe, etc.)")]
    public NoteType[] pattern = new NoteType[4];

    [Header("Timing Settings")]
    [Tooltip("How long it takes each envelope to move between conveyor positions")]
    public float moveDuration = 0.1f;
}