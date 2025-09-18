using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TimingIndicator : MonoBehaviour
{
    public float speed = 10f;

    void Start()
    {
        GetComponent<Rigidbody2D>().linearVelocity = new Vector2(speed, 0);
        Destroy(gameObject, 5f); // Self-destruct after 5 seconds
    }
}