using UnityEngine;

// Attach this alongside any enemy to give them a gun.
// EnemyBase detects and uses it automatically.
public class RangedAttack : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 8f;
    public float shootRange = 6f;
    public float shootCooldown = 1.5f;
    public Sprite shootSprite;

    [Header("Damage Override (optional)")]
    public float damageOverride = 0f;   // if > 0, replaces the bullet prefab's damage. leave at 0 to use default.

    float cooldownTimer = 0f;
    float animTimer = 0f;
    bool isShooting = false;
    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (isShooting)
        {
            animTimer -= Time.deltaTime;
            if (animTimer <= 0f)
                isShooting = false;
        }
    }

    public bool IsShooting { get { return isShooting; } }
    public bool CanShoot { get { return cooldownTimer <= 0f && !isShooting; } }

    public void ShootAt(Vector3 targetPos)
    {
        if (!CanShoot) return;

        cooldownTimer = shootCooldown;
        isShooting = true;
        animTimer = 0.25f;

        if (shootSprite != null && sr != null)
            sr.sprite = shootSprite;

        Vector2 dir = ((Vector2)targetPos - (Vector2)transform.position).normalized;

        if (bulletPrefab != null)
        {
            GameObject b = Instantiate(bulletPrefab, transform.position + (Vector3)(dir * 0.6f), transform.rotation);

            Rigidbody2D brb = b.GetComponent<Rigidbody2D>();
            if (brb != null) brb.linearVelocity = dir * bulletSpeed;

            // apply damage override if set
            if (damageOverride > 0f)
            {
                EnemyBullet eb = b.GetComponent<EnemyBullet>();
                if (eb != null) eb.damage = damageOverride;
            }
        }

        Debug.Log(name + " fired");
    }
}