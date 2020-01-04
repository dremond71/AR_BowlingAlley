using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XWingFighterShowcase : MonoBehaviour
{

    GameObject positioningStatus;
    
   
 

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
            else if (positioningStatus.tag == "positioning_on_scale") {
                
                increaseScale();
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
            else if (positioningStatus.tag == "positioning_on_scale") {
                
                decreaseScale();
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
         else if (positioningStatus.tag == "positioning_on_rotate") {
                rotateRight();
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
            else if (positioningStatus.tag == "positioning_on_rotate") {
                rotateLeft();
            }

    }

      
   void decreaseYvalue() {
          float x = this.transform.position.x;
          float y = this.transform.position.y;
          float z = this.transform.position.z;

          y-=0.1f;

          this.transform.position = new Vector3(x, y, z);
                 
   }     

 void decreaseXvalue() {
          float x = this.transform.position.x;
          float y = this.transform.position.y;
          float z = this.transform.position.z;

          x-=0.1f;

          this.transform.position = new Vector3(x, y, z);
                 
   }


void rotateLeft() {

    //https://docs.unity3d.com/ScriptReference/Transform.Rotate.html

    // transform.Rotate(90.0f, 0.0f, 0.0f, Space.World);

          float rotateAmount = -10.0f;
   

          this.transform.Rotate(0, rotateAmount, 0);
             
   }

void rotateRight() {

    //https://docs.unity3d.com/ScriptReference/Transform.Rotate.html

    // transform.Rotate(90.0f, 0.0f, 0.0f, Space.World);

          float rotateAmount = 10.0f;
   
          this.transform.Rotate(0, rotateAmount, 0);

                 
   }

void decreaseScale() {
          float value = 0.010f;
          float x = value;
          float y = value;
          float z = value;

          this.transform.localScale -= new Vector3(x, y, z);
                 
   }

void increaseScale() {
          float value = 0.010f;
          float x = value;
          float y = value;
          float z = value;

          this.transform.localScale += new Vector3(x, y, z);
     
                 
   }

void decreaseZvalue() {
          float x = this.transform.position.x;
          float y = this.transform.position.y;
          float z = this.transform.position.z;

          z-=0.1f;

          this.transform.position = new Vector3(x, y, z);
                 
   }

 void increaseYvalue() {
          float x = this.transform.position.x;
          float y = this.transform.position.y;
          float z = this.transform.position.z;

          y+=0.1f;

          this.transform.position = new Vector3(x, y, z);
                 
   }     
 void increaseXvalue() {
          float x = this.transform.position.x;
          float y = this.transform.position.y;
          float z = this.transform.position.z;

          x+=0.1f;

          this.transform.position = new Vector3(x, y, z);
                 
   }     

void increaseZvalue() {
          float x = this.transform.position.x;
          float y = this.transform.position.y;
          float z = this.transform.position.z;

          z+=0.1f;

          this.transform.position = new Vector3(x, y, z);
                 
   }     




}
