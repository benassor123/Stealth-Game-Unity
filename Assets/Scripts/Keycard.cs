using UnityEngine;

public class Keycard : MonoBehaviour
{
    public static int keycardCount = 0;

    void Start()
    {
        Debug.Log("Keycard spawned at " + transform.position);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Keycard trigger hit by: " + other.name + " tag: " + other.tag);
        if (other.CompareTag("Player"))
        {
            keycardCount++;
            Debug.Log("KEYCARD PICKED UP! Total: " + keycardCount);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Keycard collision with: " + col.gameObject.name + " tag: " + col.gameObject.tag);
        if (col.gameObject.CompareTag("Player"))
        {
            keycardCount++;
            Debug.Log("KEYCARD PICKED UP! Total: " + keycardCount);
            Destroy(gameObject);
        }
    }
}