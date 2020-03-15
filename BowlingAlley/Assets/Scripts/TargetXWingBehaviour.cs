using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetXWingBehaviour : MonoBehaviour {

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
    private float blasterVolume = 0.1f;
    private AudioSource metalHitSource;
    private AudioClip metalHit;

    private GameObject xwingBlastPrefab;
    private float blasterSpeed = 180.0f;

    private GameObject xwingBlastOrigin1;
    private GameObject xwingBlastOrigin2;
    private GameObject xwingBlastOrigin3;
    private GameObject xwingBlastOrigin4;

    private AudioSource blasterSource;
    private AudioClip blaster;

    private float shootingPauseTimer;
    bool shootingAllowed = false;

    private float delayBeforeFirstShot;

    bool roarSoundIsPlaying = false;

    private AudioSource roarSource;
    private AudioClip roar;

    public float roarVolume = 0.7f;

    float GetRandomStartDelay () {
        return Random.Range (1.5f, 3.5f);

    }

    float GetRandomShootingPauseAmount () {

        return Random.Range (0.8f, 1.75f);

    }
    void shootIfTime () {

        shootingPauseTimer -= Time.deltaTime;
        if (shootingPauseTimer <= 0f) {
            shoot ();
            shootingPauseTimer = GetRandomShootingPauseAmount ();

        }
    }
    void Awake () {
        xwingBlastPrefab = PrefabFactory.getPrefab ("miniXWingBlast");
        blasterSource = GameObject.FindGameObjectWithTag ("xwingFighterBlasts_Sound").GetComponent<AudioSource> ();
        blaster = blasterSource.clip;
        roarSource = GameObject.FindGameObjectWithTag ("xwingFighterRoar_Sound").GetComponent<AudioSource> ();
        roar = roarSource.clip;

    }
    void Start () {

        try {
            debugText = GameObject.Find ("debugText").GetComponent<TextMesh> ();
            //MyDebug("box stuff");

            explosionSource = GameObject.FindGameObjectWithTag ("swExplosion_Sound").GetComponent<AudioSource> ();
            explosion = explosionSource.clip;

            metalHitSource = GameObject.FindGameObjectWithTag ("swMetalHit_Sound").GetComponent<AudioSource> ();
            metalHit = metalHitSource.clip;

            shootingPauseTimer = GetRandomShootingPauseAmount (); //pause between each shot

            delayBeforeFirstShot = GetRandomStartDelay (); // pause before starting to shoot

            pauseBeforeShootingOnDifferentThread (); // start the pause (before starting to shoot)
        } catch (System.Exception e) {

            MyDebug ("box stuff, error: " + e.ToString ());
        }
    }

    void playRoaringSoundIfItIsTime () {

        if (!roarSoundIsPlaying)
            playRoarOnDifferentThread ();

    }
    // Update is called once per frame
    void Update () {

        playRoaringSoundIfItIsTime ();

        // enemy must have a delay before starting to shoot
        if (shootingAllowed) {
            shootIfTime ();
        }
        //  MyDebug("Health is : " + health + " and allowDamage :" + allowDamage);

        // BUT, they can receive damage right away
        if (health <= 0f) {
            Explode ();
        }

        destroyIfIrrelevantNow ();
    }

    private void OnCollisionEnter (Collision collision) {

        ContactPoint cp = collision.contacts[0];
        contactPoint = cp.point;

        if (collision.gameObject.tag == "miniTieBlast") {

            if (allowDamage) {
                health -= 1f;
                handleHit ();
            }
            pauseDamageIfNecessary ();
        } else if (collision.gameObject.tag == "PlayerShooter") {
            health = 0f; // immediately explode if box hits player shooter
        } else if (collision.gameObject.tag == "miniXWingBlast") {
            // do nothing. My own blaster appeared within my collider. 
            // it hasn't been hit by a blaster...its mine
        } else {
            health = 0f; // something unexpected
        }

        MyDebug ("box collided with : " + collision.gameObject.tag);
    }

    void destroyIfIrrelevantNow () {
        // this object becomes irrelevant if it has flown past the shooter 

        GameObject shooter = GameObject.FindGameObjectWithTag ("PlayerShooter");
        float shooterZ = shooter.transform.position.z;

        float myZ = transform.position.z;

        if (myZ < (shooterZ - 3f)) {
            LevelManager.decrementNumSpawned (); //since player one didn't kill me, get LevelManager to spawn me again
            destroySelf ();
        }

    }
    void Explode () {
        //show effect 

        try {
            GameObject explosion = Instantiate (explosionEffect, contactPoint, transform.rotation);
            PlayExplosionSound_Immediately ();

            destroySelf ();

        } catch (System.Exception e) {

            MyDebug ("Explode error: " + e.ToString ());
        }

    }

    void destroySelf () {
        Destroy (gameObject);
    }
    void MyDebug (string someText) {

        if (debugText != null & debug) {
            debugText.text = someText;
        }

    }

    void pauseDamageIfNecessary () {
        if (allowDamage) {
            allowDamage = false;
            StartCoroutine (performDamagePause ());
        }

    }

    IEnumerator performDamagePause () {

        float myDelay = 0.1f;
        yield return new WaitForSeconds (myDelay);

        allowDamage = true;

    }

    IEnumerator pauseBeforeStartingToShoot () {
        yield return new WaitForSeconds (delayBeforeFirstShot);
        shootingAllowed = true;
    }

    IEnumerator performPauseAndPlayHitSound () {

        float myDelay = 0.3f;
        yield return new WaitForSeconds (myDelay);
        PlayMetalHitSound_Immediately ();
    }

    void handleDamage () {

        GameObject damageObject = Instantiate (damageSparksPrefab, contactPoint, transform.rotation);
        damageObject.transform.SetParent (this.transform);

        damageObject = Instantiate (damageDustPrefab, contactPoint, transform.rotation);
        damageObject.transform.SetParent (this.transform);

    }

    void handleHit () {

        handleDamage ();

        StartCoroutine (performPauseAndPlayHitSound ());

    }

    void PlayExplosionSound_Immediately () {
        explosionSource.PlayOneShot (explosion, blasterVolume * 3.0f);

    }

    void PlayMetalHitSound_Immediately () {
        metalHitSource.PlayOneShot (metalHit, blasterVolume); //0.8

    }

    void pauseBeforeShootingOnDifferentThread () {

        StartCoroutine (pauseBeforeStartingToShoot ());

    }

    void playHitOnDifferentThread () {

        StartCoroutine (PlayHitSound ());

    }

    IEnumerator PlayHitSound () {
        //https://answers.unity.com/questions/904981/how-to-play-an-audio-file-after-another-finishes.html

        metalHitSource.PlayOneShot (metalHit, blasterVolume);
        yield return new WaitForSeconds (metalHit.length);

    }

    void spawnNewBlasterBolt () {
        // recreating ball

        // bolt 1

        xwingBlastOrigin1 = PrefabFactory.GetChildWithName (gameObject, "xwingBlasterPosition1");
        float x = xwingBlastOrigin1.transform.position.x;
        float y = xwingBlastOrigin1.transform.position.y;
        float z = xwingBlastOrigin1.transform.position.z;

        GameObject go = (GameObject) Instantiate (xwingBlastPrefab, new Vector3 (x, y, z), xwingBlastOrigin1.transform.rotation);
        GameObject bolt1 = PrefabFactory.GetChildWithName (go, "miniXWingBlast2");

        // bolt 2
        xwingBlastOrigin2 = PrefabFactory.GetChildWithName (gameObject, "xwingBlasterPosition2");
        x = xwingBlastOrigin2.transform.position.x;
        y = xwingBlastOrigin2.transform.position.y;
        z = xwingBlastOrigin2.transform.position.z;

        go = (GameObject) Instantiate (xwingBlastPrefab, new Vector3 (x, y, z), xwingBlastOrigin2.transform.rotation);

        GameObject bolt2 = PrefabFactory.GetChildWithName (go, "miniXWingBlast2");

        // bolt 3
        xwingBlastOrigin3 = PrefabFactory.GetChildWithName (gameObject, "xwingBlasterPosition3");
        x = xwingBlastOrigin3.transform.position.x;
        y = xwingBlastOrigin3.transform.position.y;
        z = xwingBlastOrigin3.transform.position.z;

        go = (GameObject) Instantiate (xwingBlastPrefab, new Vector3 (x, y, z), xwingBlastOrigin3.transform.rotation);
        GameObject bolt3 = PrefabFactory.GetChildWithName (go, "miniXWingBlast2");

        // bolt 4
        xwingBlastOrigin4 = PrefabFactory.GetChildWithName (gameObject, "xwingBlasterPosition4");
        x = xwingBlastOrigin4.transform.position.x;
        y = xwingBlastOrigin4.transform.position.y;
        z = xwingBlastOrigin4.transform.position.z;

        go = (GameObject) Instantiate (xwingBlastPrefab, new Vector3 (x, y, z), xwingBlastOrigin4.transform.rotation);
        GameObject bolt4 = PrefabFactory.GetChildWithName (go, "miniXWingBlast2");

        bolt1.GetComponent<Rigidbody> ().velocity = blasterSpeed * xwingBlastOrigin1.transform.forward * Time.deltaTime;
        bolt2.GetComponent<Rigidbody> ().velocity = blasterSpeed * xwingBlastOrigin1.transform.forward * Time.deltaTime;
        bolt3.GetComponent<Rigidbody> ().velocity = blasterSpeed * xwingBlastOrigin1.transform.forward * Time.deltaTime;
        bolt4.GetComponent<Rigidbody> ().velocity = blasterSpeed * xwingBlastOrigin1.transform.forward * Time.deltaTime;

    }

    void PlayBlasterSound_Immediately () {
        // we want a seperate object for each fire; so we can handle multi blasts in quick succession

        // Method I used to achieve multiple blasts that don't interrupt each other:
        // https://docs.unity3d.com/ScriptReference/AudioSource.PlayOneShot.html
        blasterSource.PlayOneShot (blaster, blasterVolume);

    }

    void shoot () {
        spawnNewBlasterBolt ();
        PlayBlasterSound_Immediately ();
    }

    void playRoarOnDifferentThread () {

        StartCoroutine (PlayRoarSound ());

    }

    IEnumerator PlayRoarSound () {
        //https://answers.unity.com/questions/904981/how-to-play-an-audio-file-after-another-finishes.html
        roarSoundIsPlaying = true;
        roarSource.PlayOneShot (roar, roarVolume);
        yield return new WaitForSeconds (roar.length);
        roarSoundIsPlaying = false;

    }

}