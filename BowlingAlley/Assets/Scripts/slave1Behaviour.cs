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

    int spawnPositionIndex = 1;

    private AudioSource roarSource;
    private AudioClip roar;

    public float blastVolume = 0.3f;

    public float roarVolume = 0.7f;

    bool roarSoundIsPlaying = false;

    private AudioSource blasterSource;
    private AudioClip blaster;

    private GameObject turboLasterBlastPrefab;

    private float blasterSpeed = 180.0f * 3.0f;

    private float shootingPauseTimer;

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
    }

    public void setSpawnPositionIndex(int theIndex)
    {
        spawnPositionIndex = theIndex;
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
            StartCoroutine(shoot());
            shootingPauseTimer = GetRandomShootingPauseAmount();

        }
    }

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

        shootIfTime();


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
