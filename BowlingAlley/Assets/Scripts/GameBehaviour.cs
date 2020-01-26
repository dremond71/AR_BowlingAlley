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
    private bool allowSpawningOfNewRack = true;
    private bool needToChangeBall = false;
    private bool changingBall = false;
    private bool ballKilled;
    private bool pinsKilled;

    private GameObject themeStatusHolder;
    
    private string previousThemeTag;

    private  AudioSource welcome;
    private  AudioSource swipingInstructions;
    private  AudioSource controlPanelLocation;
    private bool playIntroSounds = false;
  private TextMesh debugText; 
   
    private GameObject endCap;

    

   
    // Start is called before the first frame update
   void Awake() {
       welcome = GameObject.FindGameObjectWithTag("welcome_Sound").GetComponent<AudioSource>();
       swipingInstructions = GameObject.FindGameObjectWithTag("swipingDirections_Sound").GetComponent<AudioSource>();
       controlPanelLocation = GameObject.FindGameObjectWithTag("controlPanelLocation_Sound").GetComponent<AudioSource>();
        debugText =  GameObject.Find("debugText").GetComponent<TextMesh>();
       
       
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

         //MyDebug("allowSpawningOfNewRack: " + allowSpawningOfNewRack);
         //handleIntroSoundsOnDifferentThread();
         start_Fresh_Game(false);
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

       //after killing a pin, check if a new rack is required
       
       
    }

    void tagAsDeleted(GameObject pin) {

       try {
            pin.tag="deleted_pin"; 
       }
       catch(Exception e) {
          Debug.Log("Error tagging pin");
       } 
         
    }


    bool taggedAsDeleted(GameObject pin) {
       bool value = false;

      try {
            value = (pin.tag=="deleted_pin"); 
       }
       catch(Exception e) {
          Debug.Log("Error tagging pin");
       } 
 
       return value;   
    }

    void killPins_If_Falling() {

         // we should kill falling pins immediately otherwise
         // you will see them falling below the bowling alley

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

       

    }

    void killBall_If_Falling(){

        bowlingBall = GameObject.FindGameObjectWithTag("bowlingBall");
        if (bowlingBall != null) {
          if ( falling(bowlingBall) ) {
            // Debug.Log("Killing ball");
             kill_Ball(bowlingBall);
             handleBallLogicOnDifferentThread(false);
          }
        }

    }

    void kill_Ball_With_NullCheck() {
       // delete bowling ball
       bowlingBall = GameObject.FindGameObjectWithTag("bowlingBall");
        if (bowlingBall != null) {
             kill_Ball(bowlingBall);    
        }  
    } 
    void simply_Kill_Ball() {
        kill_Ball_With_NullCheck();
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
     //
     // detect change of theme
     //
     if (previousThemeTag != themeStatusHolder.tag){
           previousThemeTag = themeStatusHolder.tag;
           needToChangeBall = true;      
           return;
     }//if


     //
     // handle the changing of ball - normal processing won't continue until this work is complete
     //
     if (needToChangeBall){

        if (!changingBall) {
          changingBall = true;

          //call work to change ball
          startWorkToChangeBall();
          return;
        }
        return;
     }


     //
     // Normal processing
     //

    //positioning_off_need_game_restart  -> have config menu apply put into this mode. then this class respawns rack and ball
       if (positioningStatus.tag == "positioning_off"){
              //when alley positioning mode is off, perform regular duties
              killBall_If_Falling();
               
              killPins_If_Falling();
              killPins_If_NotVertical();
              spawnNewRack_If_Necessary();
       }
       else if (positioningStatus.tag == "positioning_off_need_game_restart" ) {
            // the user has just pressed DONE on the positioning menu, so 
            // we should spawn a new rack, and spawn a new ball

            positioningStatus.tag = "positioning_off";
            
            ballKilled=false;
            pinsKilled=false;
            allowSpawningOfNewRack = true;
           // MyDebug("allowSpawningOfNewRack: " + allowSpawningOfNewRack);
            start_Fresh_Game(true);
       }
       else if (positioningStatus.tag == "positioning_on"){
      
           // user turned on the bowling alley positioning menu
           // make certain the ball and the rack don't exist 
           ballKilled = false;   
           pinsKilled=false;
           allowSpawningOfNewRack = false;
           //MyDebug("allowSpawningOfNewRack: " + allowSpawningOfNewRack);
           simplyKillBallAndPins();

       }
       else {
          // user is busy positioning the bowling alley
          // make certain the ball and the rack don't exist

          // positioning_on_x , y, z tags...
          simplyKillBallAndPins();
     

       }// user is positioning the bowling alley

    
    }

     void simplyKillBallAndPins() {

        if (!ballKilled)  
          simply_Kill_Ball();

        if (!pinsKilled)  
           simply_Kill_All_Pins();

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

    void startWorkToChangeBall() {
       
          kill_Ball_With_NullCheck();
          bool reset_ChangingBall_Flags_When_Complete = true;
          handleBallLogicOnDifferentThread(reset_ChangingBall_Flags_When_Complete);
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

    void start_Fresh_Game(bool alsoPerformRackLogic) {
        handleBallLogicOnDifferentThread(false);
        if (alsoPerformRackLogic)
           handleRackLogicOnDifferentThread();
    }

    // function that starts a thread to take care of deleting a pin
    // that is down, but hasn't rolled off the bowling lane
    void handleDeleteNonVerticalPinOnDifferentThread(GameObject pin) {
        StartCoroutine(DeleteNonVerticalPinWork(pin));
    }


    IEnumerator DeleteNonVerticalPinWork(GameObject pin)
    {
        
        //Debug.Log("Started DoWork : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(3.0f);
       tagAsDeleted(pin);
       kill_Pin(pin);


        //Debug.Log("Finished DoWork : " + Time.time);
    }
    void handleRackLogicOnDifferentThread() {

       if (!performingRackLogic){
         performingRackLogic = true;
         StartCoroutine(DoRackWork());
       }//if
        
     }

    void handleBallLogicOnDifferentThread(bool reset_ChangingBall_Flags_When_Complete) {

        if (!performingBallLogic){
           performingBallLogic=true;
          StartCoroutine(DoBallWork(reset_ChangingBall_Flags_When_Complete));
        }
        

     }

   void handleIntroSoundsOnDifferentThread() {

        if (playIntroSounds)
           StartCoroutine(PlayIntroSounds());

     }

IEnumerator DoBallWork(bool reset_ChangingBall_Flags_When_Complete)
    {
        
        //Debug.Log("Started DoWork : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(pauseBeforeDroppingBall);

        //After we have waited 5 seconds print the time again.
        //Debug.Log("Finished pausing in DoWork : " + Time.time);

        spawnNewBall();

        performingBallLogic = false;

        // this will allow normal processing to resume in Update() method
        if (reset_ChangingBall_Flags_When_Complete){
            changingBall = false;
            needToChangeBall = false;
        }
        //Debug.Log("Finished DoWork : " + Time.time);
    }
IEnumerator DoRackWork()
    {
        
        //Debug.Log("Started DoWork : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(pauseBeforeDroppingRack);

        //After we have waited 5 seconds print the time again.
        //Debug.Log("Finished pausing in DoWork : " + Time.time);

        //whatsNextForRack();
        //spawnNewRack_If_Necessary();
        //Debug.Log("Finished DoWork : " + Time.time);
        spawnNewRack();
        performingRackLogic = false;
 
    }

     void whatsNextForBall() {
      
    
     }

   // called from kill_Pin().
   void spawnNewRack_If_Necessary() {
         if (!anyPinsLeft()) {
         
           if (allowSpawningOfNewRack){
              handleRackLogicOnDifferentThread();
           }
        }

   }
   

 long getCurrentTimeInMilliseconds() {

    return DateTimeOffset.Now.ToUnixTimeMilliseconds();
 }

 void killPins_If_NotVertical() {

         GameObject[] pins = GameObject.FindGameObjectsWithTag("bowlingPin");
         foreach (GameObject pin in pins)
         {

           float z = Mathf.Abs(pin.transform.eulerAngles.z );
           float y = Mathf.Abs(pin.transform.eulerAngles.y);

           if (  ( z > 10) || 
                 ( y > 10)
              )
           {
            
               handleDeleteNonVerticalPinOnDifferentThread(pin);
                
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


     void MyDebug(string someText) {
     
      if (debugText != null){
          debugText.text = someText;
      }

  }
}
