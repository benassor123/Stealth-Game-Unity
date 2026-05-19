using UnityEngine;
using UnityEngine.InputSystem;

public class HealthCanUse : MonoBehaviour
{
    HUD hud;

    void Start()
    {
        hud = FindFirstObjectByType<HUD>();
    }

    void Update()
    {
        if (!Keyboard.current.hKey.wasPressedThisFrame) return;
        if (hud == null) return;
        hud.UseCanister();
    }
}