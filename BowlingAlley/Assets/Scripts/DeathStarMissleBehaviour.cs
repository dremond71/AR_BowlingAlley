using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathStarMissleBehaviour : MonoBehaviour
{

    GameObject target;
    bool okToTurn = true;

    private float speedFactor = 30f;

    private float lifeDuration = 12.0f;
    private float lifeTimer;

    private float pauseDuration = 0.1f;
    private float pauseTimer;
    System.Random rnd = new System.Random();
    private TextMesh debugText;
    private bool debug = true;

    void chooseTarget()
    {

        //GameObject tantive = GameObject.FindGameObjectWithTag("tantiveIV");
        //GameObject falcon = GameObject.FindGameObjectWithTag("falcon");
        GameObject tantive = GameObject.FindGameObjectWithTag("targetXWing");
        GameObject falcon = GameObject.FindGameObjectWithTag("targetAWing");


        if ((tantive != null) && (falcon != null))
        {

            //both ships are there; randomly choose
            bool value = (rnd.NextDouble() >= 0.5);

            if (value)
            {
                target = tantive;
            }
            else
            {
                target = falcon;
            }

        }
        else
        {

            // perhaps one ship is there
            // or neither
            if (tantive != null)
            {
                target = tantive;
            }
            else if (falcon != null)
            {
                target = falcon;
            }
            else
            {
                target = null;
            }
        }

    }
    // Start is called before the first frame update
    void Start()
    {

        debugText = GameObject.Find("debugText").GetComponent<TextMesh>();

        lifeTimer = lifeDuration;
        pauseTimer = pauseDuration;

        chooseTarget();

    }

    void FixedUpdate()
    {

        this.GetComponent<Rigidbody>().velocity = speedFactor * this.transform.forward;

/*
        if (!okToTurn)
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f)
            {
                okToTurn = true;
            }
        }
*/
        if (target != null && okToTurn)
        {

            // float i = turnSpeed * Time.deltaTime;
            //  transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, i);

            Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - this.transform.position);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, speedFactor * Time.deltaTime);
            //this.GetComponent<Rigidbody> ().MoveRotation (Quaternion.RotateTowards (this.transform.rotation, targetRotation, 2f));
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
            (collision.gameObject.tag == "targetAWing") 
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