using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSparksBehaviour : MonoBehaviour
{

    //public float delay = 3f;
    public GameObject explosionEffect;
    //float countdown;
    bool hasExploded = false;

    public float blastRadius = 5f;
    public float explosionForce = 700f;
    Vector3 contactPoint;

    bool receivedContact = false;
    // Start is called before the first frame update
    void Start()
    {
        //countdown = delay;
    }

    // Update is called once per frame
    void Update()
    {

        if (receivedContact && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {


        if (collision.gameObject.tag == "grenade")
        {

            // use collision.gameObject.tag === "bullet" to figure 
            // out which object collided with you. 
            ContactPoint cp = collision.contacts[0];
            Debug.Log("Grenade touched me");
            contactPoint = cp.point;
            receivedContact = true;
        }


        //Debug.Log("contact point : " + cp.point);

    }


    void Explode()
    {

        Debug.Log("BOOM!");

        //show effect 
        GameObject explosion = Instantiate(explosionEffect, contactPoint, transform.rotation);

        // get nearby objects

        // add force 
        // Damage 

        // Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);
        // foreach (Collider nearbyObject in colliders)
        // {
        //     Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
        //     if (rb != null)
        //     {
        //         rb.AddExplosionForce(explosionForce, transform.position, blastRadius);
        //     }
        // }
        //Destroy(explosion);
        // remove grenade
        // Destroy(gameObject);
    }
}
