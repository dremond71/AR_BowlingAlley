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

    GameObject target;
    string targetTag = null;

    private float shootingPauseTimer;

    bool shootingAllowed = false;

    private GameObject turboLasterBlastPrefab;
    private float blasterSpeed = 180.0f * 3.0f;
    private float delayBeforeFirstShot;

    private AudioSource imperialMarchSource;
    private AudioClip imperialMarch;

    private float imperialMarchVolume = 1.6f;


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

        shootingPauseTimer = GetRandomShootingPauseAmount(); //pause between each shot

        delayBeforeFirstShot = GetRandomStartDelay(); // pause before starting to shoot

        pauseBeforeShootingOnDifferentThread(); // start the pause (before starting to shoot)
        debugText = GameObject.Find("debugText").GetComponent<TextMesh>();

        PlayImperialMarchSound_Immediately();
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

    void chooseTarget2()
    {
        target = GameObject.FindGameObjectWithTag("PlayerShooter");
    }

    void chooseTarget()
    {

        if (target != null) return;

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
            if (falcons != null && falcons.Length > 0)
            {
                target = falcons[0];
                targetTag = "falcon";
            }
        }

        if (target == null)
        {
            if (tantives != null && tantives.Length > 0)
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

        MyDebug("deathStar.z, starDestroyer.z : " + deathStarZ + " , " + myZ);
        if (myZ > (deathStarZ + 7.0f))
        {
            LevelManager.starDestroyerAttackFinished(); // tells LevelManager that the cavalry attack is finished
            MyDebug("About to destroy Star Destroyer!");
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
        PlayStarDestroyerBlastSound_Immediately();
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

    void shootIfTime()
    {

        shootingPauseTimer -= Time.deltaTime;
        if (shootingPauseTimer <= 0f)
        {
            shootTimes = shootTimes + 1;

            targetAttempt = targetAttempt + 1;

            // SD (Star Destroyer) TL (Turbo Laser) BR (Bottom Right) Swivel
            GameObject swivel = GameObject.FindGameObjectWithTag("SD_TL_BR_Swivel");
            if (swivel != null)
            {

                if (target != null)
                {

                    if (targetTag == "tantiveIV")
                    {

                        Vector3 lookPos = target.transform.position - swivel.transform.position;

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
                            targetAdjustedPosition = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z + matrix1_z_adjustment);
                        }
                        else if (LevelManager.getTheMatrixNumber() == 2)
                        {
                            matrix1_z_adjustment = GetRandomZAdjustment_Matrix1();
                            matrix2_x_adjustment = GetRandomXAdjustment_Matrix2();
                            targetAdjustedPosition = new Vector3(target.transform.position.x + matrix2_x_adjustment, target.transform.position.y, target.transform.position.z + matrix1_z_adjustment);
                        }
                        else if (LevelManager.getTheMatrixNumber() == 3)
                        {
                            matrix1_z_adjustment = GetRandomZAdjustment_Matrix1();
                            matrix3_x_adjustment = GetRandomXAdjustment_Matrix3();
                            targetAdjustedPosition = new Vector3(target.transform.position.x + matrix3_x_adjustment, target.transform.position.y, target.transform.position.z + matrix1_z_adjustment);
                        }
                        else
                        {
                            targetAdjustedPosition = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);
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

                if (target != null)
                {
                    shoot();
                    target = null;
                    targetTag = null;

                }

            }// swivel != null


            shootingPauseTimer = GetRandomShootingPauseAmount();

        }
    }// end of function

    void FixedUpdate()
    {

        destroyIfIrrelevantNow();

        chooseTarget();

        if (shootingAllowed)
        {
            shootIfTime();
        }

    }


}
