using UnityEngine;

// a robotic enemy that predicts player movement and tries to intercept.
// inherits from EnemyAI for patrol, vision, shooting via RangedAttack.
// different from normal shooters because its chase target is the PREDICTED
// position of the player (current position + their velocity * lookahead),
// not the current position. this makes it feel like it's cutting you off
// rather than reacting to you.
public class TrackerBot : EnemyAI
{
    [Header("Prediction")]
    public float predictionTime = 0.5f;   // seconds to look ahead. 0 = behave like normal enemy. 1+ = very aggressive intercept.

    // override: path to where we THINK the player will be
    protected override void ChaseMove()
    {
        recalcTimer -= Time.fixedDeltaTime;
        if (recalcTimer <= 0f)
        {
            Vector2 predicted = PredictPlayerPosition();
            RecomputePath(predicted);
            recalcTimer = pathRecalc;
        }

        // still uses the range + line-of-sight shooting check from the base
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

    // where will the player be in `predictionTime` seconds, based on current velocity?
    Vector2 PredictPlayerPosition()
    {
        Vector2 currentPos = player.position;
        Vector2 velocity = playerRb != null ? playerRb.linearVelocity : Vector2.zero;
        return currentPos + velocity * predictionTime;
    }

    // robots don't hear footsteps - they're deaf sensors
    protected new bool CanHearPlayer() { return false; }
}