using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ControlPanelShowcase : MonoBehaviour
{

    GameObject positioningStatus;
    
    public GameObject positioningMenu;
    public GameObject quitDialog;


    private GameObject frikkingAnnoying3DLabel;
    private GameObject positioningConfigIcon;
    private GameObject quitIcon;
    private GameObject configXYZLabel;
    private GameObject quitLabel;


     private TextMesh debugText; 

     // xwing stuff
     private  AudioSource roarSource;
     private AudioClip roar;
     
     private AudioSource blasterSource;
     private AudioClip blaster;
      bool roarSoundIsPlaying = false;
      bool red5GoingInSoundIsPlaying = false;
      bool lostR2SoundIsPlaying = false;

    private GameObject xwingBlastOrigin1;
     private GameObject xwingBlastOrigin2;
     private GameObject xwingBlastOrigin3;
     private GameObject xwingBlastOrigin4;
     private GameObject xwingBlastPrefab;
     private float tieBlastForce = 1000.0f;

     private  AudioSource r2d2Source;
     private AudioClip r2d2;
     private  AudioSource lukeSource;
     private AudioClip luke;
     private  AudioSource lostR2D2Source;
     private AudioClip lostR2D2;

     // xwing stuff

     private  AudioSource buttonClickSource;
     private AudioClip buttonClick;

// found that my xwing was being clipped (parts disappearing) when I walked close to it.
// probably because I made it huge.
// found this article on setting 'Far' setting on AR camera : https://answers.unity.com/questions/157604/objects-disappear-when-too-far-away.html


    void Awake() {
     
      
       debugText =  GameObject.Find("debugText").GetComponent<TextMesh>();
    }

    // Start is called before the first frame update
    void Start()
    {
        MyDebug("working...");
        positioningStatus      =  GameObject.Find("positioningStatus");
        
        frikkingAnnoying3DLabel      =  GameObject.Find("configPanelLabel");

        positioningConfigIcon      =  GameObject.Find("positioningConfigIcon");
        quitIcon      =  GameObject.Find("quitIcon");
 
        
        configXYZLabel      =  GameObject.Find("configXYZLabel");
        

        quitLabel      =  GameObject.Find("quitLabel");
        

          blasterSource = GameObject.FindGameObjectWithTag("xwingFighterBlasts_Sound").GetComponent<AudioSource>();
        blaster = blasterSource.clip;

         roarSource =  GameObject.FindGameObjectWithTag("xwingFighterRoar_Sound").GetComponent<AudioSource>();
         roar = roarSource.clip;   

          xwingBlastPrefab = PrefabFactory.getPrefab("xwingBlast2");  

         r2d2Source=  GameObject.FindGameObjectWithTag("r2d2_Sound").GetComponent<AudioSource>();
         r2d2 = r2d2Source.clip;   

         lukeSource=  GameObject.FindGameObjectWithTag("red5GoingIn_Sound").GetComponent<AudioSource>();
         luke = lukeSource.clip;   

        lostR2D2Source=  GameObject.FindGameObjectWithTag("lostr2d2_Sound").GetComponent<AudioSource>();
        lostR2D2 = lostR2D2Source.clip;   
        //reportOnObject(lostR2D2Source,"lostR2D2Source");
          
        buttonClickSource=  GameObject.FindGameObjectWithTag("buttonClick_Sound").GetComponent<AudioSource>();
        buttonClick = buttonClickSource.clip;   
          
    }

    void reportOnObject(GameObject go, string name) {
        if (go == null) {
            MyDebug(name + " : is null");
        }
        else {
            MyDebug(name + " : is not null");
        }
    }
  void reportOnObject(AudioSource go, string name) {
        if (go == null) {
            MyDebug(name + " : is null");
        }
        else {
            MyDebug(name + " : is not null");
        }
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

void handleInputAndroid(){

     int nbTouches = Input.touchCount;
 
      if ( nbTouches > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
      {

           //MyDebug("touched something");
           Touch touch = Input.GetTouch(0);
           Ray ray = Camera.main.ScreenPointToRay(touch.position);
 
           RaycastHit hit;
             
            if (Physics.Raycast(ray,out hit, 100.0f)) {
                if (hit.transform && hit.transform.gameObject){

                    
                   
                    string name = hit.collider.transform.gameObject.name;
                     //MyDebug("touched '" + name + "'");
                    if (name == "positioningConfigIcon"){
                            // use touched floor with 2 fingers - to bring up floor config mode
                        // MyDebug("touch config icon");
                         positioningStatus.tag = "positioning_on"; //global status
                         PlayButtonClickSound_Immediately();

                        
                         activatePositioningMenu();
                            
                    }
                    else if (name == "posX") {
                       // MyDebug("touch x icon");
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
                    else if (name == "scale") {
                      positioningStatus.tag = "positioning_on_scale"; //global status
                      PlayButtonClickSound_Immediately();
                    }
                    else if (name == "rotate") {
                      positioningStatus.tag = "positioning_on_rotate"; //global status
                      PlayButtonClickSound_Immediately();
                    }
                    else if (name == "posDone") {
                     //MyDebug("touch done icon");
                     positioningStatus.tag = "positioning_off_need_game_restart"; //global status
                     PlayButtonClickSound_Immediately();
                      deactivatePositioningMenu();
                    }
                    else if (name == "quitIcon"){
                     // confirm 
                     //MyDebug("touch quit icon");
                     PlayButtonClickSound_Immediately();
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
                    else if ( isABlaster(name) || isAWing(name)  ) {
                     // MyDebug("touched blaster");
                        shoot();
                    }
                    else if ( isCockpitWindow(name) ) {
                          if (!red5GoingInSoundIsPlaying)
                           playLukeSoundOnDifferentThread();
                    }
                    else if ( isR2D2(name) ) {
                         PlayR2D2Sound_Immediately();
                    }
                    else if ( isBehindR2D2(name) ) {
                          if (!lostR2SoundIsPlaying)
                           playLostR2OnDifferentThread();
                    }
                    else if ( isAnEngine(name) ) {
                         if (!roarSoundIsPlaying)
                           playRoarOnDifferentThread();
                    }
                }//if
            }//if


      }// there was a two finger touch

   }

        void disable_Main_ControlPanel_Buttons() {

           frikkingAnnoying3DLabel.SetActive(false);
           positioningConfigIcon.SetActive(false);
           quitIcon.SetActive(false);
           configXYZLabel.SetActive(false);
           quitLabel.SetActive(false);
        }
      void enable_Main_ControlPanel_Buttons() {
 
           frikkingAnnoying3DLabel.SetActive(true);
           positioningConfigIcon.SetActive(true);
           quitIcon.SetActive(true);
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


    IEnumerator PlayGoodbyeSounds()
    {
       
      yield return new WaitForSeconds(1.0f);
      deactivateQuitDialog();             
      Application.Quit();

    }     

IEnumerator PlayXYZPositioningSounds()
    {
      
      yield return new WaitForSeconds(1.0f);   
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



      void spawnNewBlasterBolt() {
            // recreating ball
            
 
            // bolt 1
            xwingBlastOrigin1 =  GameObject.FindGameObjectWithTag("xwingBlasterPosition1");
            reportOnObject(xwingBlastOrigin1,"xwingBlastOrigin1");
            float x = xwingBlastOrigin1.transform.position.x;
            float y = xwingBlastOrigin1.transform.position.y;
            float z = xwingBlastOrigin1.transform.position.z;
            GameObject go = (GameObject)  Instantiate(xwingBlastPrefab, new Vector3(x,y,z), Quaternion.identity);
            reportOnObject(go,"go1");
            GameObject bolt1 = PrefabFactory.GetChildWithName(go,"xwingBlastTwo");
           reportOnObject(bolt1,"bolt1");
            // bolt 2
            xwingBlastOrigin2 =  GameObject.FindGameObjectWithTag("xwingBlasterPosition2");
            x = xwingBlastOrigin2.transform.position.x;
            y = xwingBlastOrigin2.transform.position.y;
            z = xwingBlastOrigin2.transform.position.z;
            go = (GameObject)  Instantiate(xwingBlastPrefab, new Vector3(x,y,z), Quaternion.identity);
            reportOnObject(go,"go2");
            GameObject bolt2 = PrefabFactory.GetChildWithName(go,"xwingBlastTwo");
            reportOnObject(bolt2,"bolt2");

            // bolt 3
            xwingBlastOrigin3 =  GameObject.FindGameObjectWithTag("xwingBlasterPosition3");
            x = xwingBlastOrigin3.transform.position.x;
            y = xwingBlastOrigin3.transform.position.y;
            z = xwingBlastOrigin3.transform.position.z;
            go = (GameObject)  Instantiate(xwingBlastPrefab, new Vector3(x,y,z), Quaternion.identity);
            reportOnObject(go,"go3");
            GameObject bolt3 = PrefabFactory.GetChildWithName(go,"xwingBlastTwo");
            reportOnObject(bolt3,"bolt3");

            // bolt 4
            xwingBlastOrigin4 =  GameObject.FindGameObjectWithTag("xwingBlasterPosition4");
            x = xwingBlastOrigin4.transform.position.x;
            y = xwingBlastOrigin4.transform.position.y;
            z = xwingBlastOrigin4.transform.position.z;
            go = (GameObject)  Instantiate(xwingBlastPrefab, new Vector3(x,y,z), Quaternion.identity);
            reportOnObject(go,"go4");
            GameObject bolt4 = PrefabFactory.GetChildWithName(go,"xwingBlastTwo");
            reportOnObject(bolt4,"bolt4");

           // if tie fighter is rolling down the alley, add ITS force to the force of the bolts; to keep bolts ahead of tie fighter.
           float blasterForce = tieBlastForce * -1;
          

            bolt1.GetComponent<Rigidbody>().AddForce(0,0,blasterForce);
            bolt2.GetComponent<Rigidbody>().AddForce(0,0,blasterForce);
            bolt3.GetComponent<Rigidbody>().AddForce(0,0,blasterForce);
            bolt4.GetComponent<Rigidbody>().AddForce(0,0,blasterForce);        
  
     }


void playLukeSoundOnDifferentThread() {

        StartCoroutine(PlayLukeSound());

     }

void playLostR2OnDifferentThread() {

        StartCoroutine(PlayLostR2Sound());

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

IEnumerator PlayLostR2Sound()
    {
        //https://answers.unity.com/questions/904981/how-to-play-an-audio-file-after-another-finishes.html
        lostR2SoundIsPlaying = true;
        lostR2D2Source.PlayOneShot(lostR2D2,0.7f);
        yield return new WaitForSeconds(lostR2D2.length);
        lostR2SoundIsPlaying = false;

    }

IEnumerator PlayLukeSound()
    {
        //https://answers.unity.com/questions/904981/how-to-play-an-audio-file-after-another-finishes.html
        red5GoingInSoundIsPlaying = true;
        lukeSource.PlayOneShot(luke,0.7f);
        yield return new WaitForSeconds(luke.length);
        red5GoingInSoundIsPlaying = false;

    }

bool isCockpitWindow(string name) {
  bool value = false;

   if ( (name == "cockpitWindow") || (name == "cockpitRightWindow1") || (name == "cockpitRightWindow2") || (name == "cockpitRightWindow3") ||
        (name == "cockpitLeftWindow1") || (name == "cockpitLeftWindow2") || (name == "cockpitLeftWindow2")
      ) {
        value = true;
      }

  return value;
}

bool isR2D2(string name) {
  bool value = false;

   if ( name.StartsWith("r2")  
      ) {
        value = true;
      }

  return value;
}


bool isBehindR2D2(string name) {
  bool value = false;

   if ( (name == "xwingTopBehindR2Texture")  
      ) {
        value = true;
      }

  return value;
}

bool isAnEngine(string name) {
  bool value = false;

   if ( (name == "engineTopLeft") || (name == "engineTopLeft2") || (name == "engineBottomLeft") || (name == "engineBottomLeft2") ||
        (name == "engineTopRight") || (name == "engineTopRight2") || (name == "engineBottomRight") || (name == "engineBottomRight2") ||
        (name == "frontEngine1VentTexture") || (name == "backEngine1Texture") ||
        (name == "frontEngine2VentTexture") || (name == "backEngine2Texture") ||
        (name == "frontEngine4VentTexture") || (name == "backEngine4Texture") ||
        (name == "frontEngine3VentTexture") || (name == "backEngine3Texture") || (name == "xwingfighterBackTexture")
      ) {
        value = true;
      }

  return value;
}

bool isAWing(string name) {
  bool value = false;

   if ( (name == "wingLeftTop") || (name == "wingLeftTop2") || 
        (name == "wingLeftBottom") || (name == "wingLeftBottom2") ||
        (name == "wingLeftBottom") || (name == "wingLeftBottom2") ||
        (name == "wingRightBottom") || (name == "wingRightBottom2") || 
        (name == "wingRightTop") || (name == "wingRightTop2") ||
        (name == "xwingTopLeftEmblemTexture") || (name == "xwingBottomLeftEmblemTextureTop") ||                                                      
        (name == "xwingBottomRightEmblemTopTexture") || (name == "xwingTopRightEmblemTexture")
      ) {
        value = true;
      }

  return value;
}

bool isABlaster(string name) {
  bool value = false;

   if ( (name == "blasterLeftTop3") || (name == "blasterLeftTop2") || (name == "blasterLeftTop1") || 
        (name == "blasterLeftBottom3") || (name == "blasterLeftBottom2") || (name == "blasterLeftBottom1") || 
         (name == "blasterRightBottom3") || (name == "blasterRightBottom2") || (name == "blasterRightBottom1") || 
         (name == "blasterRightTop3") || (name == "blasterRightTop2") || (name == "blasterRightTop1")  
      ) {
        value = true;
      }

  return value;
}

void shoot() {
         spawnNewBlasterBolt();
         PlayBlasterSound_Immediately();
    }

void PlayButtonClickSound_Immediately()
    {
     
      buttonClickSource.PlayOneShot(buttonClick,0.7f);
     
     
    }

void PlayBlasterSound_Immediately()
    {
      // we want a seperate object for each fire; so we can handle multi blasts in quick succession
      
      // Method I used to achieve multiple blasts that don't interrupt each other:
      // https://docs.unity3d.com/ScriptReference/AudioSource.PlayOneShot.html
      blasterSource.PlayOneShot(blaster,0.7f);
     
     
    }

void PlayLostR2D2Sound_Immediately()
    {
     
      lostR2D2Source.PlayOneShot(lostR2D2,0.7f);
     
     
    }

void PlayR2D2Sound_Immediately()
    {
     
      r2d2Source.PlayOneShot(r2d2,0.7f);
     
     
    }

void PlayLukeSound_Immediately()
    {
     
      lukeSource.PlayOneShot(luke,0.7f);
     
     
    }


}
