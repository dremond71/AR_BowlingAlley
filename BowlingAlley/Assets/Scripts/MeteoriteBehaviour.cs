using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteBehaviour : MonoBehaviour
{

    private Rigidbody rb;
    private float Speed;
    private float AngularSpeed;
    private float hitVolume = 0.1f;

    public GameObject muzzleFlashEffect;
    private AudioSource metalHitSource;
    private AudioClip metalHit;
    public float health = 2f;
    
    Vector3 contactPoint;
    private bool allowDamage = true;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        metalHitSource = GameObject.FindGameObjectWithTag("swMetalHit_Sound").GetComponent<AudioSource>();
        metalHit = metalHitSource.clip;
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
        GameObject damageObject = Instantiate(muzzleFlashEffect, contactPoint, muzzleFlashEffect.transform.rotation);
        damageObject.transform.Rotate(0f, 45f, 0f);
        damageObject.transform.SetParent(this.transform);

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

        if (collision.gameObject.tag == "miniTieBlast")
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
            health = 0f; // immediately explode if box hits player shooter
        }
        else if (collision.gameObject.tag == "miniXWingBlast")
        {
            // do nothing. My own blaster appeared within my collider. 
            // it hasn't been hit by a blaster...its mine
        }
        else
        {
            health = 0f; // something unexpected
        }

    }


}
