using System;
using UnityEngine;

[System.Serializable]
public class NoteData
{

    [Tooltip("Time in seconds when this note should be spawned.")]
    public float timestamp;

    [Tooltip("The type of note to spawn (Tap, Swipe, Hold, etc.).")]
    public NoteType noteType;

    [Tooltip("For Hold notes, this is the duration in seconds. For other notes, it can be 0.")]
    public float holdDuration;
}

[CreateAssetMenu(fileName = "NewBeatmap", menuName = "Rhythm Game/New Beatmap")]
public class BeatmapData : ScriptableObject
{
    [Header("Level Information")]
    public AudioClip songClip;
    public string AditionalInfo;

    [Header("Beatmap Data")]
    public NoteData[] notes;
}