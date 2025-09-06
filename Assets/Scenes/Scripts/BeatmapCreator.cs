using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // ADD THIS LINE for the new system

public class BeatmapCreator : MonoBehaviour
{
    public AudioSource audioSource;
    private List<float> timestamps = new List<float>();

    void Awake()
    {
        // Only enable this script in the Inspector when you want to create a new beatmap.
        if (!enabled)
        {
            return;
        }
    }

    void Update()
    {
        // Get the current state of the keyboard
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return; // Exit if no keyboard is connected
        }

        // Check if the Space key was pressed THIS FRAME
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            timestamps.Add(audioSource.time);
            Debug.Log("Tap registered at: " + audioSource.time);
        }

        // Check if the 'S' key was pressed THIS FRAME to save
        if (keyboard.sKey.wasPressedThisFrame)
        {
            SaveBeatmap();
        }
    }

    void SaveBeatmap()
    {
        string beatmapData = "";
        foreach (float time in timestamps)
        {
            beatmapData += time.ToString("F3") + "f, ";
        }

        Debug.Log("--- BEATMAP FINISHED ---");
        Debug.Log("Copy the line below and paste it into your NoteSpawner script:");
        Debug.Log(beatmapData);
        Debug.Log("------------------------");

        this.enabled = false;
    }
}