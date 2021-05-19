using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finishline : MonoBehaviour
{
    private GameController _gameController;

    private void Start()
    {
        _gameController = FindObjectOfType<GameController>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Racer")
        {
            Checkforfinish();
        }
        
    }

    void Checkforfinish()
    {
        foreach(GameObject varcheckpoint in GameObject.FindGameObjectsWithTag("Checkpoint"))
        {
            Checkpoint checkpoint = varcheckpoint.GetComponent<Checkpoint>();
            if (checkpoint.isCheckpointreached())
            {
                
            }
            else
            {
               
                return;
            }
            _gameController.PauseGame();
            new WaitForSecondsRealtime(5);
            _gameController.nextLevel();
        }
    }

    

   
}
