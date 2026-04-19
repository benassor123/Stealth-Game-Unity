using UnityEngine;

public class StationaryEnem : EnemyBase
{
    [Header("Task")]
    public Transform taskFocusPoint;
    public Sprite taskSpriteA;
    public Sprite taskSpriteB;
    public float taskSwapInterval = 0.5f;

    float swapTimer = 0f;
    bool showingA = true;

    protected override void OnStart()
    {
        if (taskFocusPoint != null)
            FaceDirection(taskFocusPoint.position - transform.position);
    }

    protected override void IdleUpdate()
    {
        if (taskFocusPoint != null)
            FaceDirection(taskFocusPoint.position - transform.position);
    }

    // no ChaseFixedUpdate override - inherits base behaviour:
    // - melee: close to punch range
    // - with gun: maintain shoot range (close/back off as needed)

    protected override void UpdateSprites()
    {
        if (isPunching) return;
        if (ranged != null && ranged.IsShooting) return;

        if (state == "chase" || state == "alert")
        {
            if (chaseSprite != null) sr.sprite = chaseSprite;
            return;
        }

        // idle - swap task sprites
        if (taskSpriteA == null || taskSpriteB == null) return;

        swapTimer += Time.deltaTime;
        if (swapTimer >= taskSwapInterval)
        {
            swapTimer = 0f;
            showingA = !showingA;
            sr.sprite = showingA ? taskSpriteA : taskSpriteB;
        }
    }
}