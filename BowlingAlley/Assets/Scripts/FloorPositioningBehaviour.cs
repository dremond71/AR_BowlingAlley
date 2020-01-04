using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    private GameObject themeLabel;
    private GameObject configXYZLabel;
    private GameObject quitLabel;

    private GameObject defaultChecked;
    private GameObject imperialChecked;
    private GameObject rebelsChecked;

    private  AudioSource goodbye;
     private  AudioSource xyzPositioning;
     private TextMesh debugText; 
     private  AudioSource buttonClickSource;
     private AudioClip buttonClick;




    void Awake() {
      goodbye = GameObject.FindGameObjectWithTag("goodbye_Sound").GetComponent<AudioSource>();  
       xyzPositioning = GameObject.FindGameObjectWithTag("xyzPositioning_Sound").GetComponent<AudioSource>(); 
       debugText =  GameObject.Find("debugText").GetComponent<TextMesh>();

        buttonClickSource=  GameObject.FindGameObjectWithTag("buttonClick_Sound").GetComponent<AudioSource>();
        buttonClick = buttonClickSource.clip;   
    }

    // Start is called before the first frame update
    void Start()
    {
        // MyDebug("working...");
        positioningStatus      =  GameObject.Find("positioningStatus");
        frikkingAnnoying3DLabel      =  GameObject.Find("configPanelLabel");

        positioningConfigIcon      =  GameObject.Find("positioningConfigIcon");
        quitIcon      =  GameObject.Find("quitIcon");
        imperialTheme      =  GameObject.Find("imperialTheme");
        rebelTheme      =  GameObject.Find("rebelTheme");
        defaultTheme      =  GameObject.Find("defaultTheme");

        themeLabel      =  GameObject.Find("themeLabel");
        
        configXYZLabel      =  GameObject.Find("configXYZLabel");
        

        quitLabel      =  GameObject.Find("quitLabel");
        

        defaultChecked      =  GameObject.Find("defaultChecked");
        imperialChecked      =  GameObject.Find("imperialChecked");
        rebelsChecked      =  GameObject.Find("rebelsChecked");

  
        themeStatusHolder  =  GameObject.Find("themeStatusHolder");
       
       defaultTheme_Selection();// we aren't storing settings yet, so default theme will always be first.
    }

    // Update is called once per frame
    void Update()
    {
     handleInputAndroid();   
    }

void setActive_withTryCatch(GameObject go, bool value) {
  //not sure why some of the non-null objects cause an exception when I turn active on or off. weird.
  try {    
    go.SetActive(value); 
  }
  catch (Exception e) {
    Debug.LogException(e, this);
  }
}
void defaultTheme_Selection() {
  
   //MyDebug("defaultTheme_Selection - 1");
   setActive_withTryCatch(defaultChecked,true);
   //MyDebug("defaultTheme_Selection - 2");  
   setActive_withTryCatch(imperialChecked,false); 
   //MyDebug("defaultTheme_Selection - 3");    
   setActive_withTryCatch(rebelsChecked,false);
   //MyDebug("defaultTheme_Selection - 4");    
   themeStatusHolder.tag = "theme_default";
   //MyDebug("defaultTheme_Selection - end");
}

void imperialTheme_Selection() {
   
  // MyDebug("imperialTheme_Selection - 1");
    setActive_withTryCatch(defaultChecked,false);
    //MyDebug("imperialTheme_Selection - 2");
   setActive_withTryCatch(imperialChecked,true); 
    //MyDebug("imperialTheme_Selection - 3");
    setActive_withTryCatch(rebelsChecked,false);
    //MyDebug("imperialTheme_Selection - 4");
    themeStatusHolder.tag = "theme_starwars_empire";
    //MyDebug("imperialTheme_Selection - end");
}

void rebelsTheme_Selection() {
  
  //MyDebug("rebelsTheme_Selection - 1");
    setActive_withTryCatch(defaultChecked,false);
    //MyDebug("rebelsTheme_Selection - 2");
    setActive_withTryCatch(imperialChecked,false);  
    //MyDebug("rebelsTheme_Selection - 3");
    setActive_withTryCatch(rebelsChecked,true);
    //MyDebug("rebelsTheme_Selection - 4");
    themeStatusHolder.tag = "theme_starwars_rebels";
   // MyDebug("rebelsTheme_Selection - end");
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

                         PlayButtonClickSound_Immediately();

                         activatePositioningMenu();
                            
                    }
                    else if (name == "posX") {
                      positioningStatus.tag = "positioning_on_x"; //global status
                      PlayButtonClickSound_Immediately();
                    }
                    else if (name == "posY") {
                      positioningStatus.tag = "positioning_on_y"; //global status
                      PlayButtonClickSound_Immediately();
                    }
                    else if (name == "posZ") {
                      positioningStatus.tag = "positioning_on_z"; //global status
                      PlayButtonClickSound_Immediately();
                    }
                    else if (name == "posDone") {
                     
                     positioningStatus.tag = "positioning_off_need_game_restart"; //global status
                     PlayButtonClickSound_Immediately();
                      deactivatePositioningMenu();
                    }
                    else if (name == "quitIcon"){
                      PlayButtonClickSound_Immediately();
                     // confirm 
                     activateQuitDialog();

                    } 
                    else if (name == "quitNo") {
                      PlayButtonClickSound_Immediately();
                      deactivateQuitDialog();
                    }
                    else if (name == "quitYes") {
                      PlayButtonClickSound_Immediately();
                      handleGoodbyeSoundsOnDifferentThread();
                    }
                    else if (name == "defaultTheme") {
                      PlayButtonClickSound_Immediately();
                     // MyDebug("you selected default theme");
                      defaultTheme_Selection();
                      
                    }                   
                    else if (name == "imperialTheme") {
                      PlayButtonClickSound_Immediately();
                     // MyDebug("you selected imperial theme");
                     imperialTheme_Selection();
                      
                    }                   
                    else if (name == "rebelTheme") {
                      PlayButtonClickSound_Immediately();
                      // MyDebug("you selected rebel theme");
                     rebelsTheme_Selection();
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
           themeLabel.SetActive(false);
           configXYZLabel.SetActive(false);
           quitLabel.SetActive(false);
        }
      void enable_Main_ControlPanel_Buttons() {
 
           frikkingAnnoying3DLabel.SetActive(true);
           positioningConfigIcon.SetActive(true);
           quitIcon.SetActive(true);
           imperialTheme.SetActive(true);
           rebelTheme.SetActive(true);
           defaultTheme.SetActive(true);
          themeLabel.SetActive(true);
           configXYZLabel.SetActive(true);
           quitLabel.SetActive(true);

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
          // handleXYZPositioningSoundsOnDifferentThread();
           
     }

    void deactivatePositioningMenu() {
            
           positioningMenu.SetActive(false);
     }

void PlayButtonClickSound_Immediately()
    {
     
      buttonClickSource.PlayOneShot(buttonClick,0.7f);
     
     
    }

    IEnumerator PlayGoodbyeSounds()
    {
      //goodbye.Play();
      //yield return new WaitForSeconds(goodbye.clip.length); 
      yield return new WaitForSeconds(1.0f);
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


      void MyDebug(string someText) {
     
      if (debugText != null){
          debugText.text = someText;
      }

  }
}
