using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameBehaviour : MonoBehaviour
{
    public GameObject defaultBowlingBallPrefab;
    public GameObject empireBowlingBallPrefab;
    public GameObject rebelsBowlingBallPrefab;

    public GameObject rackPrefab;

    GameObject positioningStatus;
    private GameObject bowlingBall;
    private GameObject floor;
    
    private float pauseBeforeDroppingBall = 3.0f;
    private float pauseBeforeDroppingRack = 4.0f;
    private bool performingRackLogic = false;
    private bool performingBallLogic = false;

    private bool ballKilled;
    private bool pinsKilled;

    private GameObject themeStatusHolder;
    
    private string previousThemeTag;

    private  AudioSource welcome;
    private  AudioSource swipingInstructions;
    private  AudioSource controlPanelLocation;
    private bool playIntroSounds = false;

   
    private GameObject endCap;
    // Start is called before the first frame update
   void Awake() {
       welcome = GameObject.FindGameObjectWithTag("welcome_Sound").GetComponent<AudioSource>();
       swipingInstructions = GameObject.FindGameObjectWithTag("swipingDirections_Sound").GetComponent<AudioSource>();
       controlPanelLocation = GameObject.FindGameObjectWithTag("controlPanelLocation_Sound").GetComponent<AudioSource>();
   }

    
  
    void Start()
    {
         floor = GameObject.FindGameObjectWithTag("floor");

         endCap  =  GameObject.Find("Endcap");

         positioningStatus      =  GameObject.Find("positioningStatus");

         themeStatusHolder  =  GameObject.Find("themeStatusHolder");
        

         previousThemeTag = themeStatusHolder.tag;
         ballKilled=false;
         pinsKilled=false;

         handleIntroSoundsOnDifferentThread();
         start_Fresh_Game();
    }

    void kill_Ball(GameObject go){
       try {
           Destroy(go);
       }
       catch(Exception e) {
          Debug.Log("Error deleting ball");
       }
    }

void kill_Pin(GameObject go){
       try {
           if (taggedAsDeleted(go) ){
              
              Destroy(go);
           }//if   
       }
       catch(Exception e) {
          Debug.Log("Error deleting pin");
       }
    }

    void tagAsDeleted(GameObject pin) {
        pin.tag="deleted_pin";   
    }

    bool taggedAsDeleted(GameObject pin) {
       return pin.tag=="deleted_pin";   
    }

    void killPins_If_Falling() {
         GameObject[] pins = GameObject.FindGameObjectsWithTag("bowlingPin");
         foreach (GameObject pin in pins)
         {
            if ( falling(pin) ){
               //Debug.Log("Deleting " + pin.name);
               tagAsDeleted(pin);
            }//if  
            // else {
            //    // Debug.Log(pin.name + " is not falling");
            // } 
         }

         foreach (GameObject pin in pins)
         {
            if (taggedAsDeleted(pin))
               kill_Pin(pin);
         }

         handleRackLogicOnDifferentThread();

    }

    void killBall_If_Falling(){

        bowlingBall = GameObject.FindGameObjectWithTag("bowlingBall");
        if (bowlingBall != null) {
          if ( falling(bowlingBall) ) {
            // Debug.Log("Killing ball");
             kill_Ball(bowlingBall);
             handleBallLogicOnDifferentThread();
          }
        }

    }

    void simply_Kill_Ball() {
       // delete bowling ball
       bowlingBall = GameObject.FindGameObjectWithTag("bowlingBall");
        if (bowlingBall != null) {
             kill_Ball(bowlingBall);    
        }  
        ballKilled = true; 
    }

    void simply_Kill_All_Pins() {
        GameObject[] pins = GameObject.FindGameObjectsWithTag("bowlingPin");
         foreach (GameObject pin in pins)
         {
            tagAsDeleted(pin);
          
         }

         foreach (GameObject pin in pins)
         {
            if (taggedAsDeleted(pin))
               kill_Pin(pin);
         }

         pinsKilled = true;
    }

    // Update is called once per frame
    void Update()
    {

   //   // handle change of theme
   //   if (previousThemeTag != themeStatusHolder.tag){
   //         previousThemeTag = themeStatusHolder.tag;
   //         simply_Kill_Ball();
   //         return;
   //   }//if

    //positioning_off_need_game_restart  -> have config menu apply put into this mode. then this class respawns rack and ball
       if (positioningStatus.tag == "positioning_off"){
              //when alley positioning mode is off, perform regular duties
              killBall_If_Falling();
              killPins_If_Falling();
       }
       else if (positioningStatus.tag == "positioning_off_need_game_restart" ) {
            // the user has just pressed DONE on the positioning menu, so 
            // we should spawn a new rack, and spawn a new ball

            positioningStatus.tag = "positioning_off";
            
            ballKilled=false;
            pinsKilled=false;
            
            start_Fresh_Game();
       }
       else {
          // user is busy positioning the bowling alley
          // make certain the ball and the rack don't exist

          // positioning_on_x , y, z tags...

        if (!ballKilled)  
          simply_Kill_Ball();

        if (!pinsKilled)  
           simply_Kill_All_Pins();

       }// user is positioning the bowling alley

    
    }


     bool falling(GameObject go) {

      bool value = false;

        
      if ( isTieFighter_BowlingBall(go) || isXWingFighter_BowlingBall(go) ){

        // TextMesh  debugText = GameObject.Find("debugText").GetComponent<TextMesh>();
         //debugText.text = "working..." ;

         //debugText.text = "is Tie Fighter A..." ;
        

          // when thrown, a tie fighter only travels on Z access with force...so it never falls per se.
          // just figure out whether it is behind the back wall on the Z
          
          float backWall_z = endCap.transform.position.z;
          //backWall_z+=10;
           //debugText.text = "is Tie Fighter B..." ;

          float go_z = go.transform.position.z;
           //debugText.text = "is Tie Fighter C..." ;
           value = ( go_z > backWall_z);

           //debugText.text = "is Tie Fighter D..." ;

           //debugText.text = "tie went past wall" ;
           
      }
      else {
        float floor_y = floor.transform.position.y ;
        float go_y = go.transform.position.y;
        value = ( go_y < floor_y);
      }
      
      return value;

     
    }


         void spawnNewBall() {
            // recreating ball
            GameObject ballSpawner = GameObject.FindGameObjectWithTag("ballSpawner");
            float x = ballSpawner.transform.position.x;
            float y = ballSpawner.transform.position.y;
            float z = ballSpawner.transform.position.z;

            GameObject go;

            if (themeStatusHolder.tag == "theme_default") {
                go = (GameObject)  Instantiate(defaultBowlingBallPrefab, new Vector3(x,y,z), Quaternion.identity);
            }
            else if (themeStatusHolder.tag == "theme_starwars_empire") {
                go = (GameObject)  Instantiate(empireBowlingBallPrefab, new Vector3(x,y,z), Quaternion.identity);
            }
            else if (themeStatusHolder.tag == "theme_starwars_rebels") {                                 
                go = (GameObject)  Instantiate(rebelsBowlingBallPrefab, new Vector3(x,y,z), Quaternion.identity);
            }
            
     }

    void start_Fresh_Game() {
        handleBallLogicOnDifferentThread();
        handleRackLogicOnDifferentThread();
    }

    void handleRackLogicOnDifferentThread() {

        if (!performingRackLogic){
           performingRackLogic = true;
           StartCoroutine(DoRackWork());
        }//if

     }

    void handleBallLogicOnDifferentThread() {

        if (!performingBallLogic){
           performingBallLogic=true;
          StartCoroutine(DoBallWork());
        }
        

     }

   void handleIntroSoundsOnDifferentThread() {

        if (playIntroSounds)
           StartCoroutine(PlayIntroSounds());

     }

IEnumerator DoBallWork()
    {
        
        //Debug.Log("Started DoWork : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(pauseBeforeDroppingBall);

        //After we have waited 5 seconds print the time again.
        //Debug.Log("Finished pausing in DoWork : " + Time.time);

        whatsNextForBall();

        //Debug.Log("Finished DoWork : " + Time.time);
    }
IEnumerator DoRackWork()
    {
        
        //Debug.Log("Started DoWork : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(pauseBeforeDroppingRack);

        //After we have waited 5 seconds print the time again.
        //Debug.Log("Finished pausing in DoWork : " + Time.time);

        whatsNextForRack();

        //Debug.Log("Finished DoWork : " + Time.time);
    }

     void whatsNextForBall() {
      
        spawnNewBall();

        performingBallLogic = false;
     }

   void whatsNextForRack() {

        destroyPinsThatAreNotVertical();
        
        if (!anyPinsLeft()) {
            //Debug.Log("About to spawn new rack");
           spawnNewRack();
        }

        performingRackLogic = false;
        
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
            
                tagAsDeleted(pin);
                kill_Pin(pin);
                
           }
       
    

         }
 }


     bool anyPinsLeft() {
        bool pinsLeft=false;

        GameObject[] pins = GameObject.FindGameObjectsWithTag("bowlingPin");
        
        pinsLeft = pins.Length>0;

       
        return pinsLeft;

     }

     void spawnNewRack() {
            // recreating ball
            GameObject rackSpawner = GameObject.FindGameObjectWithTag("rackSpawner");
            float x = rackSpawner.transform.position.x;
            float y = rackSpawner.transform.position.y;
            float z = rackSpawner.transform.position.z;
            GameObject go = (GameObject)  Instantiate(rackPrefab, new Vector3(x,y,z), Quaternion.identity);
     }


     
  
   IEnumerator PlayIntroSounds()
    {
      welcome.Play();
      yield return new WaitForSeconds(welcome.clip.length); 
      
      swipingInstructions.Play(); 
      yield return new WaitForSeconds(swipingInstructions.clip.length);
     
       controlPanelLocation.Play();
       yield return new WaitForSeconds(controlPanelLocation.clip.length); 
       

    }




    bool isXWingFighter_BowlingBall(GameObject ball) {

      bool value = false;
      if (ball!=null){
         GameObject xWingItem = PrefabFactory.GetChildWithName(ball,"xwingfighterBack");
         value = (xWingItem !=null);
      }

       return value;
    }


    bool isTieFighter_BowlingBall(GameObject ball) {

      bool value = false;
      if (ball!=null){
         GameObject tieWing = PrefabFactory.GetChildWithName(ball,"tieWing1");
         value = (tieWing !=null);
      }

       return value;
    }

}
