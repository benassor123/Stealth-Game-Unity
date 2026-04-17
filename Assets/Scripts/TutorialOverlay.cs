using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialOverlay : MonoBehaviour
{
    public CanvasGroup overlayGroup;

    void Start()
    {
        overlayGroup.alpha = 1f;
        overlayGroup.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            overlayGroup.gameObject.SetActive(false);
            Time.timeScale = 1f;
            enabled = false;
        }
    }
}