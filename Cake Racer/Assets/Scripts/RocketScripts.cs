using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketScripts : MonoBehaviour
{
    private GameObject rocket;
    private Vector3 playerPos;
    private Vector3 playerDirection;
    private Quaternion playerRotation;


    // Update is called once per frame

 

    public void shootMissile(Vector3 playerPos, Vector3 playerDirection, Quaternion playerRotation, GameObject rocket)
    {
       
        
        this.playerPos = playerPos;
        playerPos.y += 6f;
        this.playerDirection = playerDirection;
        this.playerRotation = playerRotation;
        this.rocket = rocket;
        float spawndistance = 10f;

        Vector3 spawnpos = playerPos + playerDirection * spawndistance;
        Instantiate(rocket,spawnpos, playerRotation);
    }
}
