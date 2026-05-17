using UnityEngine;

public class EnemyAI : EnemyBase
{
    [Header("Patrol A-B")]
    public Transform pointA;
    public Transform pointB;

    [Header("Patrol Waypoints")]
    public Transform[] waypoints;
    public bool randomOrder;

    [Header("Patrol Behaviour")]
    public float patrolSpeed = 2f;
    public float pauseAtWaypoint;
    public float lookAroundSpeed = 60f;
    public Sprite normalSprite;

    Transform currentTarget;
    int waypointIndex;
    float pauseTimer;
    float lookDirection = 1f;

    protected override void OnStart()
    {
        if (waypoints != null && waypoints.Length > 1)
        {
            waypointIndex = 0;
            currentTarget = waypoints[0];
        }
        else
        {
            currentTarget = pointA;
        }
    }

    protected override void IdleFixedUpdate()
    {
        if (currentTarget == null) return;

        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.fixedDeltaTime;

            float angle = transform.eulerAngles.z + lookDirection * lookAroundSpeed * Time.fixedDeltaTime;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // look the other way after halfway
            if (pauseTimer < pauseAtWaypoint * 0.5f && lookDirection > 0f)
                lookDirection = -1f;

            return;
        }

        Vector2 newPos = Vector2.MoveTowards(rb.position, currentTarget.position, patrolSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
        FaceDirection(currentTarget.position - transform.position);

        if (Vector2.Distance(transform.position, currentTarget.position) < 0.4f)
        {
            if (pauseAtWaypoint > 0f)
            {
                pauseTimer = pauseAtWaypoint;
                lookDirection = 1f;
            }

            PickNextWaypoint();
        }
    }

    void PickNextWaypoint()
    {
        if (waypoints != null && waypoints.Length > 1)
        {
            if (randomOrder)
            {
                int newIndex;
                do { newIndex = Random.Range(0, waypoints.Length); }
                while (newIndex == waypointIndex);

                waypointIndex = newIndex;
            }
            else
            {
                waypointIndex = (waypointIndex + 1) % waypoints.Length;
            }

            currentTarget = waypoints[waypointIndex];
        }
        else
        {
            if (currentTarget == pointA) currentTarget = pointB;
            else currentTarget = pointA;
        }
    }

    protected override void UpdateSprites()
    {
        if (isPunching) return;
        if (ranged != null && ranged.IsShooting) return;

        if (state == "chase" || state == "alert")
        {
            if (chaseSprite != null) sr.sprite = chaseSprite;
        }
        else
        {
            if (normalSprite != null) sr.sprite = normalSprite;
        }
    }
}