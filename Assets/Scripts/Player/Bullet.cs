using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 50f;
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Enemy"))
        {
            EnemyHealth health = other.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            else
            {

                Destroy(other.gameObject);
            }

            Destroy(gameObject);
            return;
        }


        if (!other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}