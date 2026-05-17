using UnityEngine;

public class RangedAttack : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 8f;
    public float shootRange = 6f;
    public float shootCooldown = 1.5f;
    public Sprite shootSprite;
    public float damageOverride = 0f;

    float cooldownTimer;
    float animTimer;
    bool isShooting;
    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (isShooting)
        {
            animTimer -= Time.deltaTime;
            if (animTimer <= 0f) isShooting = false;
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

        if (shootSprite != null && sr != null) sr.sprite = shootSprite;

        Vector2 dir = (targetPos - transform.position).normalized;

        if (bulletPrefab == null) return;

        Vector3 spawnPos = transform.position + (Vector3)(dir * 0.5f);
        GameObject b = Instantiate(bulletPrefab, spawnPos, transform.rotation);

        Rigidbody2D brb = b.GetComponent<Rigidbody2D>();
        if (brb != null) brb.linearVelocity = dir * bulletSpeed;

        if (damageOverride > 0f)
        {
            EnemyBullet eb = b.GetComponent<EnemyBullet>();
            if (eb != null) eb.damage = damageOverride;
        }

        Debug.Log(name + " fired");
    }
}