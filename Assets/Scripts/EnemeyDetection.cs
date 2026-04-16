using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public float viewRange = 6f;
    public float viewAngle = 50f;

    Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        // draw a red line showing where enemy is looking (visible in Scene view)
        Debug.DrawRay(transform.position, transform.right * viewRange, Color.red);

        Vector2 dirToPlayer = player.position - transform.position;
        float dist = dirToPlayer.magnitude;

        if (dist > viewRange) return;

        Vector2 facingDir = transform.right;
        float angle = Vector2.Angle(facingDir, dirToPlayer);

        if (angle < viewAngle)
        {
            Debug.Log("SPOTTED!");
            Time.timeScale = 0f;
        }
    }
}