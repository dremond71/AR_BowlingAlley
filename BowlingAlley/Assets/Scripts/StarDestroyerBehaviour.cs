using System.Collections;
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

    private GameObject turboLasterBlastPrefab;
    private float blasterSpeed = 180.0f * 3.0f;
    

    private AudioSource imperialMarchSource;
    private AudioClip imperialMarch;

    private float imperialMarchVolume = 0.7f;


    private AudioSource turboLaserBlastSource;
    private AudioClip turboLaserBlast;
    private float turboLaserBlastVolume = 0.3f;

    private bool debug = false;
    private TextMesh debugText;
    private float turboLaserSpeedFactor = 30f;

    float matrix1_z_adjustment = -1.0f;
    float matrix2_x_adjustment = 0.0f;
    float matrix3_x_adjustment = 0.0f;
    int shootTimes = 0;
    int targetAttempt = 0;

    int spawnPositionIndex = 1;

    //
    // SD_TL_BR
    //
    GameObject target_SD_TL_BR = null;
    string targetTag_SD_TL_BR = null;

    private float shootingPauseTimer_SD_TL_BR;

    bool shootingAllowed_SD_TL_BR = false;

    private float delayBeforeFirstShot_SD_TL_BR;


    //
    // SD_TL_BFC
    //
    GameObject target_SD_TL_BFC = null;
    string targetTag_SD_TL_BFC = null;

    private float shootingPauseTimer_SD_TL_BFC;

    bool shootingAllowed_SD_TL_BFC = false;

    private float delayBeforeFirstShot_SD_TL_BFC;

    void Awake()
    {
        // 8 MB, so start loading early
        imperialMarchSource = GameObject.FindGameObjectWithTag("imperialMarch_Sound").GetComponent<AudioSource>();
        imperialMarch = imperialMarchSource.clip;

    }

    // Start is called before the first frame update
    void Start()
    {
        metalHitSource = GameObject.FindGameObjectWithTag("swMetalHit_Sound").GetComponent<AudioSource>();
        metalHit = metalHitSource.clip;

        turboLasterBlastPrefab = PrefabFactory.getPrefab("starDestroyerBlast");

        turboLaserBlastSource = GameObject.FindGameObjectWithTag("starDestroyerBlast_Sound").GetComponent<AudioSource>();
        turboLaserBlast = turboLaserBlastSource.clip;

        shootingPauseTimer_SD_TL_BR  = GetRandomShootingPauseAmount(); //pause between each shot
        shootingPauseTimer_SD_TL_BFC = GetRandomShootingPauseAmount(); //pause between each shot

        delayBeforeFirstShot_SD_TL_BR  = GetRandomStartDelay(); // pause before starting to shoot
        delayBeforeFirstShot_SD_TL_BFC = GetRandomStartDelay(); // pause before starting to shoot

        pauseBeforeShootingOnDifferentThread_SD_TL_BR(); // start the pause (before starting to shoot)
        pauseBeforeShootingOnDifferentThread_SD_TL_BFC(); // start the pause (before starting to shoot)

        debugText = GameObject.Find("debugText").GetComponent<TextMesh>();

        PlayImperialMarchSound_Immediately();
    }

    public void setSpawnPositionIndex(int theIndex){
        spawnPositionIndex = theIndex;
    }

    void MyDebug(string someText)
    {

        if (debugText != null & debug)
        {
            debugText.text = someText;
        }

    }

    void PlayStarDestroyerBlastSound_Immediately()
    {
        turboLaserBlastSource.PlayOneShot(turboLaserBlast, turboLaserBlastVolume);

    }

    void PlayImperialMarchSound_Immediately()
    {
        imperialMarchSource.PlayOneShot(imperialMarch, imperialMarchVolume);

    }

    void pauseBeforeShootingOnDifferentThread_SD_TL_BR()
    {
        StartCoroutine(pauseBeforeStartingToShoot_SD_TL_BR());
    }

    IEnumerator pauseBeforeStartingToShoot_SD_TL_BR()
    {
        yield return new WaitForSeconds(delayBeforeFirstShot_SD_TL_BR);
        shootingAllowed_SD_TL_BR = true;
    }

    void pauseBeforeShootingOnDifferentThread_SD_TL_BFC()
    {
        StartCoroutine(pauseBeforeStartingToShoot_SD_TL_BFC());
    }

    IEnumerator pauseBeforeStartingToShoot_SD_TL_BFC()
    {
        yield return new WaitForSeconds(delayBeforeFirstShot_SD_TL_BFC);
        shootingAllowed_SD_TL_BFC = true;
    } 

    float GetRandomStartDelay()
    {
        return Random.Range(1.0f, 1.5f);
    }
    float GetRandomShootingPauseAmount()
    {
        return Random.Range(1.0f, 1.5f);
    }


    float GetRandomZAdjustment()
    {
        return Random.Range(-0.32f, -0.34f);
    }


    void chooseTarget_SD_TL_BR() {

        if (target_SD_TL_BR != null) return;

        GameObject tantive = GameObject.FindGameObjectWithTag("tantiveIV");
        GameObject falcon = GameObject.FindGameObjectWithTag("falcon");

        GameObject[] tantives = new GameObject[] { };
        GameObject[] falcons = new GameObject[] { };

        if (tantive != null)
        {
            tantives = new GameObject[] { tantive };
            tantives = LevelManager.filterGameObjectsInFrontOfPlayer(tantives);
        }

        if (falcons != null)
        {
            falcons = new GameObject[] { falcon };
            falcons = LevelManager.filterGameObjectsInFrontOfPlayer(falcons);
        }

        GameObject[] xwings = LevelManager.filterGameObjectsInFrontOfPlayer(GameObject.FindGameObjectsWithTag("targetXWing"));
        GameObject[] meteorites = LevelManager.filterGameObjectsInFrontOfPlayer(GameObject.FindGameObjectsWithTag("targetMeteorite"));
        GameObject[] awings = LevelManager.filterGameObjectsInFrontOfPlayer(GameObject.FindGameObjectsWithTag("targetAWing"));

        
        List<GameObject> list    = new List<GameObject>();
        List<string>     tagList = new List<string>();

        if ( (spawnPositionIndex == 1) || (spawnPositionIndex == 2) ){

            if (xwings.Length > 0)
            {
                for (int i = 0; i < xwings.Length; i++){
                    list.Add( xwings[i] );
                    tagList.Add("targetXWing");    
                }
                
                
            }
            
            if (awings.Length > 0)
            {
                for (int i = 0; i < awings.Length; i++){
                    list.Add( awings[i] );
                    tagList.Add("targetAWing");    
                }                
            }

            if (meteorites.Length > 0)
            {
                for (int i = 0; i < meteorites.Length; i++){
                    list.Add( meteorites[i] );
                    tagList.Add("targetMeteorite");    
                }                
            }


            if (falcons != null && falcons.Length > 0)
            {
                list.Add( falcons[0] );
                tagList.Add("falcon");    
            }


            if (tantives != null && tantives.Length > 0)
            {
                list.Add( tantives[0] );
                tagList.Add("tantiveIV");
            }
            
            GameObject[] targets_SD_TL_BR    = list.ToArray();
            string[] targetTags_SD_TL_BR     = tagList.ToArray();

            if (targets_SD_TL_BR != null && targets_SD_TL_BR.Length>0){
                int lastIndex = targets_SD_TL_BR.Length - 1;
                target_SD_TL_BR    = targets_SD_TL_BR[lastIndex];
                targetTag_SD_TL_BR = targetTags_SD_TL_BR[lastIndex];
            }

        } // if - spawnPositionIndex == 1 || == 2


    }


    void chooseTarget_SD_TL_BFC() {

        if (target_SD_TL_BFC != null) return;

        GameObject tantive = GameObject.FindGameObjectWithTag("tantiveIV");
        GameObject falcon = GameObject.FindGameObjectWithTag("falcon");

        GameObject[] tantives = new GameObject[] { };
        GameObject[] falcons = new GameObject[] { };

        if (tantive != null)
        {
            tantives = new GameObject[] { tantive };
            tantives = LevelManager.filterGameObjectsInFrontOfPlayer(tantives);
        }

        if (falcons != null)
        {
            falcons = new GameObject[] { falcon };
            falcons = LevelManager.filterGameObjectsInFrontOfPlayer(falcons);
        }

        GameObject[] xwings = LevelManager.filterGameObjectsInFrontOfPlayer(GameObject.FindGameObjectsWithTag("targetXWing"));
        GameObject[] meteorites = LevelManager.filterGameObjectsInFrontOfPlayer(GameObject.FindGameObjectsWithTag("targetMeteorite"));
        GameObject[] awings = LevelManager.filterGameObjectsInFrontOfPlayer(GameObject.FindGameObjectsWithTag("targetAWing"));

        
        List<GameObject> list    = new List<GameObject>();
        List<string>     tagList = new List<string>();

        if ( (spawnPositionIndex == 1) || (spawnPositionIndex == 2) ){

            if (xwings.Length > 0)
            {
                for (int i = 0; i < xwings.Length; i++){
                    list.Add( xwings[i] );
                    tagList.Add("targetXWing");    
                }
                
                
            }
            
            if (awings.Length > 0)
            {
                for (int i = 0; i < awings.Length; i++){
                    list.Add( awings[i] );
                    tagList.Add("targetAWing");    
                }                
            }

            if (meteorites.Length > 0)
            {
                for (int i = 0; i < meteorites.Length; i++){
                    list.Add( meteorites[i] );
                    tagList.Add("targetMeteorite");    
                }                
            }


            if (falcons != null && falcons.Length > 0)
            {
                list.Add( falcons[0] );
                tagList.Add("falcon");    
            }


            if (tantives != null && tantives.Length > 0)
            {
                list.Add( tantives[0] );
                tagList.Add("tantiveIV");
            }
            
            GameObject[] targets_SD_TL_BFC    = list.ToArray();
            string[] targetTags_SD_TL_BFC     = tagList.ToArray();

            if (targets_SD_TL_BFC != null && targets_SD_TL_BFC.Length>0){
                int firstIndex = 0;
                target_SD_TL_BFC    = targets_SD_TL_BFC[firstIndex];
                targetTag_SD_TL_BFC = targetTags_SD_TL_BFC[firstIndex];
            }

        } // if - spawnPositionIndex == 1 || == 2


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

        if (spawnPositionIndex ==1){
            // this object becomes irrelevant 
            // IF it has flown past the death star; having been spawned from starDestroyerSpawnPoint1
            // some other condition if spawned from somewhere else

            GameObject deathStar = GameObject.FindGameObjectWithTag("deathStar");
            float deathStarZ = deathStar.transform.position.z;

            float myZ = transform.position.z;

            //MyDebug("deathStar.z, starDestroyer.z : " + deathStarZ + " , " + myZ);
            if (myZ > (deathStarZ + 7.0f))
            {
                LevelManager.starDestroyerAttackFinished(); // tells LevelManager that the cavalry attack is finished
                //MyDebug("About to destroy Star Destroyer!");
                destroySelf();
            }
        }
        else if (spawnPositionIndex == 2) {
            // this object becomes irrelevant if it has flown past the shooter 

            GameObject shooter = GameObject.FindGameObjectWithTag("PlayerShooter");
            float shooterZ = shooter.transform.position.z;

            float myZ = transform.position.z;

            if (myZ < (shooterZ - 8f))
            {
                LevelManager.starDestroyerAttackFinished(); // tells LevelManager that the cavalry attack is finished
                destroySelf();
            }
        }


    }
    void destroySelf()
    {
        Destroy(gameObject);
    }

    void shoot_SD_TL_BR()
    {
        spawnNewBlasterBolts_SD_TL_BR();
        PlayStarDestroyerBlastSound_Immediately();
    }

    void shoot_SD_TL_BFC()
    {
        spawnNewBlasterBolts_SD_TL_BFC();
        PlayStarDestroyerBlastSound_Immediately();
    }

    void spawnNewBlasterBolts_SD_TL_BR()
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

            // MyDebug("sd spawn bold, error: " + e.ToString());
        }

    }

   void spawnNewBlasterBolts_SD_TL_BFC()
    {

        try
        {
            // bolt 1
            GameObject origin1 = GameObject.FindGameObjectWithTag("SD_TL_BFC_BlasterPosition1");
            float x = origin1.transform.position.x;
            float y = origin1.transform.position.y;
            float z = origin1.transform.position.z;

            GameObject go = (GameObject)Instantiate(turboLasterBlastPrefab, new Vector3(x, y, z), origin1.transform.rotation);
            GameObject bolt1 = PrefabFactory.GetChildWithName(go, "sdTurboLaserBlast");

            // bolt 2
            GameObject origin2 = GameObject.FindGameObjectWithTag("SD_TL_BFC_BlasterPosition2");
            x = origin2.transform.position.x;
            y = origin2.transform.position.y;
            z = origin2.transform.position.z;

            go = (GameObject)Instantiate(turboLasterBlastPrefab, new Vector3(x, y, z), origin2.transform.rotation);
            GameObject bolt2 = PrefabFactory.GetChildWithName(go, "sdTurboLaserBlast");


            // bolt 3
            GameObject origin3 = GameObject.FindGameObjectWithTag("SD_TL_BFC_BlasterPosition3");
            x = origin3.transform.position.x;
            y = origin3.transform.position.y;
            z = origin3.transform.position.z;

            go = (GameObject)Instantiate(turboLasterBlastPrefab, new Vector3(x, y, z), origin3.transform.rotation);
            GameObject bolt3 = PrefabFactory.GetChildWithName(go, "sdTurboLaserBlast");

            // bolt 4
            GameObject origin4 = GameObject.FindGameObjectWithTag("SD_TL_BFC_BlasterPosition4");
            x = origin4.transform.position.x;
            y = origin4.transform.position.y;
            z = origin4.transform.position.z;

            go = (GameObject)Instantiate(turboLasterBlastPrefab, new Vector3(x, y, z), origin4.transform.rotation);
            GameObject bolt4 = PrefabFactory.GetChildWithName(go, "sdTurboLaserBlast");

            bolt1.GetComponent<Rigidbody>().velocity = blasterSpeed * origin1.transform.forward * Time.deltaTime;
            bolt2.GetComponent<Rigidbody>().velocity = blasterSpeed * origin2.transform.forward * Time.deltaTime;
            bolt3.GetComponent<Rigidbody>().velocity = blasterSpeed * origin3.transform.forward * Time.deltaTime;
            bolt4.GetComponent<Rigidbody>().velocity = blasterSpeed * origin4.transform.forward * Time.deltaTime;
        }
        catch (System.Exception e)
        {

            // MyDebug("sd spawn bold, error: " + e.ToString());
        }

    }

    float GetRandomZAdjustment_Matrix1()
    {
        float someValue = 0.0f;

        bool evenValue = ((targetAttempt % 2) == 0);
        if (evenValue)
        {
            someValue = -0.30f;// adj for slower vehicles
        }
        else
        {
            someValue = -0.45f;// adj for non-slow vehicles
        }
        return someValue;

    }

    float GetRandomXAdjustment_Matrix2()
    {
        float someValue = 0.0f;

        bool evenValue = ((targetAttempt % 2) == 0);
        if (evenValue)
        {
            someValue = 0.05f;// adj for slower vehicles
        }
        else
        {
            someValue = 0.10f;// adj for non-slow vehicles
        }
        return someValue;

    }

    float GetRandomXAdjustment_Matrix3()
    {
        float someValue = 0.0f;

        bool evenValue = ((targetAttempt % 2) == 0);
        if (evenValue)
        {
            someValue = -0.20f;// adj for slower vehicles
        }
        else
        {
            someValue = -0.25f;// adj for non-slow vehicles
        }
        return someValue;

    }

    void shootIfTime_SD_TL_BR()
    {

        shootingPauseTimer_SD_TL_BR -= Time.deltaTime;
        if (shootingPauseTimer_SD_TL_BR <= 0f)
        {
            shootTimes = shootTimes + 1;

            targetAttempt = targetAttempt + 1;

            // SD (Star Destroyer) TL (Turbo Laser) BR (Bottom Right) Swivel
            GameObject swivel = GameObject.FindGameObjectWithTag("SD_TL_BR_Swivel");

            if (swivel != null)
            {

                if (target_SD_TL_BR != null)
                {

                    if (targetTag_SD_TL_BR == "tantiveIV")
                    {

                        Vector3 lookPos = target_SD_TL_BR.transform.position - swivel.transform.position;

                        Quaternion lookRot = Quaternion.LookRotation(lookPos, Vector3.up);
                        float eulerY = lookRot.eulerAngles.y;
                        float eulerX = lookRot.eulerAngles.x;
                        float eulerZ = lookRot.eulerAngles.z;
                        Quaternion rotation = Quaternion.Euler(eulerX, eulerY, eulerZ);

                        swivel.transform.rotation = rotation;

                    }
                    else
                    {

                        Vector3 targetAdjustedPosition;

                        if (LevelManager.getTheMatrixNumber() == 1)
                        {
                            matrix1_z_adjustment = GetRandomZAdjustment_Matrix1();
                            targetAdjustedPosition = new Vector3(target_SD_TL_BR.transform.position.x, target_SD_TL_BR.transform.position.y, target_SD_TL_BR.transform.position.z + matrix1_z_adjustment);
                        }
                        else if (LevelManager.getTheMatrixNumber() == 2)
                        {
                            matrix1_z_adjustment = GetRandomZAdjustment_Matrix1();
                            matrix2_x_adjustment = GetRandomXAdjustment_Matrix2();
                            targetAdjustedPosition = new Vector3(target_SD_TL_BR.transform.position.x + matrix2_x_adjustment, target_SD_TL_BR.transform.position.y, target_SD_TL_BR.transform.position.z + matrix1_z_adjustment);
                        }
                        else if (LevelManager.getTheMatrixNumber() == 3)
                        {
                            matrix1_z_adjustment = GetRandomZAdjustment_Matrix1();
                            matrix3_x_adjustment = GetRandomXAdjustment_Matrix3();
                            targetAdjustedPosition = new Vector3(target_SD_TL_BR.transform.position.x + matrix3_x_adjustment, target_SD_TL_BR.transform.position.y, target_SD_TL_BR.transform.position.z + matrix1_z_adjustment);
                        }
                        else
                        {
                            targetAdjustedPosition = new Vector3(target_SD_TL_BR.transform.position.x, target_SD_TL_BR.transform.position.y, target_SD_TL_BR.transform.position.z);
                        }

                        Vector3 lookPos = targetAdjustedPosition - swivel.transform.position;

                        Quaternion lookRot = Quaternion.LookRotation(lookPos, Vector3.up);
                        float eulerY = lookRot.eulerAngles.y;
                        float eulerX = lookRot.eulerAngles.x;
                        float eulerZ = lookRot.eulerAngles.z;
                        Quaternion rotation = Quaternion.Euler(eulerX, eulerY, eulerZ);

                        swivel.transform.rotation = rotation;

                    }

                }
                else
                {
                    // MyDebug("");
                }

                if (target_SD_TL_BR != null)
                {
                    shoot_SD_TL_BR();
                    target_SD_TL_BR = null;
                    targetTag_SD_TL_BR = null;

                }

            }// swivel != null


            shootingPauseTimer_SD_TL_BR = GetRandomShootingPauseAmount();

        }
    }// end of function

    void shootIfTime_SD_TL_BFC()
    {

        shootingPauseTimer_SD_TL_BFC -= Time.deltaTime;
        if (shootingPauseTimer_SD_TL_BFC <= 0f)
        {
            shootTimes = shootTimes + 1;

            targetAttempt = targetAttempt + 1;

            // SD (Star Destroyer) TL (Turbo Laser) BFC (Bottom Front Center) Swivel
            GameObject swivel = GameObject.FindGameObjectWithTag("SD_TL_BFC_Swivel");

            if (swivel != null)
            {

                if (target_SD_TL_BFC != null)
                {

                    if (targetTag_SD_TL_BFC == "tantiveIV")
                    {

                        Vector3 lookPos = target_SD_TL_BFC.transform.position - swivel.transform.position;

                        Quaternion lookRot = Quaternion.LookRotation(lookPos, Vector3.up);
                        float eulerY = lookRot.eulerAngles.y;
                        float eulerX = lookRot.eulerAngles.x;
                        float eulerZ = lookRot.eulerAngles.z;
                        Quaternion rotation = Quaternion.Euler(eulerX, eulerY, eulerZ);

                        swivel.transform.rotation = rotation;

                    }
                    else
                    {

                        Vector3 targetAdjustedPosition;

                        if (LevelManager.getTheMatrixNumber() == 1)
                        {
                            matrix1_z_adjustment = GetRandomZAdjustment_Matrix1();
                            targetAdjustedPosition = new Vector3(target_SD_TL_BFC.transform.position.x, target_SD_TL_BFC.transform.position.y, target_SD_TL_BFC.transform.position.z + matrix1_z_adjustment);
                        }
                        else if (LevelManager.getTheMatrixNumber() == 2)
                        {
                            matrix1_z_adjustment = GetRandomZAdjustment_Matrix1();
                            matrix2_x_adjustment = GetRandomXAdjustment_Matrix2();
                            targetAdjustedPosition = new Vector3(target_SD_TL_BFC.transform.position.x + matrix2_x_adjustment, target_SD_TL_BFC.transform.position.y, target_SD_TL_BFC.transform.position.z + matrix1_z_adjustment);
                        }
                        else if (LevelManager.getTheMatrixNumber() == 3)
                        {
                            matrix1_z_adjustment = GetRandomZAdjustment_Matrix1();
                            matrix3_x_adjustment = GetRandomXAdjustment_Matrix3();
                            targetAdjustedPosition = new Vector3(target_SD_TL_BFC.transform.position.x + matrix3_x_adjustment, target_SD_TL_BFC.transform.position.y, target_SD_TL_BFC.transform.position.z + matrix1_z_adjustment);
                        }
                        else
                        {
                            targetAdjustedPosition = new Vector3(target_SD_TL_BFC.transform.position.x, target_SD_TL_BFC.transform.position.y, target_SD_TL_BFC.transform.position.z);
                        }

                        Vector3 lookPos = targetAdjustedPosition - swivel.transform.position;

                        Quaternion lookRot = Quaternion.LookRotation(lookPos, Vector3.up);
                        float eulerY = lookRot.eulerAngles.y;
                        float eulerX = lookRot.eulerAngles.x;
                        float eulerZ = lookRot.eulerAngles.z;
                        Quaternion rotation = Quaternion.Euler(eulerX, eulerY, eulerZ);

                        swivel.transform.rotation = rotation;

                    }

                }
                else
                {
                    // MyDebug("");
                }

                if (target_SD_TL_BFC != null)
                {
                    shoot_SD_TL_BFC();
                    target_SD_TL_BFC = null;
                    targetTag_SD_TL_BFC = null;

                }

            }// swivel != null


            shootingPauseTimer_SD_TL_BFC = GetRandomShootingPauseAmount();

        }
    }// end of function

    void FixedUpdate()
    {

        destroyIfIrrelevantNow();

        chooseTarget_SD_TL_BR();

        if (shootingAllowed_SD_TL_BR)
        {
            shootIfTime_SD_TL_BR();
        }

        chooseTarget_SD_TL_BFC();

        if (shootingAllowed_SD_TL_BFC)
        {
            shootIfTime_SD_TL_BFC();
        }

    }

}
