using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinBehaviour : MonoBehaviour
{
    public Rigidbody rb;
    private AudioSource crash;

    void Start() {
         crash = GameObject.FindGameObjectWithTag("hitByBall_Sound").GetComponent<AudioSource>();
    }
    
    void OnCollisionEnter(Collision c) {
        if (c.gameObject.tag == "bowlingBall"){
           crash.Play();
        }
       
    }
}
