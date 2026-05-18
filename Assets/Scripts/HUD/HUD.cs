using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    [Header("Health")]
    public TextMeshProUGUI healthText;
    public RectTransform healthBarFill;
    public float maxHealth = 100f;
    float currentHealth;
    float healthBarFullWidth;

    [Header("Timer")]
    public TextMeshProUGUI timerText;
    float timer = 0f;

    [Header("Floor")]
    public TextMeshProUGUI floorText;
    public int currentFloor = 1;

    [Header("Inventory")]
    public TextMeshProUGUI keysText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI smokeText;
    int ammo = 12;
    int smokeBombs = 2;

    [Header("Pause")]
    public GameObject pausePanel;
    bool paused = false;

    void Start()
    {
        currentHealth = maxHealth;
        Time.timeScale = 1f;

        if (pausePanel != null) pausePanel.SetActive(false);

        if (healthBarFill == null) return;
        healthBarFullWidth = healthBarFill.rect.width;
        UpdateHealthUI();
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame) TogglePause();
        if (paused) return;

        timer += Time.deltaTime;

        if (timerText != null)
        {
            int mins = (int)(timer / 60f);
            int secs = (int)(timer % 60f);

            string minsStr = mins.ToString();
            string secsStr = secs.ToString();
            if (mins < 10) minsStr = "0" + mins;
            if (secs < 10) secsStr = "0" + secs;
            timerText.text = minsStr + ":" + secsStr;
        }

        if (floorText != null) floorText.text = "Floor " + currentFloor;
        if (keysText != null) keysText.text = "x " + Keycard.keycardCount;
        if (ammoText != null) ammoText.text = "x " + ammo;
        if (smokeText != null) smokeText.text = "x " + smokeBombs;
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            string current = ((int)currentHealth).ToString();
            string max = ((int)maxHealth).ToString();
            healthText.text = current + " / " + max;
        }

        if (healthBarFill == null) return;
        float ratio = currentHealth / maxHealth;
        healthBarFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, healthBarFullWidth * ratio);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0f) currentHealth = 0f;
        UpdateHealthUI();

        if (currentHealth > 0f) return;
        Debug.Log("DEAD - Game Over");
        Time.timeScale = 0f;
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public int GetAmmo()
    {
        return ammo;
    }

    public void UseAmmo()
    {
        if (ammo > 0) ammo--;
    }

    public void AddAmmo(int amount)
    {
        ammo += amount;
    }

    public void UseSmokeBomb()
    {
        if (smokeBombs > 0) smokeBombs--;
    }

    public void AddSmokeBomb(int amount)
    {
        smokeBombs += amount;
    }

    public void SetFloor(int floor)
    {
        currentFloor = floor;
    }

    public void TogglePause()
    {
        paused = !paused;

        if (paused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;

        if (pausePanel != null) pausePanel.SetActive(paused);
    }

    public int GetSmokeBombs()
    {
        return smokeBombs;
    }
}