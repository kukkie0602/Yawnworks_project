using UnityEngine;

[CreateAssetMenu(fileName = "NewEnvelopeLevel", menuName = "Envelope Level")]
public class EnvelopeLevel : ScriptableObject
{
    public EnvelopeSequence[] sequences;
}