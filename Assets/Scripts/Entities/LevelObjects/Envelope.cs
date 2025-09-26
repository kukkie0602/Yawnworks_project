using UnityEngine;

public class Envelope : MonoBehaviour
{
    public NoteType noteType;
    public bool needsStampSwap = false;

    [Header("Animator")]
    public Animator animator;

    [HideInInspector] public float moveDuration;
}
