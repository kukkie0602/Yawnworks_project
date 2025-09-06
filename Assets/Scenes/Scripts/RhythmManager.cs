using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RhythmManager : MonoBehaviour
{
    [Header("Score Manager")]
    public ScoreManager scoreManager;

    [Header("Swipe Tuning")]
    public float minSwipeDistance = 50f; 
    public float maxSwipeTime = 1.0f; 

    private List<NoteController> notesInTrigger = new List<NoteController>();
    private Collider2D targetCollider;

    private Vector2 swipeStartPos;
    private float swipeStartTime;

    private bool isHoldingNote = false;
    private NoteController activeHoldNote = null;

    void Start()
    {
        targetCollider = GetComponent<Collider2D>();
        if (scoreManager != null) scoreManager.ResetScore();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            HandleTouch(Input.GetTouch(0));
        }
        else
        {
            if (Input.GetMouseButtonDown(0)) HandleTouchBegan(Input.mousePosition);
            if (Input.GetMouseButtonUp(0)) HandleTouchEnded(Input.mousePosition);
        }

        if (Input.touchCount == 0 && !Input.GetMouseButton(0) && isHoldingNote)
        {
            StartCoroutine(ProcessFailedNote(activeHoldNote, "Finger lifted from screen!"));
        }
    }

    void HandleTouch(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began: HandleTouchBegan(touch.position); break;
            case TouchPhase.Ended: HandleTouchEnded(touch.position); break;
        }
    }

    void HandleTouchBegan(Vector2 touchPos)
    {
        swipeStartPos = touchPos;
        swipeStartTime = Time.time;

        if (notesInTrigger.Count > 0 && notesInTrigger[0].noteType == NoteType.Hold)
        {
            isHoldingNote = true;
            activeHoldNote = notesInTrigger[0];
            activeHoldNote.SetActiveState();
            Debug.Log("Hold Started!");
        }
    }

    void HandleTouchEnded(Vector2 touchPos)
    {
        if (isHoldingNote)
        {
            if (activeHoldNote != null && activeHoldNote.endCap != null)
            {
                if (targetCollider.OverlapPoint(activeHoldNote.endCap.transform.position))
                {
                    Debug.Log("Hold Success!");
                    ProcessSuccessfulNote(activeHoldNote);
                }
                else
                {
                    StartCoroutine(ProcessFailedNote(activeHoldNote, "Released too early or late."));
                }
            }
            isHoldingNote = false;
            activeHoldNote = null;
        }
        else
        {
            float swipeDuration = Time.time - swipeStartTime;
            float swipeDistance = Vector2.Distance(swipeStartPos, touchPos);
            if (swipeDuration < maxSwipeTime && swipeDistance > minSwipeDistance) ProcessSwipe(touchPos - swipeStartPos);
            else ProcessTap();
        }
    }

    void ProcessSuccessfulNote(NoteController note)
    {
        if (scoreManager != null)
        {
            scoreManager.IncreaseScore();
        }
        DestroyNote(note);
    }

    IEnumerator ProcessFailedNote(NoteController note, string reason)
    {
        Debug.Log("Failed: " + reason);

        if (note != null) note.SetFailedState();
        if (scoreManager != null) scoreManager.ResetScore();

        if (note == activeHoldNote)
        {
            isHoldingNote = false;
            activeHoldNote = null;
        }

        yield return new WaitForSeconds(0.25f);

        if (note != null) DestroyNote(note);
    }

    void DestroyNote(NoteController noteToDestroy)
    {
        if (noteToDestroy == null) return;

        if (notesInTrigger.Contains(noteToDestroy))
        {
            notesInTrigger.Remove(noteToDestroy);
        }

        if (noteToDestroy == activeHoldNote)
        {
            isHoldingNote = false;
            activeHoldNote = null;
        }

        Destroy(noteToDestroy.gameObject);
    }

    void ProcessTap()
    {
        if (notesInTrigger.Count > 0 && notesInTrigger[0].noteType == NoteType.Tap)
        {
            Debug.Log("Hit Tap Note!");
            ProcessSuccessfulNote(notesInTrigger[0]);
        }
    }

    void ProcessSwipe(Vector2 swipeDirection)
    {
        if (notesInTrigger.Count > 0)
        {
            NoteController note = notesInTrigger[0];
            NoteType requiredType = GetRequiredSwipeType(swipeDirection);
            if (note.noteType == requiredType)
            {
                Debug.Log("Hit Swipe Note!");
                ProcessSuccessfulNote(note);
            }
        }
    }

    NoteType GetRequiredSwipeType(Vector2 swipeDirection)
    {
        swipeDirection.Normalize();
        if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
        {
            return swipeDirection.x > 0 ? NoteType.SwipeRight : NoteType.SwipeLeft;
        }
        else
        {
            return swipeDirection.y > 0 ? NoteType.SwipeUp : NoteType.SwipeDown;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Note"))
        {
            NoteController note = other.GetComponent<NoteController>();
            if (note != null && !notesInTrigger.Contains(note))
            {
                notesInTrigger.Add(note);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Note"))
        {
            NoteController note = other.GetComponent<NoteController>();
            if (note != null && notesInTrigger.Contains(note))
            {
                notesInTrigger.Remove(note);
            }
        }
    }

    //test
}