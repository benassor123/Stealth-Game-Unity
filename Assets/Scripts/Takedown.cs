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
        Vector2 toPlayer = ((Vector2)transform.position - (Vector2)target.transform.position).normalized;

        if (Vector2.Dot(facing, toPlayer) < 0f)
            target.OnTakedown();
        else
            Debug.Log("get behind them");
    }

    EnemyBase FindNearestInRange()
    {
        EnemyBase[] enemies = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
        EnemyBase best = null;
        float bestDist = range;

        foreach (EnemyBase e in enemies)
        {
            float d = Vector2.Distance(transform.position, e.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = e;
            }
        }

        return best;
    }
}