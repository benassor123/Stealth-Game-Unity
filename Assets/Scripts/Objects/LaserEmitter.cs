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
    bool headingToB;

    public bool IsAlive
    {
        get { return !destroyed; }
    }

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        headingToB = false;

        if (aliveSprite == null) return;
        if (sr == null) return;
        sr.sprite = aliveSprite;
    }

    void FixedUpdate()
    {
        if (destroyed) return;
        if (pointA == null || pointB == null) return;

        Transform target = pointA;
        if (headingToB) target = pointB;

        Vector2 newPos = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.fixedDeltaTime);
        transform.position = newPos;

        if (Vector2.Distance(transform.position, target.position) < 0.05f)
        {
            headingToB = !headingToB;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (destroyed) return;
        if (!other.CompareTag("Bullet")) return;

        hitsTaken++;
        Destroy(other.gameObject);
        if (
            hitsTaken >= shotsToDestroy
            )
            Break();
    }

    public void PunchHit()
    {
        if (destroyed) return;
        if (destroyOnPunch) Break();
    }

    void Break()
    {
        destroyed = true;
        if (destroyedSprite != null && sr != null) sr.sprite = destroyedSprite;
        Debug.Log(name + "destroyed");
    }
}