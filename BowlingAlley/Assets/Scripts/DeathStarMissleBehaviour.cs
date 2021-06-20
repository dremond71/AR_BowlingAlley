using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathStarMissleBehaviour : MonoBehaviour
{

    GameObject target;
   
    private float speedFactor = 30f;

    private float lifeDuration = 5.0f;
    private float lifeTimer;

    private TextMesh debugText;
    private bool debug = true;

    public void setTargetObject(GameObject targetObject)
    {
        target = targetObject;
    }


    // Start is called before the first frame update
    void Start()
    {
        debugText = GameObject.Find("debugText").GetComponent<TextMesh>();

        lifeTimer = lifeDuration;
    }

    void FixedUpdate()
    {

        this.GetComponent<Rigidbody>().velocity = speedFactor * this.transform.forward;


        // if there is a target, keep following it with turns
        if (target != null )
        {
            Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - this.transform.position);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, speedFactor * Time.deltaTime);
        }

        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {

            destroySelf();
        }
    }

    void destroySelf()
    {

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (
            (collision.gameObject.tag == "tantiveIV") ||
            (collision.gameObject.tag == "falcon") ||
            (collision.gameObject.tag == "targetXWing") ||
            (collision.gameObject.tag == "targetAWing") ||
            (collision.gameObject.tag == "PlayerShooter") ||
            (collision.gameObject.tag == "targetMeteorite") ||
            (collision.gameObject.tag == "starDestroyer") ||
            (collision.gameObject.tag == "PlayerShooter")             
        )
        {
            destroySelf();
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