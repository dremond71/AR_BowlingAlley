using UnityEngine;
using System.Collections;

public class TestFlyerScript : MonoBehaviour {
    
    Animator animator;
    float newX;
    Animation swoop;
// https://docs.unity3d.com/540/Documentation/Manual/AnimationParameters.html

// for turning off physics before animation: https://docs.unity3d.com/ScriptReference/Rigidbody-isKinematic.html
    private bool isPlaying = false;



     TextMesh debugText;
    Rigidbody rb;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        
        
         debugText =  GameObject.Find("debugText").GetComponent<TextMesh>();
         debugText.text = "working...";

         rb = GetComponent<Rigidbody>();

    //      swoop = GameObject.FindGameObjectWithTag("testFlyer_swoop_Animation").GetComponent<Animation>();
    //      if (swoop !=null)
    //         debugText.text = "found swoop animation";
    //
     }
    
    // Update is called once per frame
    void Update () {
        
        handleInputWindows();
        
        
    }

    void OnCollisionEnter(Collision col) {
       
    }


     void handleInputWindows() {

        if (Input.GetKeyDown(KeyCode.LeftArrow)){
           isPlaying=false;
            //move_Left();
             animator.SetBool("doTheSwoop", false);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)){
             isPlaying=false;
         // move_Right();
          animator.SetBool("doTheSwoop", false);
        }

        else if (Input.GetKeyDown(KeyCode.UpArrow)){
         // move_Forward();


        debugText.text = "up pressed";

       
         
        //  float animSpeed = 0.6f;
         
          if (!isPlaying) {
               isPlaying=true;
               animator.speed = 0.5f;
               rb.isKinematic = true;
               rb.detectCollisions = false;
               animator.SetBool("doTheSwoop", true);
               // animator.SetBool("doTheSwoop", false);
          }
          
    
         
        //      isPlaying = true;
        //      //swoop.speed = 0.5f;
        //      //
        //      debugText.text = "up pressed";
        //      swoop.Play();
        //      debugText.text = "after the Play command";

        //  }
  
        }   

     }



//        void move_Left() {
//             Vector3 position = this.transform.position;
//             newX = position.x - 0.05f;
//             //animator.SetFloat("newX",newX);
//             animator.SetBool("moveL", true);
//             animator.SetBool("moveR", false);
//             //animator.SetBool("moveForward", false);
//    }

//    void move_Right() {
//             Vector3 position = this.transform.position;
//              newX = position.x + 0.05f;
//              //animator.SetFloat("newX",newX);
//              animator.SetBool("moveL", false);
//              animator.SetBool("moveR", true);
//              //animator.SetBool("moveForward", false);
//    }

//    void move_Forward() {
//             Vector3 position = this.transform.position;
//              newX = position.x + 0.05f;
             
//              animator.SetBool("moveL", false);
//              animator.SetBool("moveR", false);
//              //animator.SetBool("moveForward", true);
//    }
   
   // as soon as my animation starts, send an event that calls this function... to prevent it from looping.
   // yay, me !!!!
   public void myAnimationStarted(string value) {
       Debug.Log(value);
       //
       animator.SetBool("doTheSwoop", false);
   }
}

