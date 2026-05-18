using UnityEngine;
using UnityEngine.InputSystem;

public class Chest : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite closedSprite;
    public Sprite openSprite;

    [Header("Settings")]
    public bool requiresKeycard = true;

    SpriteRenderer sr;
    bool isOpen = false;
    bool playerInRange = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (closedSprite == null) return;
        sr.sprite = closedSprite;
    }

    void Update()
    {
        if (isOpen) return;
        if (!playerInRange) return;
        if (!Keyboard.current.eKey.wasPressedThisFrame) return;

        TryOpen();
    }

    void TryOpen()
    {
        if (requiresKeycard && Keycard.keycardCount <= 0)
        {
            return;
        }

        if (requiresKeycard) Keycard.keycardCount--;

        isOpen = true;
        if (openSprite != null) sr.sprite = openSprite;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;
        playerInRange = true;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;
        playerInRange = false;
    }
}