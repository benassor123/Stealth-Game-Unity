using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 3f;

    Transform target;

    void Start()
    {
        target = pointA;
    }

    void Update()
    {
        // move toward current target
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // rotate to face direction
        Vector2 dir = target.position - transform.position;
        if (dir != Vector2.zero)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // swap target when reached
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            target = (target == pointA) ? pointB : pointA;
        }
    }
}