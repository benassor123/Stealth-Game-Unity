using UnityEngine;
using TMPro;


public class EnemyChatter : MonoBehaviour
{
    public TMP_Text textComponent;
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