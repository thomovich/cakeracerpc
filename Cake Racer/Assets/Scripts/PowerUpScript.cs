using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Karting.KartSystem;
using UnityEngine;
using Random = UnityEngine.Random;


public class PowerUpScript : MonoBehaviour
{

    
    public event EventHandler<onpowerupEventArgs> onpowerup;

    public class onpowerupEventArgs : EventArgs
    {
        public string boostid;
    }
    
    private ArcadeKart.StatPowerup boostStats = new ArcadeKart.StatPowerup { };
        

       
    
    
    

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Racer")
            {
                var rb = other.attachedRigidbody;
                if (rb)
                {
                    var kart = rb.GetComponent<ArcadeKart>();
                    if (!kart.recievedpowerup)
                    {
                        int random = Random.Range(1, 5);
                       
                        switch (random)
                        {
                            case 1:
                                boostStats.PowerUpID = "boost";
                                break;
                            case 2:
                                boostStats.PowerUpID = "rocket";
                                break;
                            case 3:
                                boostStats.PowerUpID = "tripleboost";
                                break;
                            case 4: 
                                boostStats.PowerUpID = "triplerocket";
                                break;
                            default:
                                break;
                                
                        }
                        
                        kart.AddPowerup(this.boostStats);
                        
                        onpowerup?.Invoke(this, new onpowerupEventArgs{boostid = boostStats.PowerUpID});
                        
                        

                    }
                }
                gameObject.SetActive(false);
                Invoke(nameof(CreateNewObject), 3.0f);
            }
        }

    
        void CreateNewObject()
        {
            gameObject.SetActive(true);
        }

      

       

    
    
    }


