using UnityEngine;

public class Laser : MonoBehaviour
{
    [Header("Emitters")]
    public LaserEmitter emitterA;
    public LaserEmitter emitterB;

    [Header("Beam")]
    public float beamWidth = 0.15f;
    public Color beamColor = Color.red;

    [Header("Alert")]
    public float alertRadius = 15f;
    public AudioClip alarmSound;
    public float damageOnCross = 0f;

    LineRenderer line;
    BoxCollider2D box;
    Transform player;
    bool active = true;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;

        line = GetComponent<LineRenderer>();
        if (line == null) line = gameObject.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = beamWidth;
        line.endWidth = beamWidth;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = beamColor;
        line.endColor = beamColor;

        box = GetComponent<BoxCollider2D>();
        if (box == null) box = gameObject.AddComponent<BoxCollider2D>();
        box.isTrigger = true;
    }

    void Update()
    {
        // laser dies if either emitter is destroyed
        if (active && (emitterA == null || emitterB == null || !emitterA.IsAlive || !emitterB.IsAlive))
        {
            active = false;
            line.enabled = false;
            box.enabled = false;
            return;
        }

        if (!active) return;

        UpdateBeam();
    }

    void UpdateBeam()
    {
        Vector3 start = emitterA.transform.position;
        Vector3 end = emitterB.transform.position;

        line.SetPosition(0, start);
        line.SetPosition(1, end);

        // shape the collider to sit along the beam
        Vector3 mid = (start + end) * 0.5f;
        float length = Vector3.Distance(start, end);
        float angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;

        transform.position = mid;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        box.size = new Vector2(length, beamWidth * 3f);
        box.offset = Vector2.zero;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!active) return;
        if (!other.CompareTag("Player")) return;

        Debug.Log(name + " tripped - alerting enemies");
        EnemyBase.ForceChaseNearby(transform.position, alertRadius, player.position);

        if (alarmSound != null)
            AudioSource.PlayClipAtPoint(alarmSound, transform.position);

        if (damageOnCross > 0f)
        {
            HUD hud = FindFirstObjectByType<HUD>();
            if (hud != null) hud.TakeDamage(damageOnCross);
        }
    }
}