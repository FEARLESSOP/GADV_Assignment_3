using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadLevel1()
    {
        SceneManager.LoadScene(1); // Loads scene with build index 1
    }

    public void QuitGame()
    {
        Debug.Log("quit");
        Application.Quit();
    }
}

