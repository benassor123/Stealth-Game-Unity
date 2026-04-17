using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 2.5f;
    public float viewRange = 4f;
    public float viewAngle = 40f;
    public float hearRange = 2f;
    public float giveUpTime = 2f;

    [Header("Punch")]
    public Sprite normalSprite;
    public Sprite punchSprite;
    public float punchRange = 0.8f;
    public float punchCooldown = 1f;
    public float punchDamage = 20f;

    Transform player;
    Rigidbody2D playerRb;
    Rigidbody2D rb;
    SpriteRenderer sr;
    Transform patrolTarget;
    string state = "patrol";
    float alertTimer = 0f;
    float lostTimer = 0f;
    float punchTimer = 0f;
    bool isPunching = false;
    float punchAnimTimer = 0f;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        playerRb = player.GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        patrolTarget = pointA;
    }

    void Update()
    {
        if (player == null) return;

        if (punchTimer > 0f)
            punchTimer -= Time.deltaTime;

        if (isPunching)
        {
            punchAnimTimer -= Time.deltaTime;
            if (punchAnimTimer <= 0f)
            {
                isPunching = false;
                if (normalSprite != null) sr.sprite = normalSprite;
            }
        }

        if (state == "patrol")
        {
            if (Vector2.Distance(transform.position, player.position) <= punchRange)
            {
                state = "chase";
                return;
            }
            if (CanSeePlayer())
            {
                state = "chase";
                return;
            }
            if (CanHearPlayer())
            {
                state = "alert";
                alertTimer = 1.5f;
            }
        }
        else if (state == "alert")
        {
            FaceDirection(player.position - transform.position);
            alertTimer -= Time.deltaTime;

            if (Vector2.Distance(transform.position, player.position) <= punchRange)
            {
                state = "chase";
                return;
            }
            if (CanSeePlayer())
            {
                state = "chase";
                return;
            }
            if (alertTimer <= 0f)
                state = "patrol";
        }
        else if (state == "chase")
        {
            float distToPlayer = Vector2.Distance(transform.position, player.position);

            if (distToPlayer <= punchRange)
            {
                lostTimer = 0f;
                if (punchTimer <= 0f)
                    Punch();
                return;
            }

            if (!CanSeePlayer())
            {
                lostTimer += Time.deltaTime;
                if (lostTimer > giveUpTime)
                {
                    state = "patrol";
                    lostTimer = 0f;
                }
            }
            else
            {
                lostTimer = 0f;
            }
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        if (state == "patrol") Patrol();
        else if (state == "chase") Chase();
    }

    void Patrol()
    {
        Vector2 newPos = Vector2.MoveTowards(rb.position, patrolTarget.position, patrolSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
        FaceDirection(patrolTarget.position - transform.position);

        if (Vector2.Distance(transform.position, patrolTarget.position) < 0.4f)
            patrolTarget = (patrolTarget == pointA) ? pointB : pointA;
    }

    void Chase()
    {
        Vector2 newPos = Vector2.MoveTowards(rb.position, player.position, chaseSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
        FaceDirection(player.position - transform.position);
    }

    void Punch()
    {
        punchTimer = punchCooldown;
        isPunching = true;
        punchAnimTimer = 0.3f;

        if (punchSprite != null) sr.sprite = punchSprite;

        HUD hud = FindFirstObjectByType<HUD>();
        if (hud != null)
            hud.TakeDamage(punchDamage);

        Debug.Log(gameObject.name + " punched the player!");
    }

    bool CanSeePlayer()
    {
        Vector2 dirToPlayer = player.position - transform.position;
        float dist = dirToPlayer.magnitude;
        if (dist > viewRange) return false;

        float angle = Vector2.Angle(transform.right, dirToPlayer);
        if (angle > viewAngle) return false;

        return true;
    }

    bool CanHearPlayer()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > hearRange) return false;

        if (playerRb.linearVelocity.magnitude > 0.1f)
            return true;

        return false;
    }

    void FaceDirection(Vector3 dir)
    {
        if (dir == Vector3.zero) return;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}