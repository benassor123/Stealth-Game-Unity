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
    public float damageOnCross = 20f;

    LineRenderer line;
    BoxCollider2D box;

    Transform player;
    bool active = true;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;

        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = beamWidth;


        line.endWidth = beamWidth;
        line.startColor = beamColor;

        line.endColor = beamColor;

        box = GetComponent<BoxCollider2D>();
        box.isTrigger = false;
    }

    void Update()
    {
        if (!active) return;

        if (!BothEmittersAlive())
        {
            active = false;
            line.enabled = false;
            box.enabled = false;
            return;
        }

        UpdateBeam();
    }

    bool BothEmittersAlive()
    {
        if (emitterA == null || emitterB == null) return false;
        return emitterA.IsAlive && emitterB.IsAlive;
    }

    void UpdateBeam()
    {
        Vector3 start = emitterA.transform.position;
        Vector3 end = emitterB.transform.position;

        line.SetPosition(0, start);
        line.SetPosition(1, end);


        Vector3 dir = end - start;
        transform.position = (start + end) * 0.5f;
        transform.right = dir.normalized;
        box.size = new Vector2(dir.magnitude, beamWidth * 3f);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!active) return;
        if (!other.collider.CompareTag("Player")) return;

        EnemyBase.ForceChaseNearby(transform.position, alertRadius, player.position);

        HUD hud = FindFirstObjectByType<HUD>();
        if (hud != null) hud.TakeDamage(damageOnCross);

    }
}