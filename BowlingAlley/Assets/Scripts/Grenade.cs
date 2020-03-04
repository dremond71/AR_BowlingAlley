using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{

    public float delay = 3f;
    public GameObject explosionEffect;
    float countdown;
    bool hasExploded = false;

    public float blastRadius = 5f;
    public float explosionForce = 700f;
    void Start()
    {
        countdown = delay;
    }


    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0 && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }


    void Explode()
    {

        Debug.Log("BOOM!");

        //show effect 
        GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);

        // get nearby objects

        // add force 
        // Damage 

        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);
        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, blastRadius);
            }
        }
        //Destroy(explosion);
        // remove grenade
        Destroy(gameObject);
    }
}
