using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchPlaneBehaviour : MonoBehaviour
{
     public GameObject bowlingBallPrefab;
     public GameObject rackPrefab;
     private AudioSource hiss;

     bool anyPinsLeft() {
        bool pinsLeft=false;

        GameObject[] pins = GameObject.FindGameObjectsWithTag("bowlingPin");
        //Debug.Log("pins left: " + pins.Length);
        pinsLeft = pins.Length>0;

        // foreach (GameObject pin in pins)
        //  {

        //   Debug.Log("pin left ==> " + pin.name);
    

        //  }
        return pinsLeft;

     }

     void Start() {
          hiss = GameObject.FindGameObjectWithTag("hiss_Sound").GetComponent<AudioSource>();
          hiss.volume = 0.5f;
     }

     void spawnNewBall() {
            // recreating ball
            GameObject ballSpawner = GameObject.FindGameObjectWithTag("ballSpawner");
            float x = ballSpawner.transform.position.x;
            float y = ballSpawner.transform.position.y;
            float z = ballSpawner.transform.position.z;
            GameObject go = (GameObject)  Instantiate(bowlingBallPrefab, new Vector3(x,y,z), Quaternion.identity);
     }


void spawnNewRack() {
            // recreating ball
            GameObject rackSpawner = GameObject.FindGameObjectWithTag("rackSpawner");
            float x = rackSpawner.transform.position.x;
            float y = rackSpawner.transform.position.y;
            float z = rackSpawner.transform.position.z;
            GameObject go = (GameObject)  Instantiate(rackPrefab, new Vector3(x,y,z), Quaternion.identity);
     }

 void destroyPinsThatAreNotVertical() {

         GameObject[] pins = GameObject.FindGameObjectsWithTag("bowlingPin");
         foreach (GameObject pin in pins)
         {

           float z = Mathf.Abs(pin.transform.eulerAngles.z );
           float y = Mathf.Abs(pin.transform.eulerAngles.y);

           if (  ( z > 10) || 
                 ( y > 10)
              )
           {
                //Debug.Log("extra killing not vertical pin: " + pin.name);
                tagAsDeleted(pin);
                Destroy(pin);
           }
        //    else  {
        //        Debug.Log("not killing " + pin.name + " y,z==" + y + "," + z );
        //    }
    

         }
 }

     void handleBallLogicOnDifferentThread() {

        StartCoroutine(DoWork());

     }

bool falling(GameObject pin) {
      GameObject floor = GameObject.FindGameObjectWithTag("floor");
      float floor_y = floor.transform.position.y ;
      float pin_y = pin.transform.position.y;
     // Debug.Log("pin falling => y: " + pin_y + " floor y: " + floor_y);
      return ( pin_y < floor_y);
}

IEnumerator DoWork()
    {
        
        //Debug.Log("Started DoWork : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(3);

        //After we have waited 5 seconds print the time again.
        //Debug.Log("Finished pausing in DoWork : " + Time.time);

        whatsNext();

        //Debug.Log("Finished DoWork : " + Time.time);
    }

     void whatsNext() {
        destroyPinsThatAreNotVertical();
        
        if (!anyPinsLeft()) {
            //Debug.Log("About to spawn new rack");
           spawnNewRack();
        }
        // else {
        //     Debug.Log("Not spawning new rack");
        // }

        spawnNewBall();
     }

     void tagAsDeleted(GameObject pin) {
        pin.tag="deleted_pin";
        
     }

    
     void OnCollisionEnter(Collision c) {


        if (c.gameObject.tag == "bowlingPin"){
           //Debug.Log("Killing Pin : " + c.gameObject.name);
           tagAsDeleted(c.gameObject);
           Destroy(c.gameObject);
           //Debug.Log("Killing Pin");
        }
        else if (c.gameObject.tag == "bowlingBall"){
            Destroy(c.gameObject);
            hiss.Play();
            //Debug.Log("Killing Ball");

           //Invoke("whatsNext",3);
           handleBallLogicOnDifferentThread();
           
        }
    }
}
