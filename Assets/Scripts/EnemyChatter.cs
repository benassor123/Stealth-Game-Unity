using UnityEngine;
using TMPro;

// Shows a short text bubble above the enemy's head for a few seconds.
// Attach to any enemy. EnemyBase calls chatter.Say(...) on state changes.
//
// Setup in Unity:
//   1. Create a child GameObject of the enemy, name it "Chatter"
//   2. Add a TextMeshPro component to it (Component > UI > Text - TextMeshPro, OR
//      use 3D TMP: Component > Mesh > Text - TextMeshPro)
//   3. Assign this child's TMP_Text to the textComponent field below
//   4. Position the child slightly above the enemy (y offset 0.8 or so)
//   5. Add this EnemyChatter script to the parent enemy
public class EnemyChatter : MonoBehaviour
{
    public TMP_Text textComponent;    // drag the child's TextMeshPro here
    public float messageDuration = 2f;

    float hideTimer = 0f;

    void Start()
    {
        if (textComponent != null)
            textComponent.text = "";
    }

    void Update()
    {
        if (hideTimer > 0f)
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0f && textComponent != null)
                textComponent.text = "";
        }

        // counter-rotate text so it reads upright regardless of enemy's rotation
        if (textComponent != null)
            textComponent.transform.rotation = Quaternion.identity;
    }

    public void Say(string message)
    {
        if (textComponent == null) return;
        textComponent.text = message;
        hideTimer = messageDuration;
    }
}