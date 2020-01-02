using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBall : MonoBehaviour
{
   
    public Rigidbody rb;
    private  AudioSource RollingBall;
    private bool onAndroid = true;
    private int throwForceValue=700;
   

// swipe gesture variables - begin
    private Vector2 fingerDown;
    private Vector2 fingerUp;
    private bool detectSwipeOnlyAfterRelease = true;
    private float SWIPE_THRESHOLD = 20f;

    private Vector2 direction;
    float touchTimeStart, touchTimeFinish, timeInterval;
    float throwForceInZ = 75f;
    float updated_horizontal_swipe_X_position;
// swipe gesture variables - end

 GameObject positioningStatus;


   void Start() {
      RollingBall = GameObject.FindGameObjectWithTag("rollingBall_Sound").GetComponent<AudioSource>();
      
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
        
        
    }

    void OnSwipeDown()
    {
        Debug.Log("Swipe Down");
        //nothing to do
        
    }

    void OnSwipeLeft()
    {
        Debug.Log("Swipe Left");
        
        if (positioningStatus.tag == "positioning_off"){
            moveBall_Horizontal_Swipe();
        }
      
        
    }

    void OnSwipeRight()
    {
        Debug.Log("Swipe Right");
        
        if (positioningStatus.tag == "positioning_off"){
           moveBall_Horizontal_Swipe();
        }
       

    }
}
