using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float damage = 20f;
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HUD hud = FindFirstObjectByType<HUD>();
            if (hud != null) hud.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Enemy")) return;
        if (other.GetComponent<SecurityCamera>() != null) return;

        if (!other.isTrigger)
            Destroy(gameObject);
    }
}