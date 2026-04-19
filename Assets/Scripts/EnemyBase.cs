using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Detection")]
    public float viewRange = 4f;
    public float viewAngle = 40f;
    public float hearRange = 2f;
    public LayerMask wallLayer;

    [Header("Chase")]
    public float chaseSpeed = 2.5f;
    public float giveUpTime = 2f;
    public float pathRecalc = 0.5f;
    public float waypointReach = 0.35f;

    [Header("Search (level 3+)")]
    public bool canSearch = false;
    public float searchDuration = 4f;

    [Header("Coordination (level 3+)")]
    public bool sharesLocation = false;
    public bool respondsToLocation = false;

    [Header("Punch")]
    public Sprite punchSprite;
    public float punchRange = 0.8f;
    public float punchCooldown = 1f;
    public float punchDamage = 20f;

    [Header("Alert")]
    public float alertRadius = 6f;
    public float alertDuration = 2f;

    [Header("Chase Sprite")]
    public Sprite chaseSprite;

    protected Transform player;
    protected Rigidbody2D playerRb;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected RangedAttack ranged;
    protected EnemyChatter chatter;

    protected string state = "idle";
    protected float alertTimer;
    protected float lostTimer;
    protected float punchTimer;
    protected bool isPunching;
    float punchAnim;

    List<Vector2> path = new List<Vector2>();
    int pathIndex;
    float recalcTimer;
    Vector2 targetOffset;

    Vector2 lastSeenPos;
    bool knowsLastPos;
    float searchTimer;

    protected virtual void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        playerRb = player.GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        ranged = GetComponent<RangedAttack>();
        chatter = GetComponent<EnemyChatter>();
        targetOffset = Vector2.zero;
        OnStart();
    }

    protected virtual void Update()
    {
        if (player == null) return;

        if (punchTimer > 0f) punchTimer -= Time.deltaTime;

        if (isPunching)
        {
            punchAnim -= Time.deltaTime;
            if (punchAnim <= 0f) isPunching = false;
        }

        if (state == "idle")
        {
            IdleUpdate();

            if (Vector2.Distance(transform.position, player.position) <= punchRange) { StartChase(); return; }
            if (CanSeePlayer()) { StartChase(); return; }

            if (CanHearPlayer())
            {
                if (respondsToLocation) GoAlert(player.position, 1.5f);
                else { state = "alert"; alertTimer = 1.5f; knowsLastPos = false; }
            }
        }
        else if (state == "alert")
        {
            alertTimer -= Time.deltaTime;

            if (Vector2.Distance(transform.position, player.position) <= punchRange) { StartChase(); return; }
            if (CanSeePlayer()) { StartChase(); return; }

            if (alertTimer <= 0f) { state = "idle"; knowsLastPos = false; }
        }
        else if (state == "chase")
        {
            ChaseUpdate();
        }
        else if (state == "search")
        {
            SearchUpdate();
        }

        UpdateSprites();
    }

    protected virtual void FixedUpdate()
    {
        if (player == null) return;
        if (state == "idle") IdleFixedUpdate();
        else if (state == "alert") AlertMove();
        else if (state == "chase") ChaseMove();
        else if (state == "search") SearchMove();
    }

    protected virtual void OnStart() { }
    protected virtual void IdleUpdate() { }
    protected virtual void IdleFixedUpdate() { }

    protected virtual void UpdateSprites()
    {
        if (isPunching) return;
        if (ranged != null && ranged.IsShooting) return;

        if ((state == "chase" || state == "alert" || state == "search") && chaseSprite != null)
            sr.sprite = chaseSprite;
    }

    // --- chase ---

    protected virtual void ChaseUpdate()
    {
        float dist = Vector2.Distance(transform.position, player.position);

        if (CanSeePlayer())
        {
            lastSeenPos = player.position;
            knowsLastPos = true;
            lostTimer = 0f;
        }

        if (dist <= punchRange)
        {
            if (punchTimer <= 0f) Punch();
            return;
        }

        if (!CanSeePlayer())
        {
            lostTimer += Time.deltaTime;
            if (lostTimer > giveUpTime)
            {
                if (canSearch) GoSearch();
                else { state = "idle"; lostTimer = 0f; path.Clear(); }
                return;
            }
        }

        if (ranged != null && dist <= ranged.shootRange && ranged.CanShoot && !isPunching && CanSeePlayer())
        {
            FaceDirection(player.position - transform.position);
            ranged.ShootAt(player.position);
        }
    }

    protected virtual void ChaseMove()
    {
        recalcTimer -= Time.fixedDeltaTime;
        if (recalcTimer <= 0f)
        {
            RecomputePath((Vector2)player.position + targetOffset);
            recalcTimer = pathRecalc;
        }

        // if we have a gun and can see the player in range, stop and shoot
        if (ranged != null)
        {
            float effRange = Mathf.Min(ranged.shootRange, viewRange - 1f);
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist <= effRange && CanSeePlayer())
            {
                FaceDirection(player.position - transform.position);
                return;
            }
        }

        FollowPath();
    }

    // --- alert (path to reported position) ---

    protected virtual void AlertMove()
    {
        if (!knowsLastPos) return;

        if (Vector2.Distance(transform.position, lastSeenPos) < 0.6f) return;

        recalcTimer -= Time.fixedDeltaTime;
        if (recalcTimer <= 0f)
        {
            RecomputePath(lastSeenPos);
            recalcTimer = pathRecalc;
        }

        FollowPath();
    }

    // --- search ---

    void GoSearch()
    {
        state = "search";
        searchTimer = searchDuration;
        lostTimer = 0f;
        recalcTimer = 0f;
        if (chatter != null) chatter.Say("Lost him!");
    }

    protected virtual void SearchUpdate()
    {
        if (CanSeePlayer()) { StartChase(); return; }

        searchTimer -= Time.deltaTime;
        if (searchTimer <= 0f)
        {
            state = "idle";
            knowsLastPos = false;
            path.Clear();
            if (chatter != null) chatter.Say("Resuming patrol.");
        }
    }

    protected virtual void SearchMove()
    {
        if (!knowsLastPos) return;

        float dist = Vector2.Distance(transform.position, lastSeenPos);

        if (dist > 0.6f)
        {
            recalcTimer -= Time.fixedDeltaTime;
            if (recalcTimer <= 0f)
            {
                RecomputePath(lastSeenPos);
                recalcTimer = pathRecalc;
            }
            FollowPath();
        }
    }

    // --- pathfinding helpers ---

    void RecomputePath(Vector2 targetPos)
    {
        List<Vector2> newPath = Pathfinder.FindPath(transform.position, targetPos);
        if (newPath == null || newPath.Count == 0) return;

        int startIdx = 0;
        if (newPath.Count > 1 && Vector2.Distance(transform.position, newPath[0]) < waypointReach)
            startIdx = 1;

        path = newPath;
        pathIndex = startIdx;
    }

    void FollowPath()
    {
        if (path == null || path.Count == 0 || pathIndex >= path.Count) return;

        // skip through any waypoints we're already close enough to
        while (pathIndex < path.Count &&
               Vector2.Distance(transform.position, path[pathIndex]) < waypointReach)
        {
            pathIndex++;
        }

        if (pathIndex >= path.Count) { path.Clear(); return; }

        Vector2 target = path[pathIndex];
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, chaseSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
        FaceDirection(target - (Vector2)transform.position);
    }

    // --- state entry ---

    protected virtual void OnEnterChase()
    {
        Vector2 reportPos = sharesLocation ? (Vector2)player.position : (Vector2)transform.position;
        AlertNearby(transform.position, alertRadius, alertDuration, reportPos, sharesLocation);
        recalcTimer = 0f;
        if (chatter != null) chatter.Say("Contact!");
    }

    protected void StartChase()
    {
        if (state == "chase") return;
        state = "chase";
        lastSeenPos = player.position;
        knowsLastPos = true;
        OnEnterChase();
    }

    void GoAlert(Vector2 reportPos, float duration)
    {
        if (state == "chase") return;
        state = "alert";
        alertTimer = duration;
        lastSeenPos = reportPos;
        knowsLastPos = true;
        recalcTimer = 0f;
    }

    public virtual void ReceiveAlert(float duration, Vector2 reportPos, bool reliable)
    {
        if (state == "chase") return;

        // always come help - this is baseline behaviour since level 1
        // coordinated enemies (level 3+) path precisely to the reported player position
        // uncoordinated enemies walk toward the source of the shout instead
        if (respondsToLocation && reliable)
        {
            GoAlert(reportPos, duration);
            if (chatter != null) chatter.Say("Moving to intercept!");
        }
        else
        {
            // walk toward the shout source so they at least come investigate
            GoAlert(reportPos, duration);
        }
    }

    public virtual void ForceChase(Vector2 reportPos)
    {
        if (state == "chase") return;
        state = "chase";
        lostTimer = 0f;
        recalcTimer = 0f;
        lastSeenPos = reportPos;
        knowsLastPos = true;
        if (chatter != null) chatter.Say("Engaging!");
    }

    // --- broadcasts ---

    public static void AlertNearby(Vector2 source, float radius, float duration, Vector2 reportPos, bool reliable)
    {
        EnemyBase[] all = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
        List<EnemyBase> group = new List<EnemyBase>();

        foreach (EnemyBase e in all)
        {
            if (Vector2.Distance(source, e.transform.position) > radius) continue;
            if (e.state == "chase") continue;

            if (e.respondsToLocation && reliable)
            {
                // coordinated responder - will get a cornering role
                group.Add(e);
            }
            else
            {
                // uncoordinated but still comes to help - walks toward the shout source
                e.ReceiveAlert(duration, source, false);
            }
        }

        if (group.Count > 0)
        {
            group.Sort((a, b) =>
                Vector2.Distance(a.transform.position, reportPos)
                    .CompareTo(Vector2.Distance(b.transform.position, reportPos))
            );
            AssignCorneringRoles(group, reportPos);

            foreach (EnemyBase e in group)
                e.ReceiveAlert(duration, reportPos, true);
        }
    }

    public static void ForceChaseNearby(Vector2 source, float radius, Vector2 reportPos)
    {
        EnemyBase[] all = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
        List<EnemyBase> group = new List<EnemyBase>();

        foreach (EnemyBase e in all)
        {
            if (Vector2.Distance(source, e.transform.position) <= radius && e.state != "chase")
                group.Add(e);
        }

        group.Sort((a, b) =>
            Vector2.Distance(a.transform.position, reportPos)
                .CompareTo(Vector2.Distance(b.transform.position, reportPos))
        );

        AssignCorneringRoles(group, reportPos);

        foreach (EnemyBase e in group)
            e.ForceChase(reportPos);
    }

    // spreads the group around the target so they approach from different angles
    static void AssignCorneringRoles(List<EnemyBase> group, Vector2 targetPos)
    {
        if (group.Count == 0) return;

        // closest enemy gets the direct approach
        EnemyBase pursuer = group[0];
        Vector2 pursuerDir = ((Vector2)pursuer.transform.position - targetPos).normalized;
        pursuer.targetOffset = pursuerDir * 0.3f;

        // everyone else gets a rotated angle so they flank from different sides
        for (int i = 1; i < group.Count; i++)
        {
            float angle = (i * (360f / group.Count)) * Mathf.Deg2Rad;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            Vector2 flankDir = new Vector2(
                pursuerDir.x * cos - pursuerDir.y * sin,
                pursuerDir.x * sin + pursuerDir.y * cos
            );

            group[i].targetOffset = flankDir * 1.2f;
        }
    }

    // --- punch, senses, helpers ---

    protected virtual void Punch()
    {
        punchTimer = punchCooldown;
        isPunching = true;
        punchAnim = 0.3f;

        if (punchSprite != null) sr.sprite = punchSprite;

        HUD hud = FindFirstObjectByType<HUD>();
        if (hud != null) hud.TakeDamage(punchDamage);
    }

    protected bool CanSeePlayer()
    {
        Vector2 dir = player.position - transform.position;
        float dist = dir.magnitude;
        if (dist > viewRange) return false;

        float angle = Vector2.Angle(transform.right, dir);
        if (angle > viewAngle) return false;

        if (wallLayer.value != 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir.normalized, dist, wallLayer);
            if (hit.collider != null) return false;
        }

        return true;
    }

    protected bool CanHearPlayer()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > hearRange) return false;
        return playerRb.linearVelocity.magnitude > 0.1f;
    }

    protected void FaceDirection(Vector3 dir)
    {
        if (dir == Vector3.zero) return;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}