using UnityEngine;
using System.Collections.Generic;

public class TableHitZone : MonoBehaviour
{
    public FallingEnvelopeLevel levelManager;
    public int laneIndex;
    public float tapRadius = 1f; 

    private HashSet<Envelope> envelopesInZone = new HashSet<Envelope>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        Envelope e = other.GetComponent<Envelope>();
        if (e != null)
        {
            envelopesInZone.Add(e);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Envelope e = other.GetComponent<Envelope>();
        if (e != null)
        {
            envelopesInZone.Remove(e);
        }
    }

    private void Update()
    {
        if (envelopesInZone.Count == 0) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            foreach (var e in envelopesInZone)
            {
                if (!e.isTapped)
                {
                    Collider2D col = e.GetComponent<Collider2D>();
                    if (col != null && col.OverlapPoint(mousePos))
                    {
                        levelManager.ProcessSuccessfulAction(e.gameObject);
                        break;
                    }
                }
            }
        }

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase != TouchPhase.Began) continue;

            Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            touchPos.z = 0f;

            foreach (var e in envelopesInZone)
            {
                if (!e.isTapped)
                {
                    Collider2D col = e.GetComponent<Collider2D>();
                    if (col != null && col.OverlapPoint(touchPos))
                    {
                        levelManager.ProcessSuccessfulAction(e.gameObject);
                        break;
                    }
                }
            }
        }
    }

}
