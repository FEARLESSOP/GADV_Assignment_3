using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public void LoadLevelTutorial()
    {
        //set time back to normal
        Time.timeScale = 1f;
        //load tutorial level
        SceneManager.LoadScene(1);
    }

    public void LoadLevel1()
    {
        //set time back to normal
        Time.timeScale = 1f;
        //load level 1
        SceneManager.LoadScene(2);
    }

    public void LoadLevel2()
    {
        //set time back to normal
        Time.timeScale = 1f;
        //load level 2
        SceneManager.LoadScene(3);
    }

    public void LoadLevel3()
    {
        //set time back to normal
        Time.timeScale = 1f;
        //load level 3
        SceneManager.LoadScene(4);
    }

    public void LoadLevel4()
    {
        //set time back to normal
        Time.timeScale = 1f;
        //load level 4
        SceneManager.LoadScene(5);
    }

    public void LoadLevel5()
    {
        //set time back to normal
        Time.timeScale = 1f;
        //load level 5
        SceneManager.LoadScene(6);
    }

    public void LoadNextLevel()
    {
        //set time back to normal
        Time.timeScale = 1f;
        //load next level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void MainMenu()
    {
        //set time back to normal
        Time.timeScale = 1f;
        //load main menu
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        //show quit message
        Debug.Log("quit");
        //close game
        Application.Quit();
    }
}
