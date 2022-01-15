using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImperialViperDroid : MonoBehaviour
{

    private AudioSource hummingSoundSource;
    private AudioClip hummingSound;
    private float hummingVolume = 1.0f;



    private AudioSource talkingSoundSource;
    private AudioClip talkingSound;
    private float talkingVolume = 1.0f;
    bool talkingSoundIsPlaying = false;

    private AudioSource blasterSource;
    private AudioClip blaster;

    private float blasterVolume = 1.0f;

    private float shootingPauseTimer;

    private TextMesh debugText;
    private bool debug = false;

    private GameObject blastOrigin;
    private GameObject viperBlastPrefab;
    private float blasterSpeed = 540.0f; // 540.0f
    private float delayBeforeFirstShot;

    bool shootingAllowed = false;



    GameObject blasterTarget = null;
    string blasterTargetTag = null;


    void setUpBlastOrigin()
    {
        GameObject theBlaster = PrefabFactory.GetChildWithName(gameObject, "theBlaster");
        GameObject blasterBasePart2 = PrefabFactory.GetChildWithName(theBlaster, "blasterBasePart2");
        GameObject blasterBasePart3 = PrefabFactory.GetChildWithName(blasterBasePart2, "blasterBasePart3");
        blastOrigin = PrefabFactory.GetChildWithName(blasterBasePart3, "viperBlasterPosition");

    }
    IEnumerator pauseBeforeStartingToShoot()
    {
        yield return new WaitForSeconds(delayBeforeFirstShot);
        shootingAllowed = true;
    }

    void spawnNewBlasterBolt()
    {

        float x = blastOrigin.transform.position.x;
        float y = blastOrigin.transform.position.y;
        float z = blastOrigin.transform.position.z;

        GameObject go = (GameObject)Instantiate(viperBlastPrefab, new Vector3(x, y, z), blastOrigin.transform.rotation);
        GameObject bolt1 = PrefabFactory.GetChildWithName(go, "viperBlasterBolt1");

        bolt1.GetComponent<Rigidbody>().velocity = blasterSpeed * blastOrigin.transform.forward * Time.deltaTime;


    }

    float GetRandomShootingPauseAmount()
    {

        return Random.Range(0.4f, 0.5f);

    }

    
    void shootIfTime()
    {

        shootingPauseTimer -= Time.deltaTime;
        if (shootingPauseTimer <= 0f)
        {

            chooseTarget();
            if (blasterTarget != null)
            {

                // get blaster swivel
                GameObject theBlaster = PrefabFactory.GetChildWithName(gameObject, "theBlaster");
                GameObject swivel = PrefabFactory.GetChildWithName(theBlaster, "blasterBasePart2");

                Vector3 lookPos = blasterTarget.transform.position - swivel.transform.position;

                Quaternion lookRot = Quaternion.LookRotation(lookPos, Vector3.up);
                float eulerY = lookRot.eulerAngles.y;
                float eulerX = lookRot.eulerAngles.x;
                float eulerZ = lookRot.eulerAngles.z;
                Quaternion rotation = Quaternion.Euler(eulerX, eulerY, eulerZ);

                swivel.transform.rotation = rotation;

                shoot();

                

            }


            shootingPauseTimer = GetRandomShootingPauseAmount();

        }
    }// end of function


 


    void shoot()
    {
        spawnNewBlasterBolt();
        PlayBlasterSound_Immediately();

    }

    void PlayBlasterSound_Immediately()
    {
        // we want a seperate object for each fire; so we can handle multi blasts in quick succession

        // Method I used to achieve multiple blasts that don't interrupt each other:
        // https://docs.unity3d.com/ScriptReference/AudioSource.PlayOneShot.html
        blasterSource.PlayOneShot(blaster, blasterVolume);


    }

    // Start is called before the first frame update
    void Start()
    {

        try
        {
            debugText = GameObject.Find("debugText").GetComponent<TextMesh>();

            hummingSoundSource = GameObject.FindGameObjectWithTag("viperDroidHum_Sound").GetComponent<AudioSource>();
            hummingSound = hummingSoundSource.clip;

            talkingSoundSource = GameObject.FindGameObjectWithTag("viperDroidTalk_Sound").GetComponent<AudioSource>();
            talkingSound = talkingSoundSource.clip;

            blasterSource = GameObject.FindGameObjectWithTag("viperDroidBlast_Sound").GetComponent<AudioSource>();
            blaster = blasterSource.clip;

            viperBlastPrefab = PrefabFactory.getPrefab("viperBlaster");

            setUpBlastOrigin();

            PlayHummingSound_Immediately();

            delayBeforeFirstShot = GetRandomStartDelay(); // pause before starting to shoot

            pauseBeforeShootingOnDifferentThread(); // start the pause (before starting to shoot)


        }
        catch (System.Exception e)
        {

            MyDebug("Error : " + e.ToString());
        }
    }
    float GetRandomStartDelay()
    {
        return Random.Range(10.0f, 15.0f);

    }
    void pauseBeforeShootingOnDifferentThread()
    {

        StartCoroutine(pauseBeforeStartingToShoot());

    }

    float GetRandomTalkingPauseAmount()
    {

        return Random.Range(2.0f, 6.0f);

    }
    void MyDebug(string someText)
    {

        if (debugText != null & debug)
        {
            debugText.text = someText;
        }

    }

    void PlayHummingSound_Immediately()
    {
        hummingSoundSource.loop = true;
        hummingSoundSource.clip = hummingSound;
        hummingSoundSource.volume = hummingVolume;
        hummingSoundSource.Play();

    }

    IEnumerator PlayRoarSound()
    {
        //https://answers.unity.com/questions/904981/how-to-play-an-audio-file-after-another-finishes.html
        talkingSoundIsPlaying = true;
        talkingSoundSource.PlayOneShot(talkingSound, talkingVolume);

        // wait for the length of actual sound clip, PLUS a random pause amount
        yield return new WaitForSeconds(talkingSound.length + GetRandomTalkingPauseAmount());
        talkingSoundIsPlaying = false;

    }

    void playTalkingOnDifferentThread()
    {

        StartCoroutine(PlayRoarSound());

    }
    void playTalkingSoundIfItIsTime()
    {

        if (!talkingSoundIsPlaying)
            playTalkingOnDifferentThread();

    }


    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        playTalkingSoundIfItIsTime();

        if (shootingAllowed)
        {
            shootIfTime();
        }
    }



    void chooseTarget()
    {

        

        blasterTarget = null;
        blasterTargetTag = null;

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
        GameObject[] ywings = LevelManager.filterGameObjectsInFrontOfPlayer(GameObject.FindGameObjectsWithTag("targetYWing"));


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

        if (ywings.Length > 0)
        {
            for (int i = 0; i < ywings.Length; i++)
            {
                list.Add(ywings[i]);
                tagList.Add("targetYWing");
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

    void destroySelf()
    {
        
        Destroy(gameObject);
    }

}
