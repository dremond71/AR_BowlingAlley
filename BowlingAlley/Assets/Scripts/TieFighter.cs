using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
Cannot have prefabs which require a prefab

https://answers.unity.com/questions/15565/placing-prefabs-in-prefabs.html

perhaps a singleton factory pattern

https://www.patrykgalach.com/2019/03/28/implementing-factory-design-pattern-in-unity/

Scale for bowling alley : 0.5,0.5,0.5

// for turning off physics before animation: https://docs.unity3d.com/ScriptReference/Rigidbody-isKinematic.html

*/

public class TieFighter : MonoBehaviour
{
  
     private float tieBlastForce = 1000.0f;
     private float throwForceInZ = 35.0f;
     private float tieFighterZForce;
     private GameObject tieBlastOrigin;
     private GameObject tieBlastPrefab;
     private bool rolling = false;
    // TextMesh debugText;
 private bool onAndroid = true;


     private  AudioSource roarSource;
     private AudioClip roar;
     
     private AudioSource blasterSource;
     private AudioClip blaster;

// swipe gesture variables - begin
    private Vector2 fingerDown;
    private Vector2 fingerUp;
    private bool detectSwipeOnlyAfterRelease = true;
    private float SWIPE_THRESHOLD = 20f;
    private float changeInY;
    private Vector2 direction;
    float touchTimeStart, touchTimeFinish, timeInterval;
    
    float updated_horizontal_swipe_X_position;

   
    bool roarSoundIsPlaying = false;
// swipe gesture variables - end

    
   
    void Awake()
    {

        

        
       
        
        blasterSource = GameObject.FindGameObjectWithTag("tieFighterBlaster_Sound").GetComponent<AudioSource>();
        blaster = blasterSource.clip;

         roarSource =  GameObject.FindGameObjectWithTag("tieFighterRoar_Sound").GetComponent<AudioSource>();
         roar = roarSource.clip;

      
       
    }


 

   void Start() {

        tieBlastPrefab = PrefabFactory.getPrefab("tieBlast");
   }

    void Update()
    {
        handleInputAndroid();
        
    }





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
            
 
            tieBlastOrigin =  GameObject.FindGameObjectWithTag("tieFighterBlastOrigin");
            float x = tieBlastOrigin.transform.position.x;
            float y = tieBlastOrigin.transform.position.y;
            float z = tieBlastOrigin.transform.position.z;
            GameObject go = (GameObject)  Instantiate(tieBlastPrefab, new Vector3(x,y,z), Quaternion.identity);
            GameObject bolt1 = PrefabFactory.GetChildWithName(go,"leftBlast");
            GameObject bolt2 = PrefabFactory.GetChildWithName(go,"rightBlast");

            //bolt1.SetActive(true);
           // bolt2.SetActive(true);

           // if tie fighter is rolling down the alley, add ITS force to the force of the bolts; to keep bolts ahead of tie fighter.
           float blasterForce = tieBlastForce;
           if (rolling)
               blasterForce+=tieFighterZForce;

            bolt1.GetComponent<Rigidbody>().AddForce(0,0,blasterForce);
            bolt2.GetComponent<Rigidbody>().AddForce(0,0,blasterForce);
  
     }




void handleInputAndroid(){

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUp = touch.position;
                fingerDown = touch.position;

                timeInterval=0.0f;
                changeInY=0.0f;
                touchTimeStart = Time.time;
            }

            //Detects Swipe while finger is still moving
            if (touch.phase == TouchPhase.Moved)
            {
               

                if (!detectSwipeOnlyAfterRelease)
                {
                    fingerDown = touch.position;
                    touchTimeFinish = Time.time;
                    direction = fingerDown - fingerUp;
                     timeInterval = touchTimeFinish-touchTimeStart;
                     changeInY = fingerDown.y - fingerUp.y;// we only have swipe up/down (y) or left/right (x)...no Z . 
                    checkSwipe();
                }
            }

            //Detects swipe after finger is released
            if (touch.phase == TouchPhase.Ended)
            {
                 

                fingerDown = touch.position;
                touchTimeFinish = Time.time;
                direction = fingerDown - fingerUp;
                timeInterval = touchTimeFinish-touchTimeStart;
                checkSwipe();
            }
        }

 

}


  void checkSwipe()
    {
        //Check if Vertical swipe
        if (verticalMove() > SWIPE_THRESHOLD && verticalMove() > horizontalValMove())
        {
            //Debug.Log("Vertical");
            if (fingerDown.y - fingerUp.y > 0)//up swipe
            {
                OnSwipeUp();
            }
            else if (fingerDown.y - fingerUp.y < 0)//Down swipe
            {
                OnSwipeDown();
            }
            fingerUp = fingerDown;
        }

        //Check if Horizontal swipe
        else if (horizontalValMove() > SWIPE_THRESHOLD && horizontalValMove() > verticalMove())
        {
            //Debug.Log("Horizontal");
            if (fingerDown.x - fingerUp.x > 0)//Right swipe
            {
                OnSwipeRight();
            }
            else if (fingerDown.x - fingerUp.x < 0)//Left swipe
            {
                OnSwipeLeft();
            }
            fingerUp = fingerDown;
        }

        //No Movement at-all
        else
        {
            //Debug.Log("No Swipe!");
        }
    }

    float verticalMove()
    {
        return Mathf.Abs(fingerDown.y - fingerUp.y);
    }

    float horizontalValMove()
    {
        return Mathf.Abs(fingerDown.x - fingerUp.x);
    }


    void OnSwipeRight()
    {
        Debug.Log("Swipe Right");
        
        if (!roarSoundIsPlaying)
            playRoarOnDifferentThread();

       moveTieFighter_Horizontal_Swipe();

    }

    void throwMeAndShootBlasters() {


        StartCoroutine(raceDownAlleyShooting());
     
    }
 //////////////////////////////////CALLBACK FUNCTIONS/////////////////////////////
    void OnSwipeUp()
    {
        Debug.Log("Swipe UP");
        if (! rolling){
            rolling = true;
            throwMeAndShootBlasters();
            //shoot();
        }
        
        
    }
  
    void doNotMove_But_Shoot() {

        //https://docs.unity3d.com/ScriptReference/Rigidbody.Sleep.html?_ga=2.116748998.1178135617.1577509156-1865444771.1576192928
        Rigidbody rb = this.GetComponent<Rigidbody>();

        //rb.Sleep();
        //shoot();
        //rb.WakeUp();

        rb.isKinematic = true;
        shoot();
        rb.isKinematic = false;
        
    }

    void OnSwipeDown()
    {
        Debug.Log("Swipe Down");
        //nothing to do
        //shoot();
        doNotMove_But_Shoot();


        
    }

    void OnSwipeLeft()
    {
        Debug.Log("Swipe Left");
        
        if (!roarSoundIsPlaying)
            playRoarOnDifferentThread();

        moveTieFighter_Horizontal_Swipe();
      
        
    }

    void shoot() {
         spawnNewBlasterBolt();
         PlayBlasterSound_Immediately();
    }


void moveTieFighter_Horizontal_Swipe() {

    // ONLY thing that moved bowling ball to proper position.

    Ray ray = Camera.main.ScreenPointToRay(fingerDown);// the last position of the swipe
    RaycastHit hit;
    // https://docs.unity3d.com/ScriptReference/RaycastHit.html
    if (Physics.Raycast(ray,out hit, 500.0f)) {//see what object it hit in the virtual world

        float new_X = hit.point.x ;//get the point X in the virtual world where there was a hit

            Vector3 position = this.transform.position;
            position.x = new_X; //update the position of the ball to that X coordinate
            this.transform.position = position;

    }    


}

 void delayInSeconds(float seconds){
    StartCoroutine(PauseInSeconds(seconds));
 }

 void playRoarOnDifferentThread() {

        StartCoroutine(PlayRoarSound());

     }

IEnumerator PlayRoarSound()
    {
        //https://answers.unity.com/questions/904981/how-to-play-an-audio-file-after-another-finishes.html
        roarSoundIsPlaying = true;
        roarSource.PlayOneShot(roar,0.7f);
        yield return new WaitForSeconds(roar.length);
        roarSoundIsPlaying = false;

    }

IEnumerator PauseInSeconds(float seconds)
    {

        yield return new WaitForSeconds(seconds);
        
    }

IEnumerator raceDownAlleyShooting()
    {

       // #1 attempt
       // velocity : (DistanceTravelled / TimeTaken)
       // speed due to swipe should be:  ForceAmount * (DistanceTravelled / TimeTaken)
       // pff. this didn't work 
        //tieFighterZForce = (changeInY/timeInterval) * throwForceInZ;
        //this.GetComponent<Rigidbody>().AddForce(0,0,tieFighterZForce);
        // would only move a bit, fire two shots, and move a bit again, and fire last 4 shots.

        // # 2 attempt
        // on swipe if user takes long... then force is divided by larger number...so slower force
        // on swipe if user goes fast...  then force is divided by a smaller number...so faster force
        tieFighterZForce = throwForceInZ/timeInterval;//calculate force based on user's swipe up intention.
        this.GetComponent<Rigidbody>().AddForce(0,0,tieFighterZForce);


        // float speed = 100.0f;
        //this.transform.Translate(Vector3.forward * Time.deltaTime * speed);


        // works:  // https://docs.unity3d.com/ScriptReference/Rigidbody-velocity.html
        //this.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 3);
        // for (int i = 0; i < 10; i++) 
        // { 
        //   shoot();
        //   yield return new WaitForSeconds(0.15f);
        // }

        float shotDelay=0.1f;
        float pauseInShooting=0.3f;
        //two shots
        shoot();
        yield return new WaitForSeconds(shotDelay);
        shoot();

        //pause
        yield return new WaitForSeconds(pauseInShooting);

        //four shots
        shoot();
        yield return new WaitForSeconds(shotDelay);
        shoot();
        yield return new WaitForSeconds(shotDelay);
        shoot();
        yield return new WaitForSeconds(shotDelay);
        shoot();
        yield return new WaitForSeconds(shotDelay);

        rolling = false;//just in case user didn't throw tie fighter past the end of alley. 
       
        
    }


 
}
