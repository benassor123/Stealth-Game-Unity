using UnityEngine;

public class CameraMov : MonoBehaviour
{
    Transform player;
    Camera cam;

    public float minX = -15f;
    public float maxX = 15f;
    public float minY = -9;
    public float maxY = 10f;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (player == null) return;

        float h = cam.orthographicSize;
        float w = h * cam.aspect;

        float x = Mathf.Clamp(player.position.x, minX + w, maxX - w);
        float y = Mathf.Clamp(player.position.y, minY + h, maxY - h);

        transform.position = new Vector3(x, y, -10f);
    }
}