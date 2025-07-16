using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public float currentTime;
    public TextMeshProUGUI _time;

    void Start()
    {
        currentTime = 0f;
    }

    void Update()
    {
        roundTimer();
    }

    void roundTimer()
    {
        currentTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        float hundredths = (currentTime % 1f) * 100f;

        _time.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, Mathf.FloorToInt(hundredths));
    }


}
