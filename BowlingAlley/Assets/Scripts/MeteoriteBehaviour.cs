using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteBehaviour : MonoBehaviour
{

    public GameObject damageDustPrefab;
    public GameObject sparkExplosionPrefab;
    private Rigidbody rb;
    private float Speed;
    private float AngularSpeed;
    private float hitVolume = 0.1f;

    //public GameObject muzzleFlashEffect;
    private AudioSource metalHitSource;
    private AudioClip metalHit;
    public float health = 2f;

    bool hasExploded = false;

    Vector3 contactPoint;
    private bool allowDamage = true;

    private TextMesh debugText;
    private bool debug = false;

    private AudioSource explosionSource;
    private AudioClip explosion;
    private float explosionVolume = 0.1f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        metalHitSource = GameObject.FindGameObjectWithTag("swMetalHit_Sound").GetComponent<AudioSource>();
        metalHit = metalHitSource.clip;

        debugText = GameObject.Find("debugText").GetComponent<TextMesh>();

        explosionSource = GameObject.FindGameObjectWithTag("swExplosion_Sound").GetComponent<AudioSource>();
        explosion = explosionSource.clip;

        
    }
  
    void FixedUpdate()
    {
       
        Speed = rb.velocity.magnitude;
        AngularSpeed = rb.angularVelocity.magnitude * Mathf.Rad2Deg;
        var q = Quaternion.AngleAxis(45, Vector3.left);
         
        float angle;
        Vector3 axis;
        q.ToAngleAxis(out angle, out axis);

        rb.angularVelocity = axis * angle * Mathf.Deg2Rad;

        if (health <= 0f)
        {
            Explode();
        }
       
        destroyIfIrrelevantNow();
    }


    IEnumerator performPauseAndPlayHitSound()
    {

        float myDelay = 0.3f;
        yield return new WaitForSeconds(myDelay);
        PlayMetalHitSound_Immediately();
    }

    void PlayMetalHitSound_Immediately()
    {
        metalHitSource.PlayOneShot(metalHit, hitVolume); //0.8

    }


    void handleHit()
    {

        handleDamage();

        StartCoroutine(performPauseAndPlayHitSound());

    }

    void handleDamage()
    {
        //GameObject damageObject = Instantiate(muzzleFlashEffect, contactPoint, muzzleFlashEffect.transform.rotation);
        //damageObject.transform.Rotate(0f, 45f, 0f);
        //damageObject.transform.SetParent(this.transform);

        GameObject damageObject2 = Instantiate(damageDustPrefab, contactPoint, transform.rotation);
        damageObject2.transform.SetParent(this.transform);

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
    private void OnCollisionEnter(Collision collision)
    {

        ContactPoint cp = collision.contacts[0];
        contactPoint = cp.point;

        if ( collision.gameObject.tag == "miniTieBlast") 
        {

            if (allowDamage)
            {
                health -= 1f;
                handleHit();
            }
            pauseDamageIfNecessary();
        }
        else if (collision.gameObject.tag == "PlayerShooter")
        {
            health = 0f; // immediately explode if meteorite hits player shooter
        }
        else if (collision.gameObject.tag == "falcon")
        {
            health = 0f; // immediately explode if falcon hits meteorite
        }
        else if (collision.gameObject.tag == "targetXWing")
        {
            health = 0f; // immediately explode if falcon hits meteorite
        }
        else if (collision.gameObject.tag == "miniTieMissle")
        {
            health = 0f; // immediately explode if falcon hits meteorite
        }
        else if (collision.gameObject.tag == "deathStarMissle")
        {
            health = 0f; // immediately explode if falcon hits meteorite
        }
        else if (collision.gameObject.tag == "starDestroyer")
        {
            health = 0f; // immediately explode if falcon hits meteorite
        }
        else if ( (collision.gameObject.tag == "slave1Blast") || (collision.gameObject.tag == "slave1"))
        {
            health = 0f; // immediately explode if falcon hits meteorite
        }
        else if (collision.gameObject.tag == "starDestroyerBlast")
        {
            health = 0f; // immediately explode if falcon hits meteorite
        }        
    }

    void destroyIfIrrelevantNow()
    {
        // this object becomes irrelevant if it has flown past the shooter 

        GameObject shooter = GameObject.FindGameObjectWithTag("PlayerShooter");
        float shooterZ = shooter.transform.position.z;

        float myZ = transform.position.z;

        if (myZ < (shooterZ - 3f))
        {
            LevelManager.decrementNumSpawned(); //since player one didn't kill me, get LevelManager to spawn me again
            destroySelf();
        }

    }

   void PlayExplosionSound_Immediately()
    {
        explosionSource.PlayOneShot(explosion, explosionVolume * 3.0f);

    }

    void Explode()
    {
        //show effect 

        try
        {

            // show sparks
            GameObject damageObject = Instantiate(sparkExplosionPrefab, contactPoint, sparkExplosionPrefab.transform.rotation);

            // show large dust explosion
            damageObject = Instantiate(damageDustPrefab, contactPoint, transform.rotation);
            // produce a dust explosion that is larger than the small
            // ones we do on impact
            float x = 150.0f;
            float y = 150.0f;
            float z = 0.0f;
            damageObject.transform.localScale += new Vector3(x, y, z);

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
}
