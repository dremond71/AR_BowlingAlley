using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TantiveIVShipBehaviour : MonoBehaviour {

    public GameObject energyExplosionEffect;
    private Animator anim;
    bool performingLoop = false;
    Vector3 contactPoint;
    bool receivedContact = false;

    private AudioSource exitHyperSpaceSource1;
    private AudioClip exitHyperspace;

    private AudioSource energyExplosionSource;
    private AudioClip energyExplosion;

    private float energyExplosionVolume = 0.75f;

    private void Awake () {
        anim = GetComponent<Animator> ();
        anim.speed = 0.03f; // play loop animation really slow
        exitHyperSpaceSource1 = GameObject.FindGameObjectWithTag ("exitHyperspace_Sound").GetComponent<AudioSource> ();
        exitHyperspace = exitHyperSpaceSource1.clip;

        energyExplosionSource = GameObject.FindGameObjectWithTag ("energyExplosion_Sound").GetComponent<AudioSource> ();
        energyExplosion = energyExplosionSource.clip;

    }
    // Start is called before the first frame update
    void Start () {
        PlayExitHyperspaceSound_Immediately ();
        anim.SetBool ("shouldPerformLoop", true);
    }

    // Update is called once per frame
    void Update () {

    }

    void PlayExitHyperspaceSound_Immediately () {
        // we want a seperate object for each fire; so we can handle multi blasts in quick succession

        // Method I used to achieve multiple blasts that don't interrupt each other:
        // https://docs.unity3d.com/ScriptReference/AudioSource.PlayOneShot.html
        exitHyperSpaceSource1.PlayOneShot (exitHyperspace, 2.0f);
    }

    private void OnCollisionEnter (Collision collision) {

        if ( (collision.gameObject.tag == "miniTieMissle") || (collision.gameObject.tag == "deathStarMissle")) {

            // use collision.gameObject.tag === "bullet" to figure 
            // out which object collided with you. 
            ContactPoint cp = collision.contacts[0];
            Debug.Log ("Tie Missle touched me");
            contactPoint = cp.point;
            receivedContact = true;
        }

        //Debug.Log("contact point : " + cp.point);

    }

    void FixedUpdate () {

        if (receivedContact) {
            showExplosionAtContactPoint ();
            receivedContact = false;
        }

    }

    void showExplosionAtContactPoint () {

        Debug.Log ("BOOM!");

        //show effect 
        GameObject explosion = Instantiate (energyExplosionEffect, contactPoint, transform.rotation);

        explosion.transform.parent = this.transform; //make explosion the child of ship - to make it stick and move with ship in same spot

        //then scale it down
        float value = 4.5f; // sweet spot. see cloud and lightning best
        float x = value;
        float y = value;
        float z = value;

        explosion.transform.localScale -= new Vector3 (x, y, z);

        PlayEnergyExplosionSound_Immediately ();
    }

    void PlayEnergyExplosionSound_Immediately () {
        energyExplosionSource.PlayOneShot (energyExplosion, energyExplosionVolume);

    }

}