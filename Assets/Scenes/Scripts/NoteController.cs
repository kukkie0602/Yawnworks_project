using UnityEngine;

public enum NoteType
{
    Tap,
    SwipeUp,
    SwipeDown,
    SwipeLeft,
    SwipeRight,
    Hold
}

public class NoteController : MonoBehaviour
{
    public NoteType noteType = NoteType.Tap;
    public float speed = 5f;

    [Header("Hold Note Components")]
    public float holdDuration = 1f;
    public Transform head;    
    public SpriteRenderer trail;   
    public SpriteRenderer endCap; 

    private CapsuleCollider2D capsuleCollider;

    void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    void Start()
    {
        GetComponent<Rigidbody2D>().linearVelocity = new Vector2(speed, 0);
        Destroy(gameObject, 10f);

        Color noteColor = Color.yellow;
        if (head != null) head.GetComponent<SpriteRenderer>().color = noteColor;
        if (endCap != null) endCap.GetComponent<SpriteRenderer>().color = noteColor;


        if (noteType == NoteType.Hold)
        {
            float trailLength = Mathf.Abs(speed) * holdDuration;

            if (trail != null)
            {
                trail.size = new Vector2(trailLength, trail.size.y);
                trail.transform.localPosition = new Vector3(trailLength / 2f, 0, 0);
            }
            if (head != null)
            {
                head.localPosition = new Vector3(trailLength, 0, 0);
            }

            if (capsuleCollider != null)
            {
                capsuleCollider.size = new Vector2(trailLength + trail.size.y, capsuleCollider.size.y);

                capsuleCollider.offset = new Vector2(trailLength / 2f, 0);
            }
        }
    }

    public void SetActiveState()
    {
        Color activeColor = Color.green;
        if (head != null) head.GetComponent<SpriteRenderer>().color = activeColor;
        if (trail != null) trail.color = activeColor;
        if (endCap != null) endCap.color = activeColor;
    }

    public void SetFailedState()
    {
        Color failedColor = Color.red;
        if (head != null) head.GetComponent<SpriteRenderer>().color = failedColor;
        if (trail != null) trail.color = failedColor;
        if (endCap != null) endCap.color = failedColor;
    }
}