﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarDestroyerBehaviour : MonoBehaviour
{
    private AudioSource metalHitSource;
    private AudioClip metalHit;
    private float blasterVolume = 0.1f;
    
    Vector3 contactPoint;

    public float health = 999999999f;
    public GameObject muzzleFlashEffect;

    GameObject target;
    string targetTag = null;

    private float shootingPauseTimer;

    bool shootingAllowed = false;

    private GameObject turboLasterBlastPrefab;
    private float blasterSpeed = 180.0f * 2.0f;
    private float delayBeforeFirstShot;

    private AudioSource deathStarBlastSource;
    private AudioClip deathStarBlast;
    private float deathStarBlastVolume = 0.8f;

    private bool debug = true;
    private TextMesh debugText;
    private float turboLaserSpeedFactor = 30f;

    // Start is called before the first frame update
    void Start()
    {
            metalHitSource = GameObject.FindGameObjectWithTag("swMetalHit_Sound").GetComponent<AudioSource>();
            metalHit = metalHitSource.clip;   

            turboLasterBlastPrefab = PrefabFactory.getPrefab("starDestroyerBlast");    

            deathStarBlastSource = GameObject.FindGameObjectWithTag("deathStarBlast_Sound").GetComponent<AudioSource>();
            deathStarBlast = deathStarBlastSource.clip;

            shootingPauseTimer = GetRandomShootingPauseAmount(); //pause between each shot

            delayBeforeFirstShot = GetRandomStartDelay(); // pause before starting to shoot

            pauseBeforeShootingOnDifferentThread(); // start the pause (before starting to shoot)
            debugText = GameObject.Find("debugText").GetComponent<TextMesh>();

    }

    void MyDebug(string someText)
    {

        if (debugText != null & debug)
        {
            debugText.text = someText;
        }

    }

    void PlayDeathStarBlastSound_Immediately()
    {
        deathStarBlastSource.PlayOneShot(deathStarBlast, deathStarBlastVolume);

    }

    void pauseBeforeShootingOnDifferentThread()
    {
        StartCoroutine(pauseBeforeStartingToShoot());
    }

    IEnumerator pauseBeforeStartingToShoot()
    {
        yield return new WaitForSeconds(delayBeforeFirstShot);
        shootingAllowed = true;
    }

    float GetRandomStartDelay()
    {
        return Random.Range(1.0f,1.5f);
    }
    float GetRandomShootingPauseAmount()
    {
        return Random.Range(1.0f, 1.5f);
    }


    float GetRandomZAdjustment()
    {
        return Random.Range(-0.32f, -0.34f);
    }

    void chooseTarget2 () {
        target = GameObject.FindGameObjectWithTag("PlayerShooter");
    }

    void chooseTarget () {

       if (target != null) return;

        GameObject tantive    = GameObject.FindGameObjectWithTag ("tantiveIV");
        GameObject falcon     = GameObject.FindGameObjectWithTag ("falcon");

        GameObject[] tantives= new GameObject[]{};
        GameObject[] falcons= new GameObject[]{};
    
        if (tantive != null){
            tantives = new GameObject[] {tantive};
            tantives = LevelManager.filterGameObjectsInFrontOfPlayer(tantives);
        }

        if (falcons != null){
            falcons = new GameObject[] {falcon};
            falcons = LevelManager.filterGameObjectsInFrontOfPlayer(falcons);
        }

        GameObject[] xwings    = LevelManager.filterGameObjectsInFrontOfPlayer(GameObject.FindGameObjectsWithTag("targetXWing")); 
        GameObject[] meteorites = LevelManager.filterGameObjectsInFrontOfPlayer(GameObject.FindGameObjectsWithTag("targetMeteorite"));
        GameObject[] awings     = LevelManager.filterGameObjectsInFrontOfPlayer(GameObject.FindGameObjectsWithTag("targetAWing"));

        if (target == null)
        {
            if (xwings.Length > 0)
            {
                target = xwings[0];
                targetTag = "targetXWing";
            }
        }

        if (target == null)
        {
            if (awings.Length > 0)
            {
                target = awings[0];
                targetTag = "targetAWing";
            }
        }

        if (target == null)
        {
            if (meteorites.Length > 0)
            {
                target = meteorites[0];
                targetTag = "targetMeteorite";
            }
        }

        if (target == null)
        {
            if (falcons !=null && falcons.Length>0)
            {
                target = falcons[0];
                targetTag = "falcon";
            }
        }

        if (target == null)
        {
            if (tantives != null && tantives.Length>0)
            {
                target = tantives[0];
                targetTag = "tantiveIV";
            }
        }

    }


        private void OnCollisionEnter(Collision collision)
    {

        ContactPoint cp = collision.contacts[0];
        contactPoint = cp.point;

        if (collision.gameObject.tag == "miniTieBlast")
        {          
            health -= 1f;
            handleHit();
            
        }
        else if (collision.gameObject.tag == "miniXWingBlast")
        {
            health -= 1f;
            handleHit();
        }
        else if (collision.gameObject.tag == "falconBlast")
        {
            health -= 1f;
            handleHit();
        }        
        else if (collision.gameObject.tag == "miniTieMissle")
        {
            health -= 1f;
            handleHit(); 
        }
        else if (collision.gameObject.tag == "deathStarMissle")
        {
            health -= 1f;
            handleHit();
        }
        else
        {
            health -= 1f;
            handleHit();
        }

       // MyDebug("box collided with : " + collision.gameObject.tag);
   
    }

    void handleHit()
    {

        handleDamage();

    }
    void handleDamage()
    {

        GameObject damageObject = Instantiate(muzzleFlashEffect, contactPoint, muzzleFlashEffect.transform.rotation);
        damageObject.transform.Rotate(0f, 45f, 0f);
        damageObject.transform.SetParent(this.transform);


    }

    
    void destroyIfIrrelevantNow()
    {
        // this object becomes irrelevant 
        // IF it has flown past the death star; having been spawned from starDestroyerSpawnPoint1
        // some other condition if spawned from somewhere else

        GameObject deathStar = GameObject.FindGameObjectWithTag("deathStar");
        float deathStarZ = deathStar.transform.position.z;

        float myZ = transform.position.z;

        if (myZ > (deathStarZ + 10f))
        {
            LevelManager.starDestroyerAttackFinished(); // tells LevelManager that the cavalry attack is finished
            destroySelf();
        }

    }
    void destroySelf()
    {
        Destroy(gameObject);
    }

    void shoot()
    {
        spawnNewBlasterBolt();
        PlayDeathStarBlastSound_Immediately();
    }

    void spawnNewBlasterBolt()
    {

        try
        {
            // bolt 1
            GameObject sdTLBROrigin1 = GameObject.FindGameObjectWithTag("SD_TL_BR_BlasterPosition1");
            float x = sdTLBROrigin1.transform.position.x;
            float y = sdTLBROrigin1.transform.position.y;
            float z = sdTLBROrigin1.transform.position.z;

            GameObject go = (GameObject)Instantiate(turboLasterBlastPrefab, new Vector3(x, y, z), sdTLBROrigin1.transform.rotation);
            GameObject bolt1 = PrefabFactory.GetChildWithName(go, "sdTurboLaserBlast");

            // bolt 2
            GameObject sdTLBROrigin2 = GameObject.FindGameObjectWithTag("SD_TL_BR_BlasterPosition2");
            x = sdTLBROrigin2.transform.position.x;
            y = sdTLBROrigin2.transform.position.y;
            z = sdTLBROrigin2.transform.position.z;

            go = (GameObject)Instantiate(turboLasterBlastPrefab, new Vector3(x, y, z), sdTLBROrigin2.transform.rotation);
            GameObject bolt2 = PrefabFactory.GetChildWithName(go, "sdTurboLaserBlast");


            // bolt 3
            GameObject sdTLBROrigin3 = GameObject.FindGameObjectWithTag("SD_TL_BR_BlasterPosition3");
            x = sdTLBROrigin3.transform.position.x;
            y = sdTLBROrigin3.transform.position.y;
            z = sdTLBROrigin3.transform.position.z;

            go = (GameObject)Instantiate(turboLasterBlastPrefab, new Vector3(x, y, z), sdTLBROrigin3.transform.rotation);
            GameObject bolt3 = PrefabFactory.GetChildWithName(go, "sdTurboLaserBlast");

            // bolt 4
            GameObject sdTLBROrigin4 = GameObject.FindGameObjectWithTag("SD_TL_BR_BlasterPosition4");
            x = sdTLBROrigin4.transform.position.x;
            y = sdTLBROrigin4.transform.position.y;
            z = sdTLBROrigin4.transform.position.z;

            go = (GameObject)Instantiate(turboLasterBlastPrefab, new Vector3(x, y, z), sdTLBROrigin4.transform.rotation);
            GameObject bolt4 = PrefabFactory.GetChildWithName(go, "sdTurboLaserBlast");

            bolt1.GetComponent<Rigidbody>().velocity = blasterSpeed * sdTLBROrigin1.transform.forward * Time.deltaTime;
            bolt2.GetComponent<Rigidbody>().velocity = blasterSpeed * sdTLBROrigin2.transform.forward * Time.deltaTime;
            bolt3.GetComponent<Rigidbody>().velocity = blasterSpeed * sdTLBROrigin3.transform.forward * Time.deltaTime;
            bolt4.GetComponent<Rigidbody>().velocity = blasterSpeed * sdTLBROrigin4.transform.forward * Time.deltaTime;
        }
        catch (System.Exception e)
        {

            MyDebug("sd spawn bold, error: " + e.ToString());
        }

    }

    void shootIfTime()
    {

        shootingPauseTimer -= Time.deltaTime;
        if (shootingPauseTimer <= 0f)
        {
            if (target !=null){
                shoot();
                target = null;
                targetTag = null;
            }

            shootingPauseTimer = GetRandomShootingPauseAmount();

        }
    }
     void FixedUpdate()
    {
           
           destroyIfIrrelevantNow();

           
            chooseTarget();
          

            // SD (Star Destroyer) TL (Turbo Laser) BR (Bottom Right) Swivel
            GameObject swivel = GameObject.FindGameObjectWithTag("SD_TL_BR_Swivel");
            if (swivel) {
                
                if (target !=null){

                    if (targetTag == "tantiveIV") {
                        Vector3 lookPos = target.transform.position - swivel.transform.position;
                        Quaternion lookRot = Quaternion.LookRotation(lookPos, Vector3.up);
                        float eulerY = lookRot.eulerAngles.y;
                        float eulerX = lookRot.eulerAngles.x;
                        float eulerZ = lookRot.eulerAngles.z;
                        Quaternion rotation = Quaternion.Euler(eulerX, eulerY, 0);
                        swivel.transform.rotation = Quaternion.Slerp(swivel.transform.rotation, rotation, turboLaserSpeedFactor * Time.deltaTime);
                        
                    }
                    else {
                        float z_adjustment = GetRandomZAdjustment();
                        MyDebug("Z adj == " + z_adjustment);
                        Vector3 targetAdjustedPosition = new Vector3(target.transform.position.x,target.transform.position.y,target.transform.position.z + z_adjustment);
                        Vector3 lookPos = targetAdjustedPosition - swivel.transform.position;
                        Quaternion lookRot = Quaternion.LookRotation(lookPos, Vector3.up);
                        float eulerY = lookRot.eulerAngles.y;
                        float eulerX = lookRot.eulerAngles.x;
                        float eulerZ = lookRot.eulerAngles.z;
                        Quaternion rotation = Quaternion.Euler(eulerX, eulerY, 0);
                        swivel.transform.rotation = Quaternion.Slerp(swivel.transform.rotation, rotation, turboLaserSpeedFactor * Time.deltaTime);

                    }

                }
                else {
                    
                   // do not set it to zeros, but save the original position and angles, and set it to that. :)
                   // MyDebug("");
                   // Quaternion rotation = Quaternion.Euler(0, 0, 0);
                   // swivel.transform.rotation = Quaternion.Slerp(swivel.transform.rotation, rotation, turboLaserSpeedFactor * Time.deltaTime);        
                }

            } //if - swivel
            
            if (shootingAllowed)
            {
                shootIfTime();
            }

    } // FixedUpdate() 

    void FixedUpdate2()
    {
           
           destroyIfIrrelevantNow();

            GameObject[] allSwivels = GameObject.FindGameObjectsWithTag("sdTopLeftTurboLaserSwivel");
            if (allSwivels.Length > 0) {
                GameObject target         = GameObject.FindGameObjectWithTag ("PlayerShooter");
                for (int i = 0; i < allSwivels.Length; i++){
                    GameObject oneSwivel = allSwivels[i]; 

                       // move the turbo laser base left or right depending on the target's position
                       Vector3 targetPosition = new Vector3(target.transform.position.x, oneSwivel.transform.position.y, target.transform.position.z);
                       oneSwivel.transform.LookAt(targetPosition);


                       //
                       // Since I cannot seem to succeed at making the barrels to move up or down (without also changing x)
                       // then perhaps I should make bullets that track their target just like the missles?
                       // lobbing bullets at targets?
                       // OR maybe I should make the barrels the child of the barrel rod, and rotate the barrel rod up or down?


                    //    GameObject theBlastersParent = PrefabFactory.GetChildWithName(oneSwivel,"theBlasters");
                    //    if (theBlastersParent != null){
                    //       GameObject rod = PrefabFactory.GetChildWithName(theBlastersParent,"blasterEndCapRod");

                    //         // figuring out how to move barrel up and down is brutal
                    //         // similar question here: https://forum.unity.com/threads/help-aiming-a-tank-barrel-locked-y-look-rotation-to-target-turret-movement-is-fine-though.688750/

                            
                    //         // works with Y, but also tracks x :S
                    //         Vector3 lookPos = target.transform.position - rod.transform.position;
                    //         Quaternion lookRot = Quaternion.LookRotation(lookPos, Vector3.up);
                    //         float eulerY = lookRot.eulerAngles.y;
                    //         float eulerX = lookRot.eulerAngles.x;
                    //         float eulerZ = lookRot.eulerAngles.z;
                    //         // definitely NOT : Quaternion rotation = Quaternion.Euler (0, eulerY, 0);
                    //         Quaternion rotation = Quaternion.Euler (-eulerX, 0, 0);//best
                    //         //Quaternion rotation = Quaternion.Euler (-eulerX, 0, eulerZ);// nope
                    //         rod.transform.rotation = rotation;
                    //         //rod.transform.Rotate(90,-90,180);// almost perfect 
                    //         //rod.transform.Rotate(90,-45,180);// rod tilting to left
                    //         //rod.transform.Rotate(-90,-90,180);// almost perfect but pointing straight up
                    //         //rod.transform.Rotate(90,-90,180);// almost perfect but point straight down
                    //         //rod.transform.Rotate(135,-90,180);// 45 degrees up off horizontal facing me
                    //         //rod.transform.Rotate(90,-135,180);// 45 degrees down off horizontal facing me
                    //         //rod.transform.Rotate(90,-90,90);// crooked
                    //         //rod.transform.Rotate(90,-90,270);// 45 degrees down off horizontal facing me
                    //         //rod.transform.Rotate(-90,-90,180);// almost perfect but pointing straight up
                    //          //rod.transform.Rotate(-180,-90,180);// crooked
                    //          //rod.transform.Rotate(-45,-45,180);//crooked
                    //          //rod.transform.Rotate(180,180,180);//vertical pointing backwards
                    //          //rod.transform.Rotate(90,-90,180);// points at sky, but also tracks X :S
                    //          //rod.transform.Rotate(0,-90,0);// points to left
                    //          rod.transform.Rotate(90,-90,0);// horizontal points down
                    //          //rod.transform.Rotate(90,-45,0);// 45 degrees off horizontal points down - away from me
                    //          //rod.transform.Rotate(90,-90,45);// 45 degrees off horizontal points down - towards me
                             

                    //    }
           
                }
            }
            


    } // FixedUpdate()

}