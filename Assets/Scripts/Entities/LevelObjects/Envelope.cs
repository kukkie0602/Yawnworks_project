using UnityEngine;

public class Envelope : MonoBehaviour
{
    public double targetDspTime;
    public NoteType noteType;
    public bool needsStampSwap = false;
    public bool isHalfNote;
    [HideInInspector] public int skipNoteID = 0;
    [HideInInspector] public float moveDuration;
    [HideInInspector] public bool isTapped = false;
}
