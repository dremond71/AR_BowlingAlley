using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBall_Original : MonoBehaviour
{
   
    public Rigidbody rb;
    private  AudioSource RollingBall;
    private bool onAndroid = true;
    private int throwForceValue=700;
    private GameObject bottomFloor;


// swipe gesture variables - begin
    private Vector2 fingerDown;
    private Vector2 fingerUp;
    private bool detectSwipeOnlyAfterRelease = true;
    private float SWIPE_THRESHOLD = 20f;

    private Vector2 direction;
    float touchTimeStart, touchTimeFinish, timeInterval;
    float throwForceInZ = 100f;
    float updated_horizontal_swipe_X_position;
// swipe gesture variables - end

 GameObject positioningStatus;


   void Start() {
      RollingBall = GameObject.FindGameObjectWithTag("rollingBall_Sound").GetComponent<AudioSource>();
       bottomFloor = GameObject.FindGameObjectWithTag("bottomFloor");
        positioningStatus      =  GameObject.Find("positioningStatus");
   }

   void OnCollisionEnter(Collision c) {
        if (c.gameObject.tag == "bowlingPin"){
           RollingBall.Stop();
        }
       
    }


 

   void moveBall_Left() {
            Vector3 position = this.transform.position;
            position.x = position.x - 0.05f;
             this.transform.position = position;
   }

   void moveBall_Right() {
            Vector3 position = this.transform.position;
             position.x = position.x + 0.05f;
             this.transform.position = position; 
   }

   void handleInputWindows() {

        if (Input.GetKeyDown(KeyCode.LeftArrow)){
            moveBall_Left();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)){
           moveBall_Right();
        }

        else if (Input.GetKeyDown(KeyCode.UpArrow)){
          throwBall();
        }   


        //
        // See if menu is being clicked
        // 

        if (Input.GetMouseButtonDown(0)){ // if left button pressed...
            //Debug.Log("Mouse down");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            RaycastHit hit;
            if (Physics.Raycast(ray,out hit, 100.0f)){
            // the object identified by hit.transform was clicked
            // do whatever you want
                //Debug.Log("Got a hit");
                if (hit.transform && hit.transform.gameObject){
                    //Debug.Log("Hit has a game object");
                    
                   
                    string name = hit.collider.transform.gameObject.name;

                    if (name == "leftButton"){
                            moveBall_Left();   
                    }
                    else if (name == "rightButton") {
                        moveBall_Right();
                    }
                    else if (name == "throwButton") {
                        throwBall();
                    }
                    else if (name == "decreaseYButton") {
                        decreaseYvalue();
                    }
                    
                }//if




            }// got raycast hit
        }       

   }

void handleInputAndroid(){

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUp = touch.position;
                fingerDown = touch.position;

                timeInterval=0f;
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


   void handleInputAndroid2(){

     int nbTouches = Input.touchCount;
 
      if(nbTouches > 0)
      {


           for (int i = 0; i < nbTouches; i++)
            {
                Touch touch = Input.GetTouch(i);
 
                if(touch.phase == TouchPhase.Began)
                {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
 
                    RaycastHit hit;
                   

                        if (Physics.Raycast(ray,out hit, 100.0f)) {
                            if (hit.transform && hit.transform.gameObject){

                                
                                //playRelevantSound(hit.collider.transform.gameObject.name);

                                string name = hit.collider.transform.gameObject.name;

                                if (name == "leftButton"){
                                     moveBall_Left();   
                                }
                                else if (name == "rightButton") {
                                    moveBall_Right();
                                }
                                else if (name == "throwButton") {
                                    throwBall();
                                }
                                else if (name == "decreaseYButton") {
                                    decreaseYvalue();
                                }
                                                              
                            }//if
                        }//if

                }// touch began
 
            }//for


      }// there was a touch

   }

   void handleInput() {
      if (onAndroid)
         handleInputAndroid();
      else 
         handleInputWindows();
   }

    bool falling() {
      GameObject floor = GameObject.FindGameObjectWithTag("floor");
      float floor_y = floor.transform.position.y ;
      float ball_y = this.transform.position.y;
      //Debug.Log("ball y: " + ball_y + " floor y: " + floor_y);
      return ( ball_y < floor_y);
    }
  
    // Start is called before the first frame update
    void throwBall()
    {
      
       rb.AddForce(0,0,throwForceValue);
       RollingBall.Play(); 
    }

 void throwBall_from_swipe_up()
    {
       
       if (timeInterval>0){
           
          rb.AddForce(0,0,throwForceInZ/timeInterval);
          RollingBall.Play(); 
       }
    }

void moveBall_Horizontal_Swipe() {

// ONLY thing that moved bowling ball to proper position.

Ray ray = Camera.main.ScreenPointToRay(fingerDown);// the last position of the swipe
RaycastHit hit;
// https://docs.unity3d.com/ScriptReference/RaycastHit.html
if (Physics.Raycast(ray,out hit, 100.0f)) {//see what object it hit in the virtual world

    float new_X = hit.point.x ;//get the point X in the virtual world where there was a hit

           Vector3 position = this.transform.position;
           position.x = new_X; //update the position of the ball to that X coordinate
           this.transform.position = position;

}    


}

    // Update is called once per frame
    void Update()
    {

        if (falling()){
         // Debug.Log("Falling Ball");
          RollingBall.Stop();
        }

        handleInput();
     
    }

  
   void decreaseYvalue() {
          float x = bottomFloor.transform.position.x;
          float y = bottomFloor.transform.position.y;
          float z = bottomFloor.transform.position.z;

          y-=0.1f;

          bottomFloor.transform.position = new Vector3(x, y, z);
                 
   }     

 void decreaseXvalue() {
          float x = bottomFloor.transform.position.x;
          float y = bottomFloor.transform.position.y;
          float z = bottomFloor.transform.position.z;

          x-=0.1f;

          bottomFloor.transform.position = new Vector3(x, y, z);
                 
   }

void decreaseZvalue() {
          float x = bottomFloor.transform.position.x;
          float y = bottomFloor.transform.position.y;
          float z = bottomFloor.transform.position.z;

          z-=0.1f;

          bottomFloor.transform.position = new Vector3(x, y, z);
                 
   }

 void increaseYvalue() {
          float x = bottomFloor.transform.position.x;
          float y = bottomFloor.transform.position.y;
          float z = bottomFloor.transform.position.z;

          y+=0.1f;

          bottomFloor.transform.position = new Vector3(x, y, z);
                 
   }     
 void increaseXvalue() {
          float x = bottomFloor.transform.position.x;
          float y = bottomFloor.transform.position.y;
          float z = bottomFloor.transform.position.z;

          x+=0.1f;

          bottomFloor.transform.position = new Vector3(x, y, z);
                 
   }     

void increaseZvalue() {
          float x = bottomFloor.transform.position.x;
          float y = bottomFloor.transform.position.y;
          float z = bottomFloor.transform.position.z;

          z+=0.1f;

          bottomFloor.transform.position = new Vector3(x, y, z);
                 
   }     

// functions below are involved with swipe gestures

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

    //////////////////////////////////CALLBACK FUNCTIONS/////////////////////////////
    void OnSwipeUp()
    {
        Debug.Log("Swipe UP");

        if (positioningStatus.tag == "positioning_off"){
           throwBall_from_swipe_up();
        }
        else {
            // alley positioning menu is up
            if (positioningStatus.tag == "positioning_on"){
                //do nothing, they haven't chose x,y, or z
            }
            else if (positioningStatus.tag == "positioning_on_y") {
                increaseYvalue();
            }
            else if (positioningStatus.tag == "positioning_on_z") {
                increaseZvalue();
            }

        }
        
    }

    void OnSwipeDown()
    {
        Debug.Log("Swipe Down");
        if (positioningStatus.tag == "positioning_off"){
           
        }
        else {
            // alley positioning menu is up
            if (positioningStatus.tag == "positioning_on"){
                //do nothing, they haven't chose x,y, or z
            }
            else if (positioningStatus.tag == "positioning_on_y") {
                decreaseYvalue();
            }
            else if (positioningStatus.tag == "positioning_on_z") {
                decreaseZvalue();
            }

        }
        
    }

    void OnSwipeLeft()
    {
        Debug.Log("Swipe Left");
        
        if (positioningStatus.tag == "positioning_off"){
            moveBall_Horizontal_Swipe();
        }
        else {
            // alley positioning menu is up
            if (positioningStatus.tag == "positioning_on"){
                //do nothing, they haven't chose x,y, or z
            }
            else if (positioningStatus.tag == "positioning_on_x") {
                decreaseXvalue();
            }

        }
        
    }

    void OnSwipeRight()
    {
        Debug.Log("Swipe Right");
        
        if (positioningStatus.tag == "positioning_off"){
           moveBall_Horizontal_Swipe();
        }
        else {
            // alley positioning menu is up
            if (positioningStatus.tag == "positioning_on"){
                //do nothing, they haven't chose x,y, or z
            }
            else if (positioningStatus.tag == "positioning_on_x") {
                increaseXvalue();
            }

        }

    }
}
