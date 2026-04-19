using UnityEngine;

public class EnemyAI : EnemyBase
{
    [Header("Patrol - Classic (A to B)")]
    public Transform pointA;
    public Transform pointB;

    [Header("Patrol - Multi Waypoint (optional, overrides A/B if set)")]
    public Transform[] waypoints;
    public bool randomOrder = false;

    [Header("Patrol Behaviour")]
    public float patrolSpeed = 2f;
    public float pauseAtWaypoint = 0f;   // seconds to pause and 'look around' at each waypoint. 0 = don't pause.
    public float lookAroundSpeed = 60f;  // degrees/second to rotate while paused
    public Sprite normalSprite;

    Transform currentTarget;
    int waypointIndex = 0;
    float pauseTimer = 0f;
    float lookDirection = 1f;

    protected override void OnStart()
    {
        // use waypoint array if set, otherwise fall back to A/B
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

        // pause at waypoint - rotate head to look around, don't move
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.fixedDeltaTime;

            // sweep facing direction left/right to simulate 'checking'
            float angle = transform.eulerAngles.z + lookDirection * lookAroundSpeed * Time.fixedDeltaTime;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // flip direction partway through the pause so they look both ways
            if (pauseTimer < pauseAtWaypoint * 0.5f && lookDirection > 0f)
                lookDirection = -1f;

            return;
        }

        // move toward current target
        Vector2 newPos = Vector2.MoveTowards(rb.position, currentTarget.position, patrolSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
        FaceDirection(currentTarget.position - transform.position);

        // arrived at waypoint
        if (Vector2.Distance(transform.position, currentTarget.position) < 0.4f)
        {
            // start pausing if pause time is set
            if (pauseAtWaypoint > 0f)
            {
                pauseTimer = pauseAtWaypoint;
                lookDirection = 1f;
            }

            // pick next waypoint
            PickNextWaypoint();
        }
    }

    void PickNextWaypoint()
    {
        if (waypoints != null && waypoints.Length > 1)
        {
            if (randomOrder)
            {
                // pick a different one than current
                int newIndex;
                do { newIndex = Random.Range(0, waypoints.Length); }
                while (newIndex == waypointIndex && waypoints.Length > 1);
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
            // classic A <-> B ping pong
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
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