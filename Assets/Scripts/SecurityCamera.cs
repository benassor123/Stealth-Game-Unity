using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite onSprite;
    public Sprite alertSprite;
    public Sprite offSprite;

    [Header("Sweep")]
    public float sweepHalfAngle = 45f;
    public float rotateSpeed = 25f;
    public float pauseAtEnd = 1f;

    [Header("Detection")]
    public float viewRange = 5f;
    public float viewAngle = 25f;
    public float detectTime = 1f;
    public LayerMask wallLayer;

    [Header("Alert")]
    public float alertRadius = 10f;
    public float alertHoldDuration = 1.5f;
    public AudioClip alertSound;

    Transform player;
    SpriteRenderer sr;
    float baseAngle;
    float currentAngle;
    int direction = 1;
    float pauseTimer = 0f;
    float detectTimer = 0f;
    float alertHoldTimer = 0f;
    bool disabled = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        sr = GetComponent<SpriteRenderer>();

        baseAngle = transform.eulerAngles.z;
        currentAngle = baseAngle - sweepHalfAngle;
        ApplyRotation();

        if (onSprite != null && sr != null) sr.sprite = onSprite;
    }

    void Update()
    {
        if (disabled || player == null) return;

        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
        }
        else
        {
            currentAngle += direction * rotateSpeed * Time.deltaTime;
            float maxAngle = baseAngle + sweepHalfAngle;
            float minAngle = baseAngle - sweepHalfAngle;

            if (direction > 0 && currentAngle >= maxAngle)
            {
                currentAngle = maxAngle;
                direction = -1;
                pauseTimer = pauseAtEnd;
            }
            else if (direction < 0 && currentAngle <= minAngle)
            {
                currentAngle = minAngle;
                direction = 1;
                pauseTimer = pauseAtEnd;
            }
            ApplyRotation();
        }

        bool sees = CanSeePlayer();

        if (sees)
        {
            detectTimer += Time.deltaTime;
            alertHoldTimer = alertHoldDuration;

            if (detectTimer >= detectTime)
            {
                TriggerAlert();
                detectTimer = 0f;
            }
        }
        else
        {
            detectTimer = Mathf.Max(0f, detectTimer - Time.deltaTime * 0.5f);
            if (alertHoldTimer > 0f)
                alertHoldTimer -= Time.deltaTime;
        }

        if (sr != null)
        {
            if (sees || alertHoldTimer > 0f)
            {
                if (alertSprite != null) sr.sprite = alertSprite;
            }
            else
            {
                if (onSprite != null) sr.sprite = onSprite;
            }
        }
    }

    void ApplyRotation()
    {
        transform.rotation = Quaternion.Euler(0, 0, currentAngle);
    }

    bool CanSeePlayer()
    {
        Vector2 dirToPlayer = player.position - transform.position;
        float dist = dirToPlayer.magnitude;
        if (dist > viewRange) return false;

        float angle = Vector2.Angle(transform.right, dirToPlayer);
        if (angle > viewAngle) return false;

        if (wallLayer.value != 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer.normalized, dist, wallLayer);
            if (hit.collider != null) return false;
        }

        return true;
    }

    void TriggerAlert()
    {
        Debug.Log(name + " spotted player at " + player.position);
        // cameras always share reliable position
        EnemyBase.ForceChaseNearby(transform.position, alertRadius, player.position);

        if (alertSound != null)
            AudioSource.PlayClipAtPoint(alertSound, transform.position);
    }

    public void Disable()
    {
        disabled = true;
        if (offSprite != null && sr != null) sr.sprite = offSprite;
        Debug.Log(name + " disabled");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (disabled) return;
        if (other.CompareTag("Bullet"))
        {
            Disable();
            Destroy(other.gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 forward = transform.right * viewRange;
        Vector3 left = Quaternion.Euler(0, 0, viewAngle) * forward;
        Vector3 right = Quaternion.Euler(0, 0, -viewAngle) * forward;
        Gizmos.DrawLine(transform.position, transform.position + left);
        Gizmos.DrawLine(transform.position, transform.position + right);
    }
}