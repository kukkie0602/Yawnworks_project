using System;
using UnityEngine;

[System.Serializable]
public class NoteData
{

    [Tooltip("Time in seconds when this note should be spawned.")]
    public float timestamp;

    [Tooltip("The type of note to spawn (Tap, Swipe, Hold, etc.).")]
    public NoteType noteType;
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard,
    Insane
}

[CreateAssetMenu(fileName = "NewBeatmap", menuName = "Rhythm Game/New Beatmap")]
public class BeatmapData : ScriptableObject
{
    [Header("Level Information")]
    public AudioClip songClip;
    public string levelName;

    public Difficulty difficulty;

    [Header("Beatmap Data")]
    public NoteData[] notes;
}