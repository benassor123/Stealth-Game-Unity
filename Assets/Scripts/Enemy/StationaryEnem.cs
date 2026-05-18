using UnityEngine;

public class StationaryEnem : EnemyBase
{
    [Header("Task")]
    public Transform focusPoint;
    public Sprite workSprite1;
    public Sprite workSprite2;
    public float frameTime = 0.5f;

    float frameTimer;
    bool onFrame1 = true;

    protected override void OnStart()
    {
        if (focusPoint != null)
            FaceDirection(focusPoint.position - transform.position);
    }

    protected override void IdleUpdate()
    {
        if (focusPoint != null)
            FaceDirection(focusPoint.position - transform.position);
    }

    protected override void UpdateSprites()
    {
        base.UpdateSprites();

        if (state != "idle") return;
        if (workSprite1 == null || workSprite2 == null) return;

        frameTimer += Time.deltaTime;

        if (frameTimer >= frameTime)
        {
            frameTimer = 0f;
            onFrame1 = !onFrame1;

            if (onFrame1) sr.sprite = workSprite1;
            else sr.sprite = workSprite2;
        }
    }
}