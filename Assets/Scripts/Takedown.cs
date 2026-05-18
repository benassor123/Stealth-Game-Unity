using UnityEngine;
using UnityEngine.InputSystem;

public class Takedown : MonoBehaviour
{
    public float range = 1.5f;

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
            TryTakedown();
    }

    void TryTakedown()

    {
        EnemyBase target = FindNearestInRange();

        if (target == null) return;

        Vector2 facing = target.transform.right;

        Vector2 toPlayer = (transform.position - target.transform.position).normalized;



        if (Vector2.Dot(facing, toPlayer) < 0f) // works out angle
            target.OnTakedown();
        else
            Debug.Log("get behind the enmy");
    }
    EnemyBase FindNearestInRange()
    {
        EnemyBase[] enemies = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None); // finds all enemy types
        EnemyBase closest = null;
        float closestDist = range;

        foreach (EnemyBase enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);

            if (dist > range) continue;          // outside range, skip

            if (dist < closestDist)              // closer than our current best
            {
                closest = enemy;
                closestDist = dist;
            }
        }

        return closest;
    }
}