using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARReferencePointManager))]
[RequireComponent(typeof(ARPlaneManager))]
public class ReferencePointManager : MonoBehaviour
{
 
    [SerializeField]
    private GameObject status;

    
    private Button yPlus;
    private Button yMinus;
    private Text yvalueText;

    private Button xPlus;
    private Button xMinus;
    private Text xvalueText;

    private Button zPlus;
    private Button zMinus;
    private Text zvalueText;


    private Text rotationsText;

    private InputField rotXText;
    private InputField rotYText;
    private InputField rotZText;
    private Button rotApplyButton;
    private Button donePositioningButton;

    private ARRaycastManager arRayCastManager;
    private ARReferencePointManager arReferencePointManager;
    private ARPlaneManager arPlaneManager;
    private List<ARReferencePoint> referencePointList = new List<ARReferencePoint>();
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private int fakeY=77;

    private Pose currentPose;
    private ARReferencePoint currentRP=null;


     void Awake()
    {
        arRayCastManager        = GetComponent<ARRaycastManager>();
        arReferencePointManager = GetComponent<ARReferencePointManager>();
        arPlaneManager          = GetComponent<ARPlaneManager>();

        yMinus =  GameObject.Find("YMinus").GetComponent<Button>();
        yPlus =  GameObject.Find("YPlus").GetComponent<Button>();
        yvalueText =  GameObject.Find("yvalueText").GetComponent<Text>();

        xMinus =  GameObject.Find("XMinus").GetComponent<Button>();
        xPlus =  GameObject.Find("XPlus").GetComponent<Button>();
        xvalueText =  GameObject.Find("xvalueText").GetComponent<Text>();

        zMinus =  GameObject.Find("ZMinus").GetComponent<Button>();
        zPlus =  GameObject.Find("ZPlus").GetComponent<Button>();
        zvalueText =  GameObject.Find("zvalueText").GetComponent<Text>();

         rotXText =  GameObject.Find("rotX").GetComponent<InputField>();
         rotYText =  GameObject.Find("rotY").GetComponent<InputField>();
         rotZText =  GameObject.Find("rotZ").GetComponent<InputField>();
         rotationsText = GameObject.Find("rotationsText").GetComponent<Text>();
          rotApplyButton =  GameObject.Find("rotApplyButton").GetComponent<Button>();

          donePositioningButton =  GameObject.Find("donePositioningButton").GetComponent<Button>();


         //toggleButton.onClick.AddListener(TogglePlaneDetection);

          yMinus.onClick.AddListener(decreaseYvalue);
          yPlus.onClick.AddListener(increaseYvalue);

          xMinus.onClick.AddListener(decreaseXvalue);
          xPlus.onClick.AddListener(increaseXvalue);

          zMinus.onClick.AddListener(decreaseZvalue);
          zPlus.onClick.AddListener(increaseZvalue);

           donePositioningButton.onClick.AddListener(handle_DonePositioning_Button);
           rotApplyButton.onClick.AddListener(handle_RotationApply_Button);
          

    }
 
    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void TogglePlaneDetection()
    {
        arPlaneManager.enabled = !arPlaneManager.enabled;

        foreach(ARPlane plane in arPlaneManager.trackables)
        {   
            plane.gameObject.SetActive(arPlaneManager.enabled);
        }
        
        //toggleButton.GetComponentInChildren<Text>().text = arPlaneManager.enabled ? 
        //    "Disable Plane Detection" : "Enable Plane Detection";
    }

    void handle_DonePositioning_Button() {

    }

    void handle_RotationApply_Button() {

          float newX = float.Parse(rotXText.text);
          float newY = float.Parse(rotYText.text);
          float newZ = float.Parse(rotZText.text);
          float existingW = currentPose.rotation.w;

          if (currentRP !=null){

            arReferencePointManager.RemoveReferencePoint(currentRP);
            
            Quaternion newQ = new Quaternion(newX,newY,newZ,existingW);
            currentPose.rotation = newQ;
            currentRP = arReferencePointManager.AddReferencePoint(currentPose);
            show_Rotation_Values();
          };

         

    }

   void increaseZvalue() {

        if (currentRP !=null){
          arReferencePointManager.RemoveReferencePoint(currentRP);
          currentPose.position.z+=0.1f;
          currentRP = arReferencePointManager.AddReferencePoint(currentPose);
        }
      
    }

   void decreaseZvalue() {

      if (currentRP !=null){
          arReferencePointManager.RemoveReferencePoint(currentRP);
          currentPose.position.z-=0.1f;
          currentRP = arReferencePointManager.AddReferencePoint(currentPose);
      }        
  
    }

   void increaseXvalue() {

        if (currentRP !=null){
          arReferencePointManager.RemoveReferencePoint(currentRP);
          currentPose.position.x+=0.1f;
          currentRP = arReferencePointManager.AddReferencePoint(currentPose);
        }
      
    }


    void decreaseXvalue() {

      if (currentRP !=null){
          arReferencePointManager.RemoveReferencePoint(currentRP);
          currentPose.position.x-=0.1f;
          currentRP = arReferencePointManager.AddReferencePoint(currentPose);
      }        
  
    }

    void increaseYvalue() {

        if (currentRP !=null){
          arReferencePointManager.RemoveReferencePoint(currentRP);
          currentPose.position.y+=0.1f;
          currentRP = arReferencePointManager.AddReferencePoint(currentPose);
        }
      
    }

    void decreaseYvalue() {

      if (currentRP !=null){
          arReferencePointManager.RemoveReferencePoint(currentRP);
          currentPose.position.y-=0.1f;
          currentRP = arReferencePointManager.AddReferencePoint(currentPose);
      }        
  
    }


void show_xyz_values() {
  show_X_value();
  show_Y_value();
  show_Z_value();
}

void show_Z_value() {
        if (currentRP !=null){
             zvalueText.text = currentPose.position.z + "";
        }
       
       
    }

 void show_X_value() {
        if (currentRP !=null){
             xvalueText.text = currentPose.position.x + "";
        }
       
       
    }

    void show_Y_value() {
        if (currentRP !=null){
             yvalueText.text = currentPose.position.y + "";
        }
       
       
    }

   void show_Rotation_Values() {
       show_Rotation_X_value();
       show_Rotation_Y_value();
       show_Rotation_Z_value();

       
   }

   void show_Rotation_X_value() {
        if (currentRP !=null){
             rotXText.text = currentPose.rotation.x + "";
        }    
    }

   void show_Rotation_Y_value() {
        if (currentRP !=null){
             rotYText.text = currentPose.rotation.y + "";
        }    
    }

   void show_Rotation_Z_value() {
        if (currentRP !=null){
             rotZText.text = currentPose.rotation.z + "";
        }    
    }

    void handle_Placement() {

      // only really places ONE object
      if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
                  Touch touch = Input.GetTouch(0);
                  if (arRayCastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                   {
                       currentPose = hits[0].pose;
                       currentRP = arReferencePointManager.AddReferencePoint(currentPose);
                       
                       if (currentRP != null){

                            

                            show_xyz_values();
                            show_Rotation_Values();
                            referencePointList.Add(currentRP);
                            status.tag = "alley_chosen";
                            TogglePlaneDetection();
                       }

                   }
        }   

    }

    // Update is called once per frame
    void Update(){

       if (status.tag == "alley_finding"){
             handle_Placement();
        }
        else if (status.tag == "alley_chosen"){
            
        }

      show_xyz_values();
    //    rotXText.text = "111";
    //    rotYText.text = "222";
    //    rotZText.text = "333";

    //    if (rotXText == null)
    //       yvalueText.text = "err1";
    //    else 
    //     {
    //         if (rotXText.text == null)
    //            yvalueText.text = "err2";
    //         else {
    //            yvalueText.text = "dom1";
    //            rotXText.text = "dom1";
    //         }   
    //     }

     if (currentRP !=null){
         rotationsText.text = currentPose.rotation.x + " , " + currentPose.rotation.y + " , " + currentPose.rotation.z ;
     }
      
      
    }
}
