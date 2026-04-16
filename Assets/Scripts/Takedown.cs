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
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // find which enemy is closest
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

        // work out which way the enemy is facing using their rotation
        float rot = target.transform.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 facing = new Vector2(Mathf.Cos(rot), Mathf.Sin(rot));

        // direction from the enemy to the player
        Vector2 toPlayer = ((Vector2)transform.position - (Vector2)target.transform.position).normalized;

        // dot product tells us if we are in front or behind
        // negative = behind, positive = in front
        float dot = Vector2.Dot(facing, toPlayer);

        if (dot < 0f)
        {
            Debug.Log("Takedown on " + target.name);

            // drop keycard if this enemy has one
            if (target.GetComponent<KeycardHolder>() != null && keycardPrefab != null)
                Instantiate(keycardPrefab, target.transform.position, Quaternion.identity);

            Destroy(target);
        }
        else
        {
            Debug.Log("Need to get behind them");
        }
    }
}