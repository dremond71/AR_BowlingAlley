using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TieMissleTargetBehaviour : MonoBehaviour {

    public GameObject explosionEffect;
    bool hasExploded = false;

    public float blastRadius = 5f;
    public float explosionForce = 700f;
    Vector3 contactPoint;

    bool receivedContact = false;

    // Start is called before the first frame update
    void Start () {

    }

    // Update is called once per frame
    void Update () {

        if (receivedContact && !hasExploded) {
            Explode ();
            hasExploded = true;
        }

    }

    private void OnCollisionEnter (Collision collision) {

        if (collision.gameObject.tag == "miniTieMissle") {

            // use collision.gameObject.tag === "bullet" to figure 
            // out which object collided with you. 
            ContactPoint cp = collision.contacts[0];
            Debug.Log ("Tie Missle touched me");
            contactPoint = cp.point;
            receivedContact = true;
        }

        //Debug.Log("contact point : " + cp.point);

    }

    void Explode () {

        Debug.Log ("BOOM!");

        //show effect 
        GameObject explosion = Instantiate (explosionEffect, contactPoint, transform.rotation);

    }

}