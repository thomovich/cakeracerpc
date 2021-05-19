using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool checkpointrached;
    // Start is called before the first frame update
    void Start()
    {
        checkpointrached = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Racer"))
        {
            checkpointrached = true;
        }
    }

    public bool isCheckpointreached()
    {
        return checkpointrached;
    }
}
