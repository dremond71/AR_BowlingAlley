using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TantiveIVShipBehaviour : MonoBehaviour {

    public GameObject energyExplosionEffect;
    public GameObject explosionEffect;
    public float health = 500.0f;

    private Animator anim;
    bool performingLoop = false;
    private bool debug = false;

    private bool receivedContact = false;

    Vector3 ionMissleContactPoint;

    private AudioSource exitHyperSpaceSource1;
    private AudioClip exitHyperspace;

    private AudioSource energyExplosionSource;
    private AudioClip energyExplosion;

    private float energyExplosionVolume = 0.5f;

    private AudioSource explosionSource;
    private AudioClip explosion;
    private TextMesh debugText;


    private void Awake () {
        anim = GetComponent<Animator> ();
        anim.speed = 0.03f; // play loop animation really slow
        exitHyperSpaceSource1 = GameObject.FindGameObjectWithTag ("exitHyperspace_Sound").GetComponent<AudioSource> ();
        exitHyperspace = exitHyperSpaceSource1.clip;

        energyExplosionSource = GameObject.FindGameObjectWithTag ("energyExplosion_Sound").GetComponent<AudioSource> ();
        energyExplosion = energyExplosionSource.clip;

        explosionSource = GameObject.FindGameObjectWithTag("swExplosion_Sound").GetComponent<AudioSource>();
        explosion = explosionSource.clip;

        debugText = GameObject.Find("debugText").GetComponent<TextMesh>();

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

        if ( collision.gameObject.tag == "miniTieMissle") {
            health -= 20;
            ContactPoint cp = collision.contacts[0];
            MyDebug("Tie Missle touched me");
            ionMissleContactPoint = cp.point;
            receivedContact = true;
        }
        else if ( collision.gameObject.tag == "starDestroyerBlast") {
            health -= 10;
            ContactPoint cp = collision.contacts[0];
            ionMissleContactPoint = cp.point;
            receivedContact = true;
        }    
        else if (collision.gameObject.tag == "deathStarMissle")
        {
            health -= 100;
            ContactPoint cp = collision.contacts[0];
            MyDebug("Death Missle touched me");
            ionMissleContactPoint = cp.point;
            receivedContact = true;
        }

       
        //Debug.Log("contact point : " + cp.point);

    }

    void FixedUpdate () {

       if (health <= 0f)
        {
            Explode();
        }
       else
        {
            if (receivedContact)
            {
                showExplosionAtContactPoint();
                receivedContact = false;
            }
        }

    }

    void Explode()
    {
        //show effect 

        try
        {
            GameObject explosion = Instantiate(explosionEffect, ionMissleContactPoint, transform.rotation);

            //then scale it up
            float value = 2.0f; 
            float x = value;
            float y = value;
            float z = value;

            explosion.transform.localScale += new Vector3(x, y, z);

            PlayExplosionSound_Immediately();

            destroySelf();

        }
        catch (System.Exception e)
        {

            MyDebug("Explode error: " + e.ToString());
        }

    }

    void MyDebug(string someText)
    {

        if (debugText != null & debug)
        {
            debugText.text = someText;
        }

    }

    void PlayExplosionSound_Immediately()
    {
        explosionSource.PlayOneShot(explosion, 0.1f * 3.0f);

    }

    void destroySelf()
    {
        Destroy(gameObject);
    }


    void showExplosionAtContactPoint () {

        Debug.Log ("BOOM!");

        //show effect 
        GameObject explosion = Instantiate (energyExplosionEffect, ionMissleContactPoint, transform.rotation);

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