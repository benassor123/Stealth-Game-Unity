using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 50f;    // 50 = two shots to kill (50% per hit)
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // hit an enemy
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth health = other.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            else
            {
                // no health script, just destroy (fallback)
                Destroy(other.gameObject);
            }

            Destroy(gameObject);
            return;
        }

        // hit a wall or anything that isn't the player
        if (!other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}