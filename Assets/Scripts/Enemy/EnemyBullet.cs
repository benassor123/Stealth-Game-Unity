using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float damage = 20f;
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);  // die after a few seconds if dont hit anything
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HUD hud = FindFirstObjectByType<HUD>();

            if (hud != null) hud.TakeDamage(damage);

            Destroy(gameObject);
            return;
        }


        if (collision.CompareTag("Enemy")) return;
        if (collision.GetComponent<SecurityCamera>() != null) return;




        if (!collision.isTrigger) Destroy(gameObject);
    }
}