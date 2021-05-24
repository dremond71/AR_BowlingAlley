using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TieMissleBehaviour : MonoBehaviour {

    GameObject target;
    bool okToTurn = true;
    private float lifeDuration = 12.0f;
    private float lifeTimer;

    private float pauseDuration = 0.1f;
    private float pauseTimer;
    System.Random rnd = new System.Random ();
    private TextMesh debugText;
    private bool debug = true;

    void chooseTarget () {

        target = null;

        GameObject tantive    = GameObject.FindGameObjectWithTag ("tantiveIV");
        GameObject falcon     = GameObject.FindGameObjectWithTag ("falcon");
        GameObject[] xwings     = GameObject.FindGameObjectsWithTag("targetXWing");
        GameObject[] meteorites = GameObject.FindGameObjectsWithTag("targetMeteorite");
        GameObject[] awings     = GameObject.FindGameObjectsWithTag("targetAWing");

        if (target == null)
        {
            if (xwings.Length > 0)
            {
                target = xwings[0];
            }
        }

        if (target == null)
        {
            if (awings.Length > 0)
            {
                target = awings[0];
            }
        }

        if (target == null)
        {
            if (meteorites.Length > 0)
            {
                target = meteorites[0];
            }
        }

        if (target == null)
        {
            if (falcon !=null)
            {
                target = falcon;
            }
        }

        if (target == null)
        {
            if (tantive != null)
            {
                target = tantive;
            }
        }

        /*
        if ((tantive != null) && (falcon != null)) {

            //both ships are there; randomly choose
            bool value = (rnd.NextDouble () >= 0.5);

            if (value) {
                target = tantive;
            } else {
                target = falcon;
            }

        } else {

            // perhaps one ship is there
            // or neither
            if (tantive != null) {
                target = tantive;
            } else if (falcon != null) {
                target = falcon;
            } else {
                target = null;
            }
        }
        */

    }
    // Start is called before the first frame update
    void Start () {

        debugText = GameObject.Find ("debugText").GetComponent<TextMesh> ();

        lifeTimer = lifeDuration;
        pauseTimer = pauseDuration;

        chooseTarget ();

    }

    void FixedUpdate () {

        this.GetComponent<Rigidbody> ().velocity = 3f * this.transform.forward;

/*
        if (!okToTurn) {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f) {
                okToTurn = true;
            }
        }
*/

        if (target != null && okToTurn) {

            // float i = turnSpeed * Time.deltaTime;
            //  transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, i);

            Quaternion targetRotation = Quaternion.LookRotation (target.transform.position - this.transform.position);
            this.transform.rotation = Quaternion.Slerp (this.transform.rotation, targetRotation, 15f * Time.deltaTime);
            //this.GetComponent<Rigidbody> ().MoveRotation (Quaternion.RotateTowards (this.transform.rotation, targetRotation, 2f));
        }

        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f) {

            destroySelf ();
        }
    }

    void destroySelf () {

        Destroy (gameObject);
    }

    void OnCollisionEnter (Collision collision) {
      
        if (
            (collision.gameObject.tag == "tantiveIV") ||
            (collision.gameObject.tag == "falcon") ||
            (collision.gameObject.tag == "targetXWing") ||
             (collision.gameObject.tag == "targetAWing") ||
              (collision.gameObject.tag == "targetMeteorite") 


        ) {
            destroySelf ();
        }

    }

    void MyDebug (string someText) {

        if (debugText != null & debug) {
            debugText.text = someText;
        }

    }

}