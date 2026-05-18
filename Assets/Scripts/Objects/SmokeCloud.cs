using System.Collections.Generic;
using UnityEngine;

public class SmokeCloud : MonoBehaviour
{
    public float size = 3f;
    public float duration = 5f;

    public static List<SmokeCloud> active = new List<SmokeCloud>();

    float timer;

    void Start()
    {
        active.Add(this);
        timer = duration;
    }
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        active.Remove(this);
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        active.Remove(this);
    }

    public static bool InSmoke(Vector2 pos)
    {
        for (int i = 0; i < active.Count; i++)
        {
            if (active[i] == null) continue;
            float dist = Vector2.Distance(pos, active[i].transform.position);
            if (dist <= active[i].size) return true;
        }
        return false;
    }
}