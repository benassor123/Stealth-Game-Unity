using UnityEngine;

public class PatrolEnemy : EnemyBase
{
    [Header("Patrol A-B")]
    public Transform pointA;
    public Transform pointB;

    [Header("Patrol Waypoints")]
    public Transform[] waypoints;
    public bool randomOrder;

    [Header("Patrol Behaviour")]
    public float patrolSpeed = 2f;
    public Sprite normalSprite;

    Transform currentTarget;
    int waypointIndex;

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

        Vector2 newPos = Vector2.MoveTowards(rb.position, currentTarget.position, patrolSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
        FaceDirection(currentTarget.position - transform.position);

        if (Vector2.Distance(transform.position, currentTarget.position) < 0.4f)
            PickNextWaypoint();
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