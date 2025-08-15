using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    [Header("Volume Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    public static float sfxVolume = 1f;

    void Start()
    {
        //reset timer
        currentTime = 0f;

        //find level end script
        levelStatus = FindObjectOfType<LevelEnd>();
        if (levelStatus != null)
            levelStatus.OnLevelComplete += HandleLevelComplete;

        //hide level complete ui
        if (levelCompleteCanvas != null) levelCompleteCanvas.SetActive(false);
        //hide pause menu ui
        if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(false);

        //play background music
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }

        //set music slider
        if (musicSlider != null)
        {
            musicSlider.value = 0.1f;
            musicSource.volume = musicSlider.value;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        //set sfx slider
        if (sfxSlider != null)
        {
            sfxSlider.value = sfxVolume;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    void Update()
    {
        //check for escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    private void FixedUpdate()
    {
        //update timer
        roundTimer();
    }

    void roundTimer()
    {
        //run timer only if level active
        if (levelStatus != null && levelStatus.levelStatus)
        {
            //add time
            currentTime += Time.deltaTime;
            //convert time to minutes seconds hundredths
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            float hundredths = (currentTime % 1f) * 100f;
            //update time text
            _time.text = string.Format("{0:00}:{1:00}:{2:00}",
                minutes, seconds, Mathf.FloorToInt(hundredths));
        }
    }

    void HandleLevelComplete()
    {
        //stop time
        Time.timeScale = 0f;
        //show final time
        if (finalTimeText != null) finalTimeText.text = "Time: " + _time.text;
        //show level complete ui
        if (levelCompleteCanvas != null) levelCompleteCanvas.SetActive(true);
    }

    public void PauseGame()
    {
        //stop time
        Time.timeScale = 0f;
        isPaused = true;
        //show pause menu
        if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(true);
    }

    public void ResumeGame()
    {
        //resume time
        Time.timeScale = 1f;
        isPaused = false;
        //hide pause menu
        if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(false);
    }

    public void SetMusicVolume(float value)
    {
        //change music volume
        if (musicSource != null)
            musicSource.volume = value;
    }

    public void SetSFXVolume(float value)
    {
        //change sfx volume
        sfxVolume = value;
    }
}
