using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurboLaserTestBehaviour : MonoBehaviour
{
    GameObject target = null;
    string targetTag = null;


    private AudioSource metalHitSource;
    private AudioClip metalHit;
    private float blasterVolume = 0.1f;

    Vector3 contactPoint;

    public float health = 999999999f;
    public GameObject muzzleFlashEffect;

    private float shootingPauseTimer;

    bool shootingAllowed = false;

    private GameObject turboLasterBlastPrefab;
    private float blasterSpeed = 180.0f * 3.0f;

    private float delayBeforeFirstShot;

    private AudioSource deathStarBlastSource;
    private AudioClip deathStarBlast;
    private float deathStarBlastVolume = 0.8f;

    private bool debug = true;
    private TextMesh debugText;

    private float turboLaserSpeedFactor = 30f;

    float matrix1_z_adjustment = -1.0f;
    float matrix2_x_adjustment = 0.0f;
    float matrix3_x_adjustment = 0.0f;
    int shootTimes = 0;
    int targetAttempt = 0;

    // Start is called before the first frame update
    void Start()
    {
        turboLasterBlastPrefab = PrefabFactory.getPrefab("starDestroyerBlast");

        deathStarBlastSource = GameObject.FindGameObjectWithTag("deathStarBlast_Sound").GetComponent<AudioSource>();
        deathStarBlast = deathStarBlastSource.clip;

        debugText = GameObject.Find("debugText").GetComponent<TextMesh>();

        shootingPauseTimer = GetRandomShootingPauseAmount(); //pause between each shot

        delayBeforeFirstShot = GetRandomStartDelay(); // pause before starting to shoot

        pauseBeforeShootingOnDifferentThread(); // start the pause (before starting to shoot)

    }

    void chooseTarget2()

    {
        if (target != null) return;

        target = GameObject.FindGameObjectWithTag("PlayerShooter");
        targetTag = "PlayerShooter";
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

    // Update is called once per frame
    void FixedUpdate()
    {

        chooseTarget();

        if (shootingAllowed)
        {
            shootIfTime();
        }

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
        return Random.Range(1.0f, 1.5f);
    }
    float GetRandomShootingPauseAmount()
    {
        return Random.Range(1.0f, 1.5f);
    }
    void shoot()
    {
       // MyDebug("targetAttempt,X adj== " + targetAttempt + "," + matrix3_x_adjustment);
        //MyDebug("Z adj == " + matrix1_z_adjustment );
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
            shootTimes = shootTimes + 1;
            //getTheMatrixNumber
            targetAttempt = targetAttempt + 1;




            /*  This works perfectly when:
             *
             * 1)
             *
             *   |--O--|   (Tie Fighter)
             *
             *      II     
             *     [  ]    ( Turbo Laser)
             *
             *  and both have Z pointing straight forward
             *
             * 2)
             *     
             *     [  ]    ( Turbo Laser) (Z pointing backwards)
             *      II         
             *
             *
             *   |--O--|   (Tie Fighter) (Z pointing forward)
             *
             *  
             */

            if (target != null)
            {

                if (targetTag == "tantiveIV")
                {
                    //Vector3 targetAdjustedPosition = new Vector3(target.transform.position.x,target.transform.position.y,target.transform.position.z + 0.05f);
                    Vector3 lookPos = target.transform.position - this.transform.position;
                    //Vector3 lookPos = targetAdjustedPosition - this.transform.position;
                    //MyDebug("target x,y,z="+ target.transform.position.x + "\n," + target.transform.position.y + "\n," + target.transform.position.z + "\n"   );
                    Quaternion lookRot = Quaternion.LookRotation(lookPos, Vector3.up);
                    float eulerY = lookRot.eulerAngles.y;
                    float eulerX = lookRot.eulerAngles.x;
                    float eulerZ = lookRot.eulerAngles.z;
                    Quaternion rotation = Quaternion.Euler(eulerX, eulerY, eulerZ);
                    //this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, turboLaserSpeedFactor * Time.deltaTime);
                    this.transform.rotation = rotation;

                }
                else
                {
                    // MyDebug("target x,y,z="+ target.transform.position.x + "\n," + target.transform.position.y + "\n," + target.transform.position.z + "\n"   );
                    //float z_adjustment = 0.05f; nope
                    //float z_adjustment = 0.1f;
                    //float z_adjustment = -0.35f;// pretty good except for fast vehicles
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
                    //Vector3 lookPos = target.transform.position - this.transform.position;
                    Vector3 lookPos = targetAdjustedPosition - this.transform.position;
                    //MyDebug("target x,y,z="+ target.transform.position.x + "\n," + target.transform.position.y + "\n," + target.transform.position.z + "\n"   );
                    Quaternion lookRot = Quaternion.LookRotation(lookPos, Vector3.up);
                    float eulerY = lookRot.eulerAngles.y;
                    float eulerX = lookRot.eulerAngles.x;
                    float eulerZ = lookRot.eulerAngles.z;
                    Quaternion rotation = Quaternion.Euler(eulerX, eulerY, eulerZ);
                    //this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, turboLaserSpeedFactor * Time.deltaTime);
                    this.transform.rotation = rotation;

                }

            }
            else
            {
                MyDebug("");
                //Quaternion rotation = Quaternion.Euler(0, 0, 0);
                //this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, turboLaserSpeedFactor * Time.deltaTime);
                ////this.transform.rotation = rotation;            
            }

            if (target != null)
            {
                shoot();
                target = null;
                targetTag = null;

            }

            shootingPauseTimer = GetRandomShootingPauseAmount();

        }
    }// end of function
}
