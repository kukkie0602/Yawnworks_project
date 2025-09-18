using UnityEngine;

[System.Serializable]
public class NotePrefabMapping
{
    public NoteType noteType;
    [Tooltip("The prefab for the envelope on the conveyor belt.")]
    public GameObject envelopePrefab;
}