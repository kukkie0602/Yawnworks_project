using UnityEngine;

public class Envelope : MonoBehaviour
{
    public NoteType noteType;
    public bool needsStampSwap = false;

    [HideInInspector] public int skipNoteID = 0;
    [HideInInspector] public float moveDuration;
}
