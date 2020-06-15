using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMilleniumFalconScript : MonoBehaviour
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

    private float energyExplosionVolume = 0.75f;

    public GameObject energyExplosionEffect;


    public GameObject explosionEffect;

    public GameObject damageSparksPrefab;
    public GameObject damageDustPrefab;

    private AudioSource energyExplosionSource;
    private AudioClip energyExplosion;


    public float health = 2f;
    bool hasExploded = false;
    public float blastRadius = 5f;
    public float explosionForce = 700f;
    Vector3 contactPoint;
    Vector3 missleContactPoint;
    private TextMesh debugText;

    private bool debug = true;

    bool roarSoundIsPlaying = false;

    private AudioSource roarSource;
    private AudioClip roar;

    private AudioSource blasterSource;
    private AudioClip blaster;

    private AudioSource situationNormalSource;
    private AudioClip situationNormal;

    private AudioSource chewyRoarSource;
    private AudioClip chewyRoar;

    private AudioSource explosionSource;
    private AudioClip explosion;

    public bool facingTieFighter = false;
    public float roarVolume = 0.7f;

    bool receivedContact = false;

    private float metalHitVolume = 0.1f;
    public float blastVolume = 0.3f;
    private GameObject blastPrefab;
    private float blasterSpeed = 180.0f;

    private AudioSource metalHitSource;
    private AudioClip metalHit;

    private bool allowDamage = true;

    private GameObject topBlasterOrigin;


    private float shootingPauseTimer;
    bool shootingAllowed = false;

    private float delayBeforeFirstShot;

    private bool playingEnergyExplosion = false;
    private bool playingPostEnergyExplosionSounds = false;

    void Awake()
    {

        //  MyDebug("MF : beginning of Awake");
        blastPrefab = PrefabFactory.getPrefab("falconBlast");


        roarSource = GameObject.FindGameObjectWithTag("falconRoar_Sound").GetComponent<AudioSource>();
        roar = roarSource.clip;

        energyExplosionSource = GameObject.FindGameObjectWithTag("energyExplosion_Sound").GetComponent<AudioSource>();
        energyExplosion = energyExplosionSource.clip;

        blasterSource = GameObject.FindGameObjectWithTag("falconBlasts_Sound").GetComponent<AudioSource>();
        blaster = blasterSource.clip;

        metalHitSource = GameObject.FindGameObjectWithTag("swMetalHit_Sound").GetComponent<AudioSource>();
        metalHit = metalHitSource.clip;

        explosionSource = GameObject.FindGameObjectWithTag("swExplosion_Sound").GetComponent<AudioSource>();
        explosion = explosionSource.clip;


        situationNormalSource = GameObject.FindGameObjectWithTag("hanSoloSituationNormal_Sound").GetComponent<AudioSource>();
        situationNormal = situationNormalSource.clip;


        chewyRoarSource = GameObject.FindGameObjectWithTag("chewyRoar_Sound").GetComponent<AudioSource>();
        chewyRoar = chewyRoarSource.clip;


        debugText = GameObject.Find("debugText").GetComponent<TextMesh>();

        // MyDebug("MF : end of Awake");
    }
    // Start is called before the first frame update
    void Start()
    {
        shootingPauseTimer = GetRandomShootingPauseAmount(); //pause between each shot

        delayBeforeFirstShot = GetRandomStartDelay(); // pause before starting to shoot

        pauseBeforeShootingOnDifferentThread(); // start the pause (before starting to shoot)

    }

    // Update is called once per frame
    void Update()
    {


        playRoaringSoundIfItIsTime();

        // enemy must have a delay before starting to shoot
        if (shootingAllowed)
        {
            shootIfTime();
        }

        // BUT, they can receive damage right away
        if (health <= 0f)
        {
            Explode();
        }

        destroyIfIrrelevantNow();

    }

    void FixedUpdate()
    {

        if (receivedContact)
        {
            showExplosionAtContactPoint();
            receivedContact = false;
        }

    }

    void PlayExplosionSound_Immediately()
    {
        explosionSource.PlayOneShot(explosion, blastVolume * 3.0f);

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

    void destroyIfIrrelevantNow()
    {
        // this object becomes irrelevant if it has flown past the shooter 

        GameObject shooter = GameObject.FindGameObjectWithTag("PlayerShooter");
        float shooterZ = shooter.transform.position.z;

        float myZ = transform.position.z;

        if (myZ < (shooterZ - 3f))
        {

            destroySelf();
        }

    }

    void destroySelf()
    {
        Destroy(gameObject);
    }

    void playRoaringSoundIfItIsTime()
    {

        if (!roarSoundIsPlaying)
            playRoarOnDifferentThread();

    }

    void playRoarOnDifferentThread()
    {

        StartCoroutine(PlayRoarSound());

    }

    IEnumerator PlaySituationNormalSound()
    {
        playingPostEnergyExplosionSounds = true;
        PlaySituationNormalSound_Immediately();
        yield return new WaitForSeconds(situationNormal.length);
        PlayChewyRoar_Immediately();
        yield return new WaitForSeconds(chewyRoar.length);
        playingPostEnergyExplosionSounds = false;


    }

    IEnumerator PlayRoarSound()
    {
        //MyDebug("playing MF roar ");
        try
        {
            //https://answers.unity.com/questions/904981/how-to-play-an-audio-file-after-another-finishes.html
            roarSoundIsPlaying = true;
            roarSource.PlayOneShot(roar, roarVolume);
        }
        catch (System.Exception e)
        {
            MyDebug("error playing MF roar :" + e.Message);
        }
        yield return new WaitForSeconds(roar.length + 5.0f);

        roarSoundIsPlaying = false;
        //MyDebug("finished playing MF roar ");
    }

    void spawnNewBlasterBolt()
    {
        // recreating ball


        topBlasterOrigin = GameObject.FindGameObjectWithTag("mfTopBlasterPosition");
        float x = topBlasterOrigin.transform.position.x;
        float y = topBlasterOrigin.transform.position.y;
        float z = topBlasterOrigin.transform.position.z;
        if (facingTieFighter)
        {
            z = z - 0.05f;
        }
        GameObject go = (GameObject)Instantiate(blastPrefab, new Vector3(x, y, z), Quaternion.identity);
        GameObject bolt1 = PrefabFactory.GetChildWithName(go, "falconLeftBlast");
        GameObject bolt2 = PrefabFactory.GetChildWithName(go, "falconRightBlast");

        bolt1.GetComponent<Rigidbody>().velocity = blasterSpeed * topBlasterOrigin.transform.forward * Time.deltaTime;
        bolt2.GetComponent<Rigidbody>().velocity = blasterSpeed * topBlasterOrigin.transform.forward * Time.deltaTime;



    }
    IEnumerator shoot()
    {
        PlayBlasterSound_Immediately();
        float shotDelay = 0.1f;

        // six shots        
        spawnNewBlasterBolt();
        yield return new WaitForSeconds(shotDelay);
        spawnNewBlasterBolt();
        yield return new WaitForSeconds(shotDelay);
        spawnNewBlasterBolt();
        yield return new WaitForSeconds(shotDelay);
        spawnNewBlasterBolt();
        yield return new WaitForSeconds(shotDelay);
        spawnNewBlasterBolt();
        yield return new WaitForSeconds(shotDelay);
        spawnNewBlasterBolt();
        yield return new WaitForSeconds(shotDelay);

        // six shots        
        spawnNewBlasterBolt();
        yield return new WaitForSeconds(shotDelay);
        spawnNewBlasterBolt();
        yield return new WaitForSeconds(shotDelay);
        spawnNewBlasterBolt();
        yield return new WaitForSeconds(shotDelay);
        spawnNewBlasterBolt();
        yield return new WaitForSeconds(shotDelay);
        spawnNewBlasterBolt();
        yield return new WaitForSeconds(shotDelay);
        spawnNewBlasterBolt();
        yield return new WaitForSeconds(shotDelay);

        // six shots        
        spawnNewBlasterBolt();
        yield return new WaitForSeconds(shotDelay);
        spawnNewBlasterBolt();
        yield return new WaitForSeconds(shotDelay);
        spawnNewBlasterBolt();
        yield return new WaitForSeconds(shotDelay);
        spawnNewBlasterBolt();
        yield return new WaitForSeconds(shotDelay);
        spawnNewBlasterBolt();
        yield return new WaitForSeconds(shotDelay);
        spawnNewBlasterBolt();

    }

    void PlayBlasterSound_Immediately()
    {
        // we want a seperate object for each fire; so we can handle multi blasts in quick succession

        // Method I used to achieve multiple blasts that don't interrupt each other:
        // https://docs.unity3d.com/ScriptReference/AudioSource.PlayOneShot.html
        blasterSource.PlayOneShot(blaster, blastVolume);

    }

    void PlaySituationNormalSound_Immediately()
    {
        // we want a seperate object for each fire; so we can handle multi blasts in quick succession

        // Method I used to achieve multiple blasts that don't interrupt each other:
        // https://docs.unity3d.com/ScriptReference/AudioSource.PlayOneShot.html
        situationNormalSource.PlayOneShot(situationNormal, blastVolume * 2.0f);

    }

    void PlayChewyRoar_Immediately()
    {
        // we want a seperate object for each fire; so we can handle multi blasts in quick succession

        // Method I used to achieve multiple blasts that don't interrupt each other:
        // https://docs.unity3d.com/ScriptReference/AudioSource.PlayOneShot.html
        chewyRoarSource.PlayOneShot(chewyRoar, blastVolume * 2.0f);

    }


    float GetRandomStartDelay()
    {
        return Random.Range(1.5f, 3.5f);

    }

    float GetRandomShootingPauseAmount()
    {

        return Random.Range(6.0f, 8.0f);

    }

    void pauseBeforeShootingOnDifferentThread()
    {

        StartCoroutine(pauseBeforeStartingToShoot());

    }

    bool okToShoot()
    {
        return (shootingAllowed && !playingEnergyExplosion && !playingPostEnergyExplosionSounds);
    }
    IEnumerator pauseBeforeStartingToShoot()
    {
        yield return new WaitForSeconds(delayBeforeFirstShot);
        shootingAllowed = true;
    }

    void shootIfTime()
    {

        shootingPauseTimer -= Time.deltaTime;

        if (!okToShoot()) return; // don't shoot if energy explosion is not done

        if (shootingPauseTimer <= 0f)
        {
            StartCoroutine(shoot());
            shootingPauseTimer = GetRandomShootingPauseAmount();

        }
    }

    void handleDamage()
    {

        GameObject damageObject = Instantiate(damageSparksPrefab, contactPoint, transform.rotation);
        damageObject.transform.SetParent(this.transform);

        damageObject = Instantiate(damageDustPrefab, contactPoint, transform.rotation);
        damageObject.transform.SetParent(this.transform);

    }

    void PlayMetalHitSound_Immediately()
    {
        metalHitSource.PlayOneShot(metalHit, metalHitVolume); //0.8

    }

    IEnumerator performPauseAndPlayHitSound()
    {

        float myDelay = 0.3f;
        yield return new WaitForSeconds(myDelay);
        PlayMetalHitSound_Immediately();
    }

    void handleHit()
    {

        handleDamage();

        StartCoroutine(performPauseAndPlayHitSound());

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
        else if (collision.gameObject.tag == "falconBlast")
        {
            // do nothing. My own blaster appeared within my collider. 
            // it hasn't been hit by a blaster...its mine
        }
        else if (collision.gameObject.tag == "miniTieMissle")
        {
            missleContactPoint = cp.point;

            receivedContact = true;
        }
        else
        {
            health = 0f; // something unexpected
        }



        //MyDebug("box collided with : " + collision.gameObject.tag);
    }

    void showExplosionAtContactPoint()
    {

        Debug.Log("BOOM!");

        //show effect 
        GameObject explosion = Instantiate(energyExplosionEffect, missleContactPoint, transform.rotation);

        explosion.transform.parent = this.transform;//make explosion the child of ship - to make it stick and move with ship in same spot

        //then scale it down
        float value = 0.6f;// sweet spot. see cloud and lightning best
        float x = value;
        float y = value;
        float z = value;

        explosion.transform.localScale -= new Vector3(x, y, z);

        if (!playingEnergyExplosion)
        {
            playingEnergyExplosion = true;
            StartCoroutine(PlayEnergyExplosionSound());
        }
    }

    void PlayEnergyExplosionSound_Immediately()
    {
        energyExplosionSource.PlayOneShot(energyExplosion, energyExplosionVolume);
    }

    IEnumerator PlayEnergyExplosionSound()
    {
        PlayEnergyExplosionSound_Immediately();
        yield return new WaitForSeconds(energyExplosion.length);
        playingEnergyExplosion = false;
        if (!playingPostEnergyExplosionSounds)
        {
            StartCoroutine(PlaySituationNormalSound());
        }
    }

    void MyDebug(string someText)
    {

        if (debugText != null & debug)
        {
            debugText.text = someText;
        }

    }

}
