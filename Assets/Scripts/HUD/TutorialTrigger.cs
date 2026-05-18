using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TutorialTrigger : MonoBehaviour
{
    [Header("uI")]
    public string title = "Tip";
    [TextArea(2, 4)]
    public string message = " ";

    [Header("UI")]
    public GameObject promptPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI messageText;

    bool triggered = false;
    bool showing = false;

    void Update()
    {
        if (showing && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            promptPanel.SetActive(false);
            Time.timeScale = 1f;
            showing = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        showing = true;

        titleText.text = title;
        messageText.text = message;
        promptPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}