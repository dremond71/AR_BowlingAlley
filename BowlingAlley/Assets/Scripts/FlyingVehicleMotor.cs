using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingVehicleMotor : MonoBehaviour
{
  
  private float blasterSpeed = 180.0f;


     
     
     private GameObject xwingBlastOrigin1;
     private GameObject xwingBlastOrigin2;
     private GameObject xwingBlastOrigin3;
     private GameObject xwingBlastOrigin4;
     private GameObject xwingBlastPrefab;
      private  AudioSource roarSource;
     private AudioClip roar;
     
     private AudioSource blasterSource;
     private AudioClip blaster;
    
      private TextMesh debugText;
     private bool shooting = false;
     private float pauseAfterShooting = 0.15f;
        

 void Awake()
    {
       
        blasterSource = GameObject.FindGameObjectWithTag("xwingFighterBlasts_Sound").GetComponent<AudioSource>();
        blaster = blasterSource.clip;

         roarSource =  GameObject.FindGameObjectWithTag("xwingFighterRoar_Sound").GetComponent<AudioSource>();
         roar = roarSource.clip;

     xwingBlastPrefab = PrefabFactory.getPrefab("flyingXWingBlastParent");    
      debugText = GameObject.Find("debugText").GetComponent<TextMesh>();

   
       

    }

 
private void FixedUpdate() {
    transform.position += transform.forward * Time.deltaTime * 90.0f;
    transform.Rotate(Input.GetAxis("Vertical"),0.0f,-Input.GetAxis("Horizontal") );

// float i = turnSpeed * Time.deltaTime;
//  transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, i);

     if (Input.GetButton("Fire1") ) {
       if (!shooting){
            shooting = true;
            shoot();
            onDifferentThread_PauseAfterShooting();
       }
           
     }
    
}


//  void LateUpdate()
//     {
       
//         AnimatorStateInfo currentAnimatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);
//          myObject.position = position;
//     }

/*
Quaternion targetRotation = Quaternion.identity;
float turnSpeed = 10;


public void rotateToFace(Vector3 facing, turnSpeed)
{
    // assign new target rotation
    // (call only when a new direction assigned)
    Vector3 direction = new Vector3(facing.x, 0, facing.z);
    Quaternion targetRotation = Quaternion.LookRotation(direction);
    this.turnSpeed = turnSpeed;
}


void Update() {


 float i = turnSpeed * Time.deltaTime;
 transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, i);

}
*/

 void PlayRoarSound_Immediately()
    {
      // we want a seperate object for each fire; so we can handle multi blasts in quick succession
      
      // Method I used to achieve multiple blasts that don't interrupt each other:
      // https://docs.unity3d.com/ScriptReference/AudioSource.PlayOneShot.html
     
        roarSource.PlayOneShot(roar,0.7f);
       
     
     
     
    }

  

  void PlayBlasterSound_Immediately()
    {
      // we want a seperate object for each fire; so we can handle multi blasts in quick succession
      
      // Method I used to achieve multiple blasts that don't interrupt each other:
      // https://docs.unity3d.com/ScriptReference/AudioSource.PlayOneShot.html
      blasterSource.PlayOneShot(blaster,0.7f);
     
     
    }



       void spawnNewBlasterBolt() {
            // recreating ball
            
 
            // bolt 1
            xwingBlastOrigin1 =  GameObject.FindGameObjectWithTag("xwingBlasterPosition1");
            float x = xwingBlastOrigin1.transform.position.x;
            float y = xwingBlastOrigin1.transform.position.y;
            float z = xwingBlastOrigin1.transform.position.z;
            
            GameObject go = (GameObject)  Instantiate(xwingBlastPrefab, new Vector3(x,y,z),  xwingBlastOrigin1.transform.rotation);
            GameObject bolt1 = PrefabFactory.GetChildWithName(go,"flyingXWingBlast");
           
           
            // bolt 2
            xwingBlastOrigin2 =  GameObject.FindGameObjectWithTag("xwingBlasterPosition2");
            x = xwingBlastOrigin2.transform.position.x;
            y = xwingBlastOrigin2.transform.position.y;
            z = xwingBlastOrigin2.transform.position.z;
            
             go = (GameObject)  Instantiate(xwingBlastPrefab, new Vector3(x,y,z),  xwingBlastOrigin2.transform.rotation);
         
             GameObject bolt2 = PrefabFactory.GetChildWithName(go,"flyingXWingBlast");
          

            // bolt 3
            xwingBlastOrigin3 =  GameObject.FindGameObjectWithTag("xwingBlasterPosition3");
            x = xwingBlastOrigin3.transform.position.x;
            y = xwingBlastOrigin3.transform.position.y;
            z = xwingBlastOrigin3.transform.position.z;
            
             go = (GameObject)  Instantiate(xwingBlastPrefab, new Vector3(x,y,z),  xwingBlastOrigin3.transform.rotation);
            GameObject bolt3 = PrefabFactory.GetChildWithName(go,"flyingXWingBlast");
          

            // bolt 4
            xwingBlastOrigin4 =  GameObject.FindGameObjectWithTag("xwingBlasterPosition4");
            x = xwingBlastOrigin4.transform.position.x;
            y = xwingBlastOrigin4.transform.position.y;
            z = xwingBlastOrigin4.transform.position.z;
            
             go = (GameObject)  Instantiate(xwingBlastPrefab, new Vector3(x,y,z),  xwingBlastOrigin4.transform.rotation);
            GameObject bolt4 = PrefabFactory.GetChildWithName(go,"flyingXWingBlast");
           

    
           
            bolt1.GetComponent<Rigidbody>().velocity = blasterSpeed * xwingBlastOrigin1.transform.forward;
            bolt2.GetComponent<Rigidbody>().velocity = blasterSpeed * xwingBlastOrigin1.transform.forward;
            bolt3.GetComponent<Rigidbody>().velocity = blasterSpeed * xwingBlastOrigin1.transform.forward;
            bolt4.GetComponent<Rigidbody>().velocity = blasterSpeed * xwingBlastOrigin1.transform.forward;
     
   





     }

void onDifferentThread_PauseAfterShooting() {

        StartCoroutine(DoThePausing());

}

IEnumerator DoThePausing()
    {
        yield return new WaitForSeconds(pauseAfterShooting);
        shooting = false;

    }

   void shoot() {
         spawnNewBlasterBolt();
         PlayBlasterSound_Immediately();
    }


   void MyDebug(string someText)
    {

        if (debugText != null)
        {
            debugText.text = someText;
        }

    }
}
