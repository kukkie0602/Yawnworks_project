using UnityEngine;

[CreateAssetMenu(fileName = "NewEnvelopeLevel", menuName = "Envelope Level")]
public class EnvelopeLevel : ScriptableObject
{
    [Header("Level Information")]
    public AudioClip songClip;
    public string levelName;
    public float beatsPerMinute = 120f;
    public Difficulty difficulty;
    public EnvelopeSequence[] sequences;
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard,
    Insane
}