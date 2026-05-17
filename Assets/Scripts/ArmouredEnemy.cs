using UnityEngine;

public class ArmouredEnemy : EnemyAI
{
    [Header("Armour")]
    public float alertRadiusOnFailedTakedown = 8f;

    public override void OnTakedown()
    {
        Vector2 playerPos = player != null ? (Vector2)player.position : (Vector2)transform.position;
        ForceChase(playerPos);
        ForceChaseNearby(transform.position, alertRadiusOnFailedTakedown, playerPos);
        if (chatter != null) chatter.Say("Nice try!");
    }
}