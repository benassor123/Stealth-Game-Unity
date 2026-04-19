using UnityEngine;

public class ArmouredEnemy : EnemyAI
{
    [Header("Armour")]
    public float alertRadiusOnFailedTakedown = 8f;

    public void OnTakedownBlocked()
    {
        Debug.Log(name + " - armour blocked the takedown! Raising the alarm.");

        // player is right behind us when a takedown is attempted - their position is known
        Vector2 playerPos = player != null ? (Vector2)player.position : (Vector2)transform.position;

        ForceChase(playerPos);
        ForceChaseNearby(transform.position, alertRadiusOnFailedTakedown, playerPos);

        if (chatter != null) chatter.Say("Nice try!");
    }
}