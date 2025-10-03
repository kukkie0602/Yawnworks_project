using UnityEngine;
using System.Collections.Generic;

public class TableHitZone : MonoBehaviour
{
    public FallingEnvelopeLevel levelManager; // reference to the level manager
    public int laneIndex; // which lane this hitbox is
    public float tapRadius = 1f; // radius for tap detection

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

        // --- Mouse Input ---
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
                        break; // only tap one envelope per click
                    }
                }
            }
        }

        // --- Touch Input ---
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
                        break; // only tap one envelope per touch
                    }
                }
            }
        }
    }

}
