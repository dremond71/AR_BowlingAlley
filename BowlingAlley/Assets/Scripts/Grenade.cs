using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{

    public float delay = 3f; 
    public GameObject explosionEffect; 
    float countdown;
    bool hasExploded = false;
    void Start()
    {
       countdown=delay; 
    }

    
    void Update()
    {
       countdown -= Time.deltaTime;
       if (countdown <= 0 && !hasExploded){
           Explode();
           hasExploded = true;
       } 
    }

    void Explode() {
        Debug.Log("Boom!");

        //show effect 
         Instantiate(explosionEffect,transform.position,transform.rotation);
        // get nearby objects
           // add force 
           // Damage 

        // remove grenade
        Destroy(gameObject);
    }
}
