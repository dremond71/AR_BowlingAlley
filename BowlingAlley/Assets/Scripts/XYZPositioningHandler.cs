using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XYZPositioningHandler : MonoBehaviour
{

    GameObject positioningStatus;
    GameObject bottomFloor;
   
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

    void Start()
    {
     positioningStatus      =  GameObject.Find("positioningStatus");
     bottomFloor = GameObject.FindGameObjectWithTag("bottomFloor");  
    }

    
    void Update()
    {
        if (positioningStatus.tag != "positioning_off"){
           handleInput();
        }//if
    }


     void handleInput() {
     
         handleInputAndroid();
     
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

   void OnSwipeUp()
    {
        Debug.Log("Swipe UP");

       
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

    void OnSwipeDown()
    {
    
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

    void OnSwipeLeft()
    {
        Debug.Log("Swipe Left");
        
    
       
            // alley positioning menu is up
            if (positioningStatus.tag == "positioning_on"){
                //do nothing, they haven't chose x,y, or z
            }
            else if (positioningStatus.tag == "positioning_on_x") {
                decreaseXvalue();
            }

       
        
    }

    void OnSwipeRight()
    {
        Debug.Log("Swipe Right");
        
  
            // alley positioning menu is up
            if (positioningStatus.tag == "positioning_on"){
                //do nothing, they haven't chose x,y, or z
            }
            else if (positioningStatus.tag == "positioning_on_x") {
                increaseXvalue();
            }

      

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



}
