using UnityEngine;

public class StationaryEnem : MonoBehaviour
{
    [Header("Task")]
    public Transform taskFocusPoint;

    [Header("Sprites")]
    public Sprite taskSpriteA;
    public Sprite taskSpriteB;
    public Sprite chaseSprite;
    public Sprite punchSprite;
    public float taskSwapInterval = 0.5f;

    [Header("Detection")]
    public float viewRange = 4f;
    public float viewAngle = 40f;
    public float hearRange = 2f;

    [Header("Chase")]
    public float chaseSpeed = 2.5f;
    public float giveUpTime = 2f;

    [Header("Punch")]
    public float punchRange = 0.8f;
    public float punchCooldown = 1f;
    public float punchDamage = 20f;

    Transform player;
    Rigidbody2D playerRb;
    Rigidbody2D rb;
    SpriteRenderer sr;

    string state = "task";
    float alertTimer = 0f;
    float lostTimer = 0f;

    float swapTimer = 0f;
    bool showingA = true;

    float punchTimer = 0f;
    bool isPunching = false;
    float punchAnimTimer = 0f;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        playerRb = player.GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (taskFocusPoint != null)
            FaceDirection(taskFocusPoint.position - transform.position);
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
                if (chaseSprite != null) sr.sprite = chaseSprite;
            }
        }

        if (state == "task")
        {
            if (taskFocusPoint != null)
                FaceDirection(taskFocusPoint.position - transform.position);

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
                state = "task";
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
                    state = "task";
                    lostTimer = 0f;
                }
            }
            else
            {
                lostTimer = 0f;
            }
        }

        UpdateSprite();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        if (state == "chase")
        {
            Vector2 newPos = Vector2.MoveTowards(rb.position, player.position, chaseSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            FaceDirection(player.position - transform.position);
        }
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

    void UpdateSprite()
    {
        if (sr == null) return;

        if (isPunching) return;

        if (state == "chase")
        {
            if (chaseSprite != null) sr.sprite = chaseSprite;
            return;
        }

        if (taskSpriteA == null || taskSpriteB == null) return;

        swapTimer += Time.deltaTime;
        if (swapTimer >= taskSwapInterval)
        {
            swapTimer = 0f;
            showingA = !showingA;
            sr.sprite = showingA ? taskSpriteA : taskSpriteB;
        }
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