using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 15f;
    public float fireOffset = 0.5f;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            TryShoot();
    }

    void TryShoot()
    {

        PlayerMov player = GetComponent<PlayerMov>();
        if (player == null || !player.gunDrawn)
        {
            Debug.Log("Draw your gun first! (press G)");
            return;
        }


        HUD hud = FindFirstObjectByType<HUD>();
        if (hud != null && hud.GetAmmo() <= 0)
        {
            Debug.Log("No ammo!");
            return;
        }

        if (hud != null)
            hud.UseAmmo();


        Vector2 dir = transform.right;
        Vector3 spawnPos = transform.position + (Vector3)(dir * fireOffset);

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, transform.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = dir * bulletSpeed;
    }
}