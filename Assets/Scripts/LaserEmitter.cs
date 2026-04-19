using UnityEngine;

public class LaserEmitter : MonoBehaviour
{
    [Header("Health")]
    public int shotsToDestroy = 1;
    public bool destroyOnPunch = true;

    [Header("Sprites")]
    public Sprite aliveSprite;
    public Sprite destroyedSprite;

    [Header("Optional patrol (for moving lasers)")]
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 1f;

    SpriteRenderer sr;
    int hitsTaken;
    bool destroyed;
    Transform target;    // current patrol target

    public bool IsAlive { get { return !destroyed; } }

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (aliveSprite != null && sr != null) sr.sprite = aliveSprite;

        target = pointA;
    }

    void FixedUpdate()
    {
        if (destroyed) return;
        if (pointA == null || pointB == null) return;

        // move toward current patrol target
        Vector2 newPos = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.fixedDeltaTime);
        transform.position = newPos;

        // reached target? swap
        if (Vector2.Distance(transform.position, target.position) < 0.05f)
            target = (target == pointA) ? pointB : pointA;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (destroyed) return;

        if (other.CompareTag("Bullet"))
        {
            hitsTaken++;
            Destroy(other.gameObject);
            if (hitsTaken >= shotsToDestroy) Break();
        }
    }

    // called by Takedown.cs when the player punches this emitter
    public void PunchHit()
    {
        if (destroyed) return;
        if (destroyOnPunch) Break();
    }

    void Break()
    {
        destroyed = true;
        if (destroyedSprite != null && sr != null) sr.sprite = destroyedSprite;
        Debug.Log(name + " destroyed");
    }
}