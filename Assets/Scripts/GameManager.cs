using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private LevelEnd levelStatus;
    public float currentTime;
    public TextMeshProUGUI _time;
    //public bool levelStatus;
    public float timeComplete;

    void Start()
    {
        currentTime = 0f;

        levelStatus = FindObjectOfType<LevelEnd>();
        if (levelStatus == null)
        {
            Debug.LogError("LevelEnd script not found in scene!");
        }
    }

    void Update()
    {
        //roundTimer();
    }

    private void FixedUpdate()
    {
        roundTimer();
    }

    void roundTimer()
    {
        if (levelStatus.levelStatus == true)
        {
            currentTime += Time.deltaTime;

            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            float hundredths = (currentTime % 1f) * 100f;

            _time.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, Mathf.FloorToInt(hundredths));
        }

        else if (levelStatus.levelStatus == false)
        {
            Debug.Log(_time.text);
        }
    }
}
