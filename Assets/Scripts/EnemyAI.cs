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

    Transform player;
    Rigidbody2D playerRb;
    Rigidbody2D rb;
    Transform patrolTarget;
    string state = "patrol";
    float alertTimer = 0f;
    float lostTimer = 0f;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        playerRb = player.GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        patrolTarget = pointA;
    }

    void Update()
    {
        if (player == null) return;

        // detection / state switching (logic only, no movement)
        if (state == "patrol")
        {
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
                state = "patrol";
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

    // movement lives in FixedUpdate so physics handles collisions
    void FixedUpdate()
    {
        if (player == null) return;

        if (state == "patrol") Patrol();
        else if (state == "chase") Chase();
        // alert state = stand still, so no movement
    }

    void Patrol()
    {
        Vector2 newPos = Vector2.MoveTowards(rb.position, patrolTarget.position, patrolSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        FaceDirection(patrolTarget.position - transform.position);

        if (Vector2.Distance(transform.position, patrolTarget.position) < 0.3f)
            patrolTarget = (patrolTarget == pointA) ? pointB : pointA;
    }

    void Chase()
    {
        Vector2 newPos = Vector2.MoveTowards(rb.position, player.position, chaseSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
        FaceDirection(player.position - transform.position);
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