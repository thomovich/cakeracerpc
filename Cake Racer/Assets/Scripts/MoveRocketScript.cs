using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRocketScript : MonoBehaviour
{
    
    private new AudioManager audio;
    public GameObject explosion;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5f);
        audio = FindObjectOfType<AudioManager>();

    }


    private void FixedUpdate()
    {

        transform.position += transform.forward * Time.deltaTime * 50f;
    }

    private void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
        audio.Play("Explosion");
        var clonebomb = Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
        Destroy(clonebomb, 1f);
    }
}
