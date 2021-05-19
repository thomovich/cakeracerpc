using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class CountdownController : MonoBehaviour
{
    public int countdowntime;
    public Text countdowndisplay;
    private GameController GameController;
    private new AudioManager audio;

    private void Start()
    {
        float start = Time.realtimeSinceStartup;
        GameController = FindObjectOfType<GameController>();
        audio = FindObjectOfType<AudioManager>();
        while (Time.realtimeSinceStartup < start)
        {
            
        }
        gameStart();
        
    }

    

    void gameStart()
    {
        audio.Play("Countdown");
        StartCoroutine(countdowntostart());
    }

    IEnumerator countdowntostart()
    {
        while (countdowntime > 0)
        {
            countdowndisplay.text = countdowntime.ToString();
            if (countdowntime == 3)
            {
                countdowndisplay.color = Color.red;
            } else if (countdowntime == 2)
            {
                countdowndisplay.color = Color.yellow;
            } else if (countdowntime == 1)
            {
                countdowndisplay.color = Color.green;
            }
            

            yield return new WaitForSecondsRealtime(1.1f);

            countdowntime--;
        }

        countdowndisplay.text = "GO!";
        
        GameController.StartGame();
        
        yield return new WaitForSecondsRealtime(1f);
        
        countdowndisplay.gameObject.SetActive(false);
    }
}
