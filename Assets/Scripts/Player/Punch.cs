using UnityEngine;
using UnityEngine.InputSystem;

public class Punch : MonoBehaviour
{
    public float range = 1.2f;
    public Sprite punchSprite;


    public float damageDealt = 10f;

    public float punchCooldown = 0.4f;
    float punchTimer;

    SpriteRenderer sr;
    PlayerMov mov;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        mov = GetComponent<PlayerMov>();
    }

    void Update()
    {
        if (punchTimer > 0f) punchTimer -= Time.deltaTime;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        if (mov == null) return;
        if (mov.gunDrawn) return;
        if (punchTimer > 0f) return;

        DoPunch();
    }

    void DoPunch()
    {
        punchTimer = punchCooldown;
        if (punchSprite != null) sr.sprite = punchSprite;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageDealt);
                continue;
            }

            LaserEmitter emitter = hit.GetComponent<LaserEmitter>();
            if (emitter != null)
            {
                emitter.PunchHit();
                continue;
            }
        }
    }
}