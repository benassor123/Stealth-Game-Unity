using UnityEngine;

public class StationaryEnem : MonoBehaviour
{
    [Header("Task")]
    public Transform taskFocusPoint;

    [Header("Sprites")]
    public Sprite taskSpriteA;
    public Sprite taskSpriteB;
    public Sprite chaseSprite;
    public float taskSwapInterval = 0.5f;

    [Header("Detection")]
    public float viewRange = 4f;
    public float viewAngle = 40f;
    public float hearRange = 2f;

    [Header("Chase")]
    public float chaseSpeed = 2.5f;
    public float giveUpTime = 2f;

    Transform player;
    Rigidbody2D playerRb;
    Rigidbody2D rb;
    SpriteRenderer sr;

    string state = "task";
    float alertTimer = 0f;
    float lostTimer = 0f;

    // sprite swap
    float swapTimer = 0f;
    bool showingA = true;

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

        // state logic (no movement here)
        if (state == "task")
        {
            if (taskFocusPoint != null)
                FaceDirection(taskFocusPoint.position - transform.position);

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
            if (Vector2.Distance(transform.position, player.position) < 0.5f)
            {
                Debug.Log("CAUGHT! Game Over");
                Time.timeScale = 0f;
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

    // movement lives in FixedUpdate so physics handles collisions
    void FixedUpdate()
    {
        if (player == null) return;

        if (state == "chase")
        {
            Vector2 newPos = Vector2.MoveTowards(rb.position, player.position, chaseSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            FaceDirection(player.position - transform.position);
        }
        // task and alert states = stand still, no movement
    }

    void UpdateSprite()
    {
        if (sr == null) return;

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