using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slave1Behaviour : MonoBehaviour
{
    private AudioSource metalHitSource;
    private AudioClip metalHit;
    private float blasterVolume = 0.1f;

    Vector3 contactPoint;

    public float health = 999999999f;
    public GameObject muzzleFlashEffect;

    private bool debug = true;
    private TextMesh debugText;


    private AudioSource roarSource;
    private AudioClip roar;

    public float blastVolume = 2.0f;

    public float roarVolume = 2.0f;

    bool roarSoundIsPlaying = false;

    private AudioSource blasterSource;
    private AudioClip blaster;

    private GameObject turboLasterBlastPrefab;

    private float blasterSpeed = 180.0f * 3.0f;


    GameObject blasterTarget = null;
    string blasterTargetTag = null;


    private float shootingPauseTimer;

    bool shootingAllowed = false;

    private float delayBeforeFirstShot;

    void chooseTarget()
    {

       if (blasterTarget != null) return;


        GameObject tantive = GameObject.FindGameObjectWithTag("tantiveIV");
        tantive = null;// for now
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


        List<GameObject> list = new List<GameObject>();
        List<string> tagList = new List<string>();



        if (xwings.Length > 0)
        {
            for (int i = 0; i < xwings.Length; i++)
            {
                list.Add(xwings[i]);
                tagList.Add("targetXWing");
            }


        }

        if (awings.Length > 0)
        {
            for (int i = 0; i < awings.Length; i++)
            {
                list.Add(awings[i]);
                tagList.Add("targetAWing");
            }
        }

        if (meteorites.Length > 0)
        {
            for (int i = 0; i < meteorites.Length; i++)
            {
                list.Add(meteorites[i]);
                tagList.Add("targetMeteorite");
            }
        }


        if (falcons != null && falcons.Length > 0)
        {
            list.Add(falcons[0]);
            tagList.Add("falcon");
        }


        if (tantives != null && tantives.Length > 0)
        {
            list.Add(tantives[0]);
            tagList.Add("tantiveIV");
        }

        GameObject[] targets = list.ToArray();
        string[] targetTags = tagList.ToArray();

        if (targets != null && targets.Length > 0)
        {
            int lastIndex = targets.Length - 1;
            blasterTarget = targets[lastIndex];
            blasterTargetTag = targetTags[lastIndex];
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
            LevelManager.bobaFettAttackFinished();
            destroySelf();
        }

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
        return Random.Range(0.5f, 0.8f);
    }

    void Awake()
    {
        roarSource = GameObject.FindGameObjectWithTag("slave1Roar_Sound").GetComponent<AudioSource>();
        roar = roarSource.clip;

        blasterSource = GameObject.FindGameObjectWithTag("slave1Guns_Sound").GetComponent<AudioSource>();
        blaster = blasterSource.clip;

        turboLasterBlastPrefab = PrefabFactory.getPrefab("slave1Blast");

    }

    // Start is called before the first frame update
    void Start()
    {
        debugText = GameObject.Find("debugText").GetComponent<TextMesh>();
        shootingPauseTimer = GetRandomShootingPauseAmount(); //pause between each shot
        delayBeforeFirstShot = GetRandomStartDelay(); // pause before starting to shoot

        pauseBeforeShootingOnDifferentThread();
    }

    void MyDebug(string someText)
    {

        if (debugText != null & debug)
        {
            debugText.text = someText;
        }

    }


    IEnumerator shoot()
    {
        PlayBlasterSound_Immediately();
        float shotDelay = 0.1f;

        spawnNewBlasterBolt();
        yield return new WaitForSeconds(shotDelay);
        spawnNewBlasterBolt();
        yield return new WaitForSeconds(shotDelay);
        spawnNewBlasterBolt();
        yield return new WaitForSeconds(shotDelay);

        blasterTarget = null;
        blasterTargetTag = null;

    }



    float GetRandomShootingPauseAmount()
    {


        return Random.Range(0.4f, 0.5f);

    }


    public void shootBlastersAtTarget()
    {
        StartCoroutine(shoot());
    }

    void shootIfTime()
    {

        shootingPauseTimer -= Time.deltaTime;
        if (shootingPauseTimer <= 0f)
        {


            if (blasterTarget != null)
            {

                // get blaster swivel
                GameObject swivel = GameObject.FindGameObjectWithTag("slave1GunsSwivel");

                Vector3 lookPos = blasterTarget.transform.position - swivel.transform.position;

                Quaternion lookRot = Quaternion.LookRotation(lookPos, Vector3.up);
                float eulerY = lookRot.eulerAngles.y;
                float eulerX = lookRot.eulerAngles.x;
                float eulerZ = lookRot.eulerAngles.z;
                Quaternion rotation = Quaternion.Euler(eulerX, eulerY, eulerZ);

                swivel.transform.rotation = rotation;


                shootBlastersAtTarget();
  



            }// swivel != null


            shootingPauseTimer = GetRandomShootingPauseAmount();

        }
    }// end of function


    void spawnNewBlasterBolt()
    {

        try
        {
            // bolt 1
            GameObject blastOrigin1 = GameObject.FindGameObjectWithTag("slave1BlasterPosition1");
            float x = blastOrigin1.transform.position.x;
            float y = blastOrigin1.transform.position.y;
            float z = blastOrigin1.transform.position.z;

            GameObject go = (GameObject)Instantiate(turboLasterBlastPrefab, new Vector3(x, y, z), blastOrigin1.transform.rotation);
            GameObject bolt1 = PrefabFactory.GetChildWithName(go, "s1Blast");
            bolt1.GetComponent<Rigidbody>().velocity = blasterSpeed * blastOrigin1.transform.forward * Time.deltaTime;

            // bolt 2
            GameObject blastOrigin2 = GameObject.FindGameObjectWithTag("slave1BlasterPosition2");
            x = blastOrigin2.transform.position.x;
            y = blastOrigin2.transform.position.y;
            z = blastOrigin2.transform.position.z;

            GameObject go2 = (GameObject)Instantiate(turboLasterBlastPrefab, new Vector3(x, y, z), blastOrigin2.transform.rotation);
            GameObject bolt2 = PrefabFactory.GetChildWithName(go2, "s1Blast");
            bolt2.GetComponent<Rigidbody>().velocity = blasterSpeed * blastOrigin2.transform.forward * Time.deltaTime;

            // bolt 3
            GameObject blastOrigin3 = GameObject.FindGameObjectWithTag("slave1BlasterPosition3");
            x = blastOrigin3.transform.position.x;
            y = blastOrigin3.transform.position.y;
            z = blastOrigin3.transform.position.z;

            GameObject go3 = (GameObject)Instantiate(turboLasterBlastPrefab, new Vector3(x, y, z), blastOrigin3.transform.rotation);
            GameObject bolt3 = PrefabFactory.GetChildWithName(go3, "s1Blast");
            bolt3.GetComponent<Rigidbody>().velocity = blasterSpeed * blastOrigin3.transform.forward * Time.deltaTime;

            // bolt 4
            GameObject blastOrigin4 = GameObject.FindGameObjectWithTag("slave1BlasterPosition4");
            x = blastOrigin4.transform.position.x;
            y = blastOrigin4.transform.position.y;
            z = blastOrigin4.transform.position.z;

            GameObject go4 = (GameObject)Instantiate(turboLasterBlastPrefab, new Vector3(x, y, z), blastOrigin4.transform.rotation);
            GameObject bolt4 = PrefabFactory.GetChildWithName(go4, "s1Blast");
            bolt4.GetComponent<Rigidbody>().velocity = blasterSpeed * blastOrigin4.transform.forward * Time.deltaTime;

        }
        catch (System.Exception e)
        {
            MyDebug(e.ToString());
        }

    }

    void PlayBlasterSound_Immediately()
    {
        // we want a seperate object for each fire; so we can handle multi blasts in quick succession

        // Method I used to achieve multiple blasts that don't interrupt each other:
        // https://docs.unity3d.com/ScriptReference/AudioSource.PlayOneShot.html
        blasterSource.PlayOneShot(blaster, blastVolume);

    }

    void handleHit()
    {

        handleDamage();



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


    void handleDamage()
    {

        GameObject damageObject = Instantiate(muzzleFlashEffect, contactPoint, muzzleFlashEffect.transform.rotation);
        damageObject.transform.Rotate(0f, 45f, 0f);
        damageObject.transform.SetParent(this.transform);


    }



    void destroySelf()
    {
        Destroy(gameObject);
    }

    void FixedUpdate()
    {

        playRoaringSoundIfItIsTime();

        chooseTarget();

        if (shootingAllowed)
        {
            shootIfTime();
        }

        destroyIfIrrelevantNow();


    }

    IEnumerator PlayRoarSound()
    {
        //https://answers.unity.com/questions/904981/how-to-play-an-audio-file-after-another-finishes.html
        roarSoundIsPlaying = true;
        roarSource.PlayOneShot(roar, roarVolume);
        yield return new WaitForSeconds(roar.length);
        roarSoundIsPlaying = false;

    }

    void playRoarOnDifferentThread()
    {

        StartCoroutine(PlayRoarSound());

    }

    void playRoaringSoundIfItIsTime()
    {

        if (!roarSoundIsPlaying)
            playRoarOnDifferentThread();

    }

}
