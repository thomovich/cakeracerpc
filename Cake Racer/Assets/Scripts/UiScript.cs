using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Karting.KartSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiScript : MonoBehaviour
{
    public Text powerups;
    public GameObject boostPrefab;
    public GameObject rocketPrefab;
    public Canvas Canvas;
    private List<GameObject> objects = new List<GameObject>();
    private int amountofpowerups;




    // Start is called before the first frame update
    void Start()
    {
        
        foreach(GameObject powerup in GameObject.FindGameObjectsWithTag("Powerup"))
        {
            PowerUpScript powerupscript = powerup.GetComponent<PowerUpScript>();
            powerupscript.onpowerup += TestingPowerups;
        }
        
        
        
        GameObject player = GameObject.Find("Thirdpersonracer");
        ArcadeKart arcadeskript = player.GetComponent<ArcadeKart>();
        
        arcadeskript.onpowerupused += removeImage;



    }

    private void TestingPowerups(object sender, PowerUpScript.onpowerupEventArgs e)
    {
        
        setImage(e.boostid);
        
        
    }

    
    
    

    void setImage(String id)
    {
        if (id.Contains("boost"))
        {
            if (id.Equals("tripleboost"))
            {
                for (int i = 0; i < 3; i++)
                {
                    createUiObject(boostPrefab);
                }
            }
            else
            {
                createUiObject(boostPrefab);
            }
            
        }
        
        
        else if(id.Contains("rocket"))
        {
            if (id.Equals("triplerocket"))
            {
                for (int i = 0; i < 3; i++)
                {
                    createUiObject(rocketPrefab);
                }
            }
            else
            {
                createUiObject(rocketPrefab);
            }
            
        }
        
        
    }

    void removeImage(object sender, EventArgs e)
    {
        Destroy(objects[objects.Count - 1]);
        objects.RemoveAt(objects.Count - 1);
    }

    void createUiObject(GameObject prefab)
    {
        amountofpowerups = objects.Count;
        GameObject newItem = Instantiate(prefab, new Vector3(100f * (1*amountofpowerups) + 100f,1000f ,0f), Quaternion.identity);
        newItem.transform.SetParent(Canvas.transform);
        objects.Add(newItem);
       
    }
   
}
