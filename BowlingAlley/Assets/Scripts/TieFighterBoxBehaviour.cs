using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TieFighterBoxBehaviour : MonoBehaviour
{

    public GameObject explosionEffect;
    public GameObject sparksEffect;
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

    List<GameObject> sparksList = new List<GameObject>();

    void Awake()
    {

    }
    void Start()
    {

        try
        {
            debugText = GameObject.Find("debugText").GetComponent<TextMesh>();
            MyDebug("box stuff");

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
    }

    private void OnCollisionEnter(Collision collision)
    {


        if (collision.gameObject.tag == "starwars_weapon_blast")
        {
            health = 0f;
            ContactPoint cp = collision.contacts[0];
            contactPoint = cp.point;

            // if (allowDamage)
            // {
            //     //health -= 1f;
            //     handleHit();
            // }
            // pauseDamageIfNecessary();
        }
        else
        {
            health = 0f;// immediately explode if box hits player
        }

    }

    void Explode()
    {
        //show effect 

        try
        {
            GameObject explosion = Instantiate(explosionEffect, contactPoint, transform.rotation);
            PlayExplosionSound_Immediately();

            deleteAllSparks();
            Destroy(gameObject);

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

    void handleSparks()
    {
        //show spark effect 
        GameObject sparksObject = Instantiate(sparksEffect, contactPoint, transform.rotation);
        sparksList.Add(sparksObject);
    }

    void deleteAllSparks()
    {
        foreach (GameObject spark in sparksList)
        {
            Destroy(spark);
        }
    }
    void handleHit()
    {

        handleSparks();

        // PlayMetalHitSound_Immediately();
        //playHitOnDifferentThread ();
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