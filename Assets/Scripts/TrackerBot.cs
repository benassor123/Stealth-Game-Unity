using UnityEngine;

public class TrackerBot : PatrolEnemy
{
    [Header("Prediction")]
    public float predictionTime = 0.5f;

    protected override void ChaseMove()
    {
        recalcTimer -= Time.fixedDeltaTime;

        if (recalcTimer <= 0f)
        {
            Vector2 predicted = PredictPlayerPosition();
            RecomputePath(predicted);
            recalcTimer = pathRecalc;
        }

        // if (ranged != null)
        // {
        //     float shootStopRange = Mathf.Min(ranged.shootRange, viewRange - 1f);
        //     float dist = Vector2.Distance(transform.position, player.position);

        //     if (dist <= shootStopRange && CanSeePlayer())
        //     {
        //         FaceDirection(player.position - transform.position);
        //         return;
        //     }
        // }

        FollowPath();
    }

    Vector2 PredictPlayerPosition()
    {
        Vector2 currentPos = player.position;
        Vector2 velocity = Vector2.zero;
        if (playerRb != null) velocity = playerRb.linearVelocity;

        return currentPos + velocity * predictionTime;
    }

    protected new bool CanHearPlayer() { return false; }
}