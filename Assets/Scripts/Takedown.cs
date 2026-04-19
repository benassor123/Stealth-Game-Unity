using UnityEngine;
using UnityEngine.InputSystem;

public class Takedown : MonoBehaviour
{
    public float range = 1.5f;
    public GameObject keycardPrefab;

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
            TryTakedown();
    }

    void TryTakedown()
    {
        // first, check for a laser emitter in range - punching these breaks them
        LaserEmitter emitter = FindClosestEmitterInRange();
        if (emitter != null)
        {
            emitter.PunchHit();
            Debug.Log("Punched laser emitter " + emitter.name);
            return;
        }

        // otherwise, try a normal enemy takedown
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject target = null;
        float bestDist = Mathf.Infinity;

        foreach (GameObject e in enemies)
        {
            float d = Vector2.Distance(transform.position, e.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                target = e;
            }
        }

        if (target == null) return;
        if (bestDist > range) return;

        float rot = target.transform.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 facing = new Vector2(Mathf.Cos(rot), Mathf.Sin(rot));
        Vector2 toPlayer = ((Vector2)transform.position - (Vector2)target.transform.position).normalized;
        float dot = Vector2.Dot(facing, toPlayer);

        if (dot < 0f)
        {
            // armoured enemies fail the takedown and raise alarm
            ArmouredEnemy armour = target.GetComponent<ArmouredEnemy>();
            if (armour != null) { armour.OnTakedownBlocked(); return; }

            Debug.Log("Takedown on " + target.name);

            if (target.GetComponent<KeycardHolder>() != null && keycardPrefab != null)
                Instantiate(keycardPrefab, target.transform.position, Quaternion.identity);

            Destroy(target);
        }
        else
        {
            Debug.Log("Need to get behind them");
        }
    }

    LaserEmitter FindClosestEmitterInRange()
    {
        LaserEmitter[] emitters = FindObjectsByType<LaserEmitter>(FindObjectsSortMode.None);
        LaserEmitter best = null;
        float bestDist = range;

        foreach (LaserEmitter em in emitters)
        {
            if (!em.IsAlive) continue;
            float d = Vector2.Distance(transform.position, em.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = em;
            }
        }

        return best;
    }
}