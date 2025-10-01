using UnityEngine;

public class Envelope : MonoBehaviour
{
    public NoteType noteType;
    public bool needsStampSwap = false;
    public bool isHalfNote;
    [Header("Animator")]
    public Animator animator;
    [HideInInspector] public int skipNoteID = 0;
    [HideInInspector] public float moveDuration;

    [HideInInspector] public bool isTapped = false;
}
