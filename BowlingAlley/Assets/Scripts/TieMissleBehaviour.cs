using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TieMissleBehaviour : MonoBehaviour
{

    GameObject target;
    bool okToTurn = false;
    private float lifeDuration = 12.0f;
    private float lifeTimer;

    private float pauseDuration = 0.1f;
    private float pauseTimer;

    // Start is called before the first frame update
    void Start()
    {

        lifeTimer = lifeDuration;
        pauseTimer = pauseDuration;

        target = GameObject.FindGameObjectWithTag("tantiveIV");

        if (target == null)
            target = GameObject.FindGameObjectWithTag("falcon");


        //this.GetComponent<Rigidbody> ().velocity = 2f * this.transform.forward;

    }

    void FixedUpdate()
    {

        this.GetComponent<Rigidbody>().velocity = 3f * this.transform.forward;

        if (!okToTurn)
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f)
            {
                Debug.Log("ok to turn now");
                okToTurn = true;
            }
        }

        if (target != null && okToTurn)
        {

            // float i = turnSpeed * Time.deltaTime;
            //  transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, i);

            Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - this.transform.position);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, 2f * Time.deltaTime);
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
        Debug.Log("missle dead");
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (
            (collision.gameObject.tag == "tantiveIV") ||
            (collision.gameObject.tag == "falcon")
        )
        {
            destroySelf();
        }

    }
}