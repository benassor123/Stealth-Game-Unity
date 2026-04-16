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
        if (closedSprite != null) sr.sprite = closedSprite;
    }

    void Update()
    {
        if (isOpen) return;
        if (!playerInRange) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("E pressed near chest | keycards: " + Keycard.keycardCount + " | requires: " + requiresKeycard);
            TryOpen();
        }
    }

    void TryOpen()
    {
        if (requiresKeycard && Keycard.keycardCount <= 0)
        {
            Debug.Log("Chest LOCKED – need a keycard! (have: " + Keycard.keycardCount + ")");
            return;
        }

        if (requiresKeycard)
            Keycard.keycardCount--;

        isOpen = true;
        if (openSprite != null) sr.sprite = openSprite;
        Debug.Log("Chest OPENED!");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Chest trigger: " + other.name + " tag: " + other.tag);
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    // fallback if Is Trigger isn't ticked
    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Chest collision: " + col.gameObject.name + " tag: " + col.gameObject.tag);
        if (col.gameObject.CompareTag("Player"))
            playerInRange = true;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
            playerInRange = false;
    }
}