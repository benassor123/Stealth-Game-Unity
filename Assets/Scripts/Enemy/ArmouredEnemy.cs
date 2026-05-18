using UnityEngine;

public class ArmouredEnemy : PatrolEnemy
{
    [Header("Armour")]
    public float backupRadius = 8f;

    public override void OnTakedown()
    {

        Vector2 playerPos = transform.position;
        if (player != null) playerPos = player.position;

        ForceChase(playerPos);
        ForceChaseNearby(transform.position, backupRadius, playerPos);

        if (chatter != null) chatter.Say("Nice try!");
    }
}