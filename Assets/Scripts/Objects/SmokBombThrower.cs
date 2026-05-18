using UnityEngine;
using UnityEngine.InputSystem;

public class SmokeBombThrower : MonoBehaviour
{
    public GameObject smokePrefab;

    HUD hud;

    void Start()
    {
        hud = FindFirstObjectByType<HUD>();
    }

    void Update()
    {
        if (!Keyboard.current.rKey.wasPressedThisFrame) return;

        if (hud == null) return;

        if (smokePrefab == null) return;

        if (hud.GetSmokeBombs() <= 0) return;

        hud.UseSmokeBomb();
        Instantiate(smokePrefab, transform.position, Quaternion.identity);
    }
}