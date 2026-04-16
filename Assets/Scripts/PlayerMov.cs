using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMov : MonoBehaviour
{
    public float speed = 8f;
    public Sprite idleSprite;
    public Sprite moveSprite;

    Rigidbody2D rb;
    SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        var kb = Keyboard.current;
        float x = 0f, y = 0f;

        if (kb.wKey.isPressed || kb.upArrowKey.isPressed) y = 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed) y = -1f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) x = -1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) x = 1f;

        Vector2 dir = new Vector2(x, y).normalized;
        rb.linearVelocity = dir * speed;

        if (dir != Vector2.zero)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            sr.sprite = moveSprite;
        }
        else
        {
            sr.sprite = idleSprite;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            HUD hud = FindObjectOfType<HUD>();
            if (hud != null)
                hud.TakeDamage(100f);
        }
    }
}