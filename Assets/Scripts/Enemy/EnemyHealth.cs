using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public GameObject healthBarPrefab;
    public float barHeight = 0.7f;

    public float currentHealth;
    GameObject healthBarObj;
    Transform fillBar;
    float originalFillScaleX;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBarPrefab != null)
        {
            healthBarObj = Instantiate(healthBarPrefab);

            fillBar = healthBarObj.transform.Find("Fill");
            if (fillBar != null)
                originalFillScaleX = fillBar.localScale.x;
        }
    }

    void LateUpdate()
    {
        if (healthBarObj != null)
        {
            healthBarObj.transform.position = transform.position + new Vector3(0f, barHeight, 0f);
            healthBarObj.transform.rotation = Quaternion.identity;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0f) currentHealth = 0f;

        Debug.Log(gameObject.name + " HP: " + currentHealth + "/" + maxHealth);

        UpdateBar();

        if (currentHealth <= 0f)
            Die();
    }

    void UpdateBar()
    {
        if (fillBar == null) return;

        float ratio = currentHealth / maxHealth;

        Vector3 scale = fillBar.localScale;
        scale.x = originalFillScaleX * ratio;
        fillBar.localScale = scale;

        float offset = originalFillScaleX * (1f - ratio) * 0.5f;
        fillBar.localPosition = new Vector3(-offset, 0f, 0f);
    }

    public void Die()
    {
        Debug.Log(gameObject.name + " died!");

        EnemyBase enemy = GetComponent<EnemyBase>();
        if (enemy != null && enemy.dropOnDeath != null)
            Instantiate(enemy.dropOnDeath, transform.position, Quaternion.identity);

        if (healthBarObj != null)
            Destroy(healthBarObj);

        Destroy(gameObject);
    }
}