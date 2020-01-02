using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorPositioningBehaviour : MonoBehaviour
{

    GameObject positioningStatus;
    
    public GameObject positioningMenu;
    public GameObject quitDialog;

   private GameObject themeStatusHolder;

    private GameObject frikkingAnnoying3DLabel;
    private GameObject positioningConfigIcon;
    private GameObject quitIcon;
    private GameObject imperialTheme;
    private GameObject rebelTheme;
    private GameObject defaultTheme;

    private  AudioSource goodbye;
     private  AudioSource xyzPositioning;


// debug 

//private  AudioSource defaultThemeSound;
 //private AudioSource empireThemeSound;


    void Awake() {
      goodbye = GameObject.FindGameObjectWithTag("goodbye_Sound").GetComponent<AudioSource>();  
       xyzPositioning = GameObject.FindGameObjectWithTag("xyzPositioning_Sound").GetComponent<AudioSource>(); 
       //debug
        //defaultThemeSound = GameObject.FindGameObjectWithTag("rollingBall_Sound").GetComponent<AudioSource>();
        // empireThemeSound = GameObject.FindGameObjectWithTag("hitByBall_Sound").GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        positioningStatus      =  GameObject.Find("positioningStatus");
        frikkingAnnoying3DLabel      =  GameObject.Find("configPanelLabel");

        positioningConfigIcon      =  GameObject.Find("positioningConfigIcon");
        quitIcon      =  GameObject.Find("quitIcon");
        imperialTheme      =  GameObject.Find("imperialTheme");
        rebelTheme      =  GameObject.Find("rebelTheme");
        defaultTheme      =  GameObject.Find("defaultTheme");


        themeStatusHolder  =  GameObject.Find("themeStatusHolder");
       
    }

    // Update is called once per frame
    void Update()
    {
     handleInputAndroid();   
    }


void handleInputAndroid(){

     int nbTouches = Input.touchCount;
 
      if ( nbTouches > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
      {

           
           Touch touch = Input.GetTouch(0);
           Ray ray = Camera.main.ScreenPointToRay(touch.position);
 
           RaycastHit hit;
             
            if (Physics.Raycast(ray,out hit, 100.0f)) {
                if (hit.transform && hit.transform.gameObject){

                    
                   
                    string name = hit.collider.transform.gameObject.name;

                    if (name == "positioningConfigIcon"){
                            // use touched floor with 2 fingers - to bring up floor config mode
                         
                         positioningStatus.tag = "positioning_on"; //global status

                        
                         activatePositioningMenu();
                            
                    }
                    else if (name == "posX") {
                      positioningStatus.tag = "positioning_on_x"; //global status
                    }
                    else if (name == "posY") {
                      positioningStatus.tag = "positioning_on_y"; //global status
                    }
                    else if (name == "posZ") {
                      positioningStatus.tag = "positioning_on_z"; //global status
                    }
                    else if (name == "posDone") {
                     
                     positioningStatus.tag = "positioning_off_need_game_restart"; //global status
                      deactivatePositioningMenu();
                    }
                    else if (name == "quitIcon"){
                     // confirm 
                     activateQuitDialog();

                    } 
                    else if (name == "quitNo") {
                      deactivateQuitDialog();
                    }
                    else if (name == "quitYes") {
                      handleGoodbyeSoundsOnDifferentThread();
                    }
                    else if (name == "defaultTheme") {
                      //defaultThemeSound.Play();
                      themeStatusHolder.tag = "theme_default";
                    }                   
                    else if (name == "imperialTheme") {
                     // empireThemeSound.Play();
                      themeStatusHolder.tag = "theme_starwars_empire";
                    }                   
                    else if (name == "rebelTheme") {
                      themeStatusHolder.tag = "theme_starwars_rebels";
                      // defaultThemeSound.Play();
                      //themeStatusHolder.tag = "theme_default";
                    }


                }//if
            }//if



      }// there was a two finger touch

   }

        void disable_Main_ControlPanel_Buttons() {

           frikkingAnnoying3DLabel.SetActive(false);
           positioningConfigIcon.SetActive(false);
           quitIcon.SetActive(false);
           imperialTheme.SetActive(false);
           rebelTheme.SetActive(false);
           defaultTheme.SetActive(false);

        }
      void enable_Main_ControlPanel_Buttons() {
 
           frikkingAnnoying3DLabel.SetActive(true);
           positioningConfigIcon.SetActive(true);
           quitIcon.SetActive(true);
           imperialTheme.SetActive(true);
           rebelTheme.SetActive(true);
           defaultTheme.SetActive(true);


        }
         void activateQuitDialog() {  
           quitDialog.SetActive(true);
           disable_Main_ControlPanel_Buttons();
     }

       void deactivateQuitDialog() {
           quitDialog.SetActive(false);
           enable_Main_ControlPanel_Buttons();
     }     

         void activatePositioningMenu() {
           positioningMenu.SetActive(true);
           handleXYZPositioningSoundsOnDifferentThread();
           
     }

    void deactivatePositioningMenu() {
            
           positioningMenu.SetActive(false);
     }


    IEnumerator PlayGoodbyeSounds()
    {
      goodbye.Play();
      yield return new WaitForSeconds(goodbye.clip.length); 
      
      deactivateQuitDialog();             
      Application.Quit();

    }     

IEnumerator PlayXYZPositioningSounds()
    {
      xyzPositioning.Play();
      yield return new WaitForSeconds(xyzPositioning.clip.length);   
    } 

    void handleXYZPositioningSoundsOnDifferentThread() {

        StartCoroutine(PlayXYZPositioningSounds());

     }

     void handleGoodbyeSoundsOnDifferentThread() {

        StartCoroutine(PlayGoodbyeSounds());

     }
}
