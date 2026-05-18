using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [Header("Sweep")]
    public float sweepHalfAngle = 45f;
    public float rotateSpeed = 25f;

    [Header("Detection")]
    public float viewRange = 5f;
    public float viewAngle = 25f;
    public LayerMask wallLayer;

    [Header("Alert")]
    public float alertRadius = 10f;

    Transform player;
    float currentOffset;
    int direction = 1;
    bool disabled;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;

        currentOffset = -sweepHalfAngle;
        transform.Rotate(0, 0, currentOffset);
    }

    void Update()
    {
        if (disabled || player == null) return;

        // sweep back and forth
        float step = direction * rotateSpeed * Time.deltaTime;
        transform.Rotate(0, 0, step);
        currentOffset += step;

        if (currentOffset > sweepHalfAngle) direction = -1;
        else if (currentOffset < -sweepHalfAngle) direction = 1;

        if (CanSeePlayer())
            TriggerAlert();
    }

    bool CanSeePlayer()
    {
        Vector2 dir = player.position - transform.position;
        float dist = dir.magnitude;
        if (dist > viewRange) return false;

        float angle = Vector2.Angle(transform.right, dir);
        if (angle > viewAngle) return false;

        if (wallLayer.value != 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir.normalized, dist, wallLayer);
            if (hit.collider != null) return false;
        }

        return true;
    }

    void TriggerAlert()
    {
        Debug.Log(name + " spotted player");
        EnemyBase.ForceChaseNearby(transform.position, alertRadius, player.position);
    }

    public void Disable()
    {
        disabled = true;
        Debug.Log(name + " disabled");
    }
}