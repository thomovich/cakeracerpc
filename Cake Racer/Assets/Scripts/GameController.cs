using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    public void StartGame()
    {
        Time.timeScale = 1;
    }

    private void Start()
    {
        PauseGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void nextLevel()
    
    {
        if (SceneManager.GetActiveScene().buildIndex != 2)
            
        {
            // only load levels we have
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        
    }
}
