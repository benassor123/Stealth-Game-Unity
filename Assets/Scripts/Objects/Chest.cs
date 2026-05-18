using UnityEngine;
using UnityEngine.InputSystem;

public class Chest : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite closedSprite;
    public Sprite openSprite;

    [Header("Settings")]
    public bool requiresKeycard = true;

    [Header("Loot")]
    public int minAmmo = 0;
    public int maxAmmo = 3;
    public int minSmoke = 0;
    public int maxSmoke = 2;
    public int minHealth = 0;
    public int maxHealth = 30;

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
        if (requiresKeycard && Keycard.keycardCount <= 0) return;
        if (requiresKeycard) Keycard.keycardCount--;

        isOpen = true;
        if (openSprite != null) sr.sprite = openSprite;

        GiveLoot();
    }

    void GiveLoot()
    {
        HUD hud = FindFirstObjectByType<HUD>();
        if (hud == null) return;

        int ammo = Random.Range(minAmmo, maxAmmo + 1);

        int smoke = Random.Range(minSmoke, maxSmoke + 1);
        int health = Random.Range(minHealth, maxHealth + 1);

        hud.AddAmmo(ammo);
        hud.AddSmokeBomb(smoke);
        hud.Heal(health);

        Debug.Log("The Chest gave: " + ammo + " ammo, " + smoke + " smoke, " + health + " hp");
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