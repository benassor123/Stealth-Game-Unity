using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    public float timeLeft = 120f;
    public TMP_Text display;
    public GameObject failScreen;

    bool failed;

    void Update()
    {
        if (failed) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft < 0f) timeLeft = 0f;

        int mins = (int)(timeLeft / 60f);
        int secs = (int)(timeLeft % 60f);
        display.text = mins + ":" + secs.ToString("D2");

        if (timeLeft <= 0f) Fail();
    }

    void Fail()
    {
        failed = true;
        if (failScreen != null) failScreen.SetActive(true);
        Time.timeScale = 0f;
    }
}