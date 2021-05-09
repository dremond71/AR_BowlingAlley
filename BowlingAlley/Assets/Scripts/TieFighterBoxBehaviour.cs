using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TieFighterBoxBehaviour : MonoBehaviour
{

    /*

     Ok... to detect collisions, the object needs a collider. 
           to move an object with AddForce, the object needs a rigidbody  
           If two objects collide, physics will be applied and will move objects IF 
           they have a rigidbody which applies physics. 
           I didn't want objects to move...only detect collisions and apply an 
           effects object (sparks or dust). 
           So I gave the bullets prefab a weight and drag of 0 pounds, so the 
           physics engine doesn't move stuff. 
           I also gave the tiefighter a collider and rigid body, but turned 
           isKinematic to true; which prevents it from being affected by physics. 


    */
    public GameObject explosionEffect;

    public GameObject damageSparksPrefab;
    public GameObject damageDustPrefab;

    public float health = 2f;
    bool hasExploded = false;

    public float blastRadius = 5f;
    public float explosionForce = 700f;
    Vector3 contactPoint;

    private TextMesh debugText;

    private bool debug = false;

    private bool allowDamage = true;

    private AudioSource explosionSource;
    private AudioClip explosion;

    private AudioSource metalHitSource;
    private AudioClip metalHit;



    void Awake()
    {

    }
    void Start()
    {

        try
        {
            debugText = GameObject.Find("debugText").GetComponent<TextMesh>();
            //MyDebug("box stuff");

            explosionSource = GameObject.FindGameObjectWithTag("swExplosion_Sound").GetComponent<AudioSource>();
            explosion = explosionSource.clip;

            metalHitSource = GameObject.FindGameObjectWithTag("swMetalHit_Sound").GetComponent<AudioSource>();
            metalHit = metalHitSource.clip;

        }
        catch (System.Exception e)
        {

            MyDebug("box stuff, error: " + e.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {

        MyDebug("Health is : " + health + " and allowDamage :" + allowDamage);
        if (health <= 0f)
        {
            Explode();
        }

        destroyIfIrrelevantNow();
    }

    private void OnCollisionEnter(Collision collision)
    {

        ContactPoint cp = collision.contacts[0];
        contactPoint = cp.point;

        if (collision.gameObject.tag == "miniTieBlast")
        {

            if (allowDamage)
            {
                health -= 1f;
                handleHit();
            }
            pauseDamageIfNecessary();
        }
        else if (collision.gameObject.tag == "PlayerShooter1")
        {
            health = 0f;// immediately explode if box hits player shooter
        }
        else
        {
            health = 0f;// something unexpected
        }

        // MyDebug("box collided with : " + collision.gameObject.tag);
    }


    void destroyIfIrrelevantNow()
    {
        // this object becomes irrelevant if it has flown past the shooter 

        GameObject shooter = GameObject.FindGameObjectWithTag("PlayerShooter");
        float shooterZ = shooter.transform.position.z;

        float myZ = transform.position.z;

        if (myZ < (shooterZ - 3f))
        {
            LevelManager.decrementNumSpawned();//since player one didn't kill me, get LevelManager to spawn me again
            destroySelf();
        }


    }
    void Explode()
    {
        //show effect 

        try
        {
            GameObject explosion = Instantiate(explosionEffect, contactPoint, transform.rotation);
            PlayExplosionSound_Immediately();


            destroySelf();

        }
        catch (System.Exception e)
        {

            MyDebug("Explode error: " + e.ToString());
        }

    }

    void destroySelf()
    {
        Destroy(gameObject);
    }
    void MyDebug(string someText)
    {

        if (debugText != null & debug)
        {
            debugText.text = someText;
        }

    }

    void pauseDamageIfNecessary()
    {
        if (allowDamage)
        {
            allowDamage = false;
            StartCoroutine(performDamagePause());
        }

    }

    IEnumerator performDamagePause()
    {

        float myDelay = 0.1f;
        yield return new WaitForSeconds(myDelay);

        allowDamage = true;

    }

    IEnumerator performPauseAndPlayHitSound()
    {

        float myDelay = 0.3f;
        yield return new WaitForSeconds(myDelay);
        PlayMetalHitSound_Immediately();
    }

    void handleDamage()
    {


        GameObject damageObject = Instantiate(damageSparksPrefab, contactPoint, transform.rotation);
        damageObject.transform.SetParent(this.transform);



        damageObject = Instantiate(damageDustPrefab, contactPoint, transform.rotation);
        damageObject.transform.SetParent(this.transform);




    }



    void handleHit()
    {

        handleDamage();

        StartCoroutine(performPauseAndPlayHitSound());

    }

    void PlayExplosionSound_Immediately()
    {
        explosionSource.PlayOneShot(explosion, 1.0f);

    }

    void PlayMetalHitSound_Immediately()
    {
        metalHitSource.PlayOneShot(metalHit, 0.8f); //0.8

    }

    void playHitOnDifferentThread()
    {

        StartCoroutine(PlayHitSound());

    }

    IEnumerator PlayHitSound()
    {
        //https://answers.unity.com/questions/904981/how-to-play-an-audio-file-after-another-finishes.html

        metalHitSource.PlayOneShot(metalHit, 0.7f);
        yield return new WaitForSeconds(metalHit.length);

    }

}