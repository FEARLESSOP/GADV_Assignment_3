using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private LevelEnd levelStatus;
    public float currentTime;
    public TextMeshProUGUI _time;

    [Header("UI Elements")]
    public GameObject levelCompleteCanvas;
    public TextMeshProUGUI finalTimeText;

    [Header("Pause Menu")]
    public GameObject pauseMenuCanvas;
    private bool isPaused = false;

    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip backgroundMusic;


    void Start()
    {
        currentTime = 0f;

        levelStatus = FindObjectOfType<LevelEnd>();
        if (levelStatus == null)
        {
            Debug.LogError("LevelEnd script not found in scene!");
            return;
        }

        levelStatus.OnLevelComplete += HandleLevelComplete;

        if (levelCompleteCanvas != null)
            levelCompleteCanvas.SetActive(false);

        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(false);

        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void FixedUpdate()
    {
        roundTimer();
    }

    void roundTimer()
    {
        if (levelStatus.levelStatus)
        {
            currentTime += Time.deltaTime;

            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            float hundredths = (currentTime % 1f) * 100f;

            _time.text = string.Format("{0:00}:{1:00}:{2:00}",
                minutes, seconds, Mathf.FloorToInt(hundredths));
        }
    }

    void HandleLevelComplete()
    {
        Time.timeScale = 0f;

        if (finalTimeText != null)
            finalTimeText.text = "Time: " + _time.text;

        if (levelCompleteCanvas != null)
            levelCompleteCanvas.SetActive(true);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(false);
    }
}
