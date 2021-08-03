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

    private bool debug = false;
    private TextMesh debugText;


    int spawnPositionIndex = 1;


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



    }

}
