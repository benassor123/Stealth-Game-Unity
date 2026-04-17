using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMov : MonoBehaviour
{
    public float speed = 8f;
    public Sprite idleSprite;
    public Sprite moveSprite;
    public Sprite gunSprite;
    public bool gunDrawn = false;

    Rigidbody2D rb;
    SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Keyboard.current.gKey.wasPressedThisFrame)
            gunDrawn = !gunDrawn;
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
            sr.sprite = gunDrawn ? gunSprite : moveSprite;
        }
        else
        {
            sr.sprite = gunDrawn ? gunSprite : idleSprite;
        }
    }
}