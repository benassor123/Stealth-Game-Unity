using UnityEngine;

public class ArmouredEnemy : PatrolEnemy
{
    [Header("Armour")]
    public float backupRadius = 8f;

    [Header("Enrage")]
    public bool canEnrage = false;
    public float enrageHpThreshold = 50f;
    public float enrageSpeedBoost = 1.5f;
    public float enrageDamageBoost = 2f;
    public float enrageSizeBoost = 1.5f;
    public float spawnInterval = 5f;
    public GameObject backupPrefab;
    public Transform leftSpawn;
    public Transform rightSpawn;

    bool enraged;
    float spawnTimer;

    public override void OnTakedown()
    {
        Vector2 playerPos = transform.position;
        if (player != null) playerPos = player.position;

        ForceChase(playerPos);
        ForceChaseNearby(transform.position, backupRadius, playerPos);

        if (chatter != null) chatter.Say("Nice try!");
    }

    protected override void Update()
    {
        base.Update();
        TryEnrage();
        UpdateEnrage();
    }

    void TryEnrage()
    {
        if (enraged || !canEnrage) return;

        EnemyHealth h = GetComponent<EnemyHealth>();
        if (h == null || h.currentHealth > enrageHpThreshold) return;

        enraged = true;
        spawnTimer = spawnInterval;
        chaseSpeed *= enrageSpeedBoost;
        patrolSpeed *= enrageSpeedBoost;
        punchDamage *= enrageDamageBoost;
        if (ranged != null) ranged.damageOverride *= enrageDamageBoost;
        transform.localScale *= enrageSizeBoost;
        sr.color = Color.red;

        SpawnBackups();

        if (chatter != null) chatter.Say("Enough!");
    }

    void UpdateEnrage()
    {
        if (!enraged) return;
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnBackups();
            spawnTimer = spawnInterval;
        }
    }

    void SpawnBackups()
    {
        if (backupPrefab == null) return;
        if (leftSpawn != null)
        {
            GameObject l = Instantiate(backupPrefab);
            l.transform.position = leftSpawn.position;
        }
        if (rightSpawn != null)
        {
            GameObject r = Instantiate(backupPrefab);
            r.transform.position = rightSpawn.position;
        }
    }
}