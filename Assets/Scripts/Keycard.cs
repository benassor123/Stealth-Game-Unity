using UnityEngine;

public class Keycard : MonoBehaviour
{
    public static int keycardCount = 0;

    void Start()
    {
        Debug.Log("Keycard now at -  " + transform.position);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            keycardCount++;
            Debug.Log("Keycard count total - " + keycardCount);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            keycardCount++;
            Debug.Log("keycard picked up, keycardcount total -  " + keycardCount);
            Destroy(gameObject);
        }
    }
}