using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class BeatmapGenerator : MonoBehaviour
{
    [Header("Output")]
    [Tooltip("Drag the BeatmapData asset you want to populate here.")]
    public BeatmapData targetBeatmap;

    [Header("Beatmap Settings")]
    [Tooltip("Beats Per Minute for the song.")]
    public int bpm = 120;

    [Tooltip("Total length of the level in seconds.")]
    public float levelDurationSeconds = 60f;

    [Tooltip("How many seconds to wait before the first beat.")]
    public float firstBeatDelay = 2f;

    [Tooltip("The default NoteType to assign to all generated notes.")]
    public NoteType defaultNoteType = NoteType.Tap;


    [ContextMenu("Generate and Populate Beatmap")]
    private void GenerateAndPopulateBeatmap()
    {
        if (targetBeatmap == null)
        {
            Debug.LogError("No Target Beatmap asset assigned! Please drag one into the Inspector slot.");
            return;
        }

        float secondsPerBeat = 60f / bpm;
        List<float> timestamps = new List<float>();
        float currentTime = firstBeatDelay;

        while (currentTime <= levelDurationSeconds)
        {
            timestamps.Add(currentTime);
            currentTime += secondsPerBeat;
        }

        targetBeatmap.notes = new NoteData[timestamps.Count];

        for (int i = 0; i < timestamps.Count; i++)
        {
            targetBeatmap.notes[i] = new NoteData
            {
                timestamp = timestamps[i],
                noteType = defaultNoteType
            };
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(targetBeatmap);
#endif

        Debug.Log("SUCCESS: Populated '" + targetBeatmap.name + "' with " + timestamps.Count + " notes!");
    }
}