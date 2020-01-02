using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;

public class SceneHandler : MonoBehaviour
{
    public GameObject placementIndicator;
    public GameObject alleyPrefab;

    private GameObject status;
    private bool alleyInPlace=false;


    private Pose placementPose;
    private bool placementPoseIsValid = false;

    static int attempts=0;
    static List<ARRaycastHit> s_Hits;
    ARRaycastManager m_RaycastManager;
   
/*
if ( m_SessionOrigin.Raycast (screenPoint, s_Hits, TrackableType.Planes)) {
 
           foreach( ARRaycastHit hit in s_Hits ) {
         
                if ( m_PlaneManager.TryGetPlane(hit.trackableId).boundedPlane.Alignment == PlaneAlignment.Horizontal) {
                    Pose hitPose = s_Hits[0].pose;
                    positions.Add(hitPose.position);
                    return true;
                }
           }
 
        }
*/


    // Start is called before the first frame update
    void Start()
    {
         m_RaycastManager = GetComponent<ARRaycastManager>();
        
         

         status =  GameObject.Find("statusHolder");

    }

    void instantiate_Alley() {

    }

    void handle_PlacementIndicator() {
        
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            status.tag = "alley_chosen";
            PlaceObject();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (status.tag == "alley_finding"){
             handle_PlacementIndicator();
        }
        else if (status.tag == "alley_chosen"){
            
        }
    }


    private void PlaceObject()
    {

        if (placementPoseIsValid){

            if (!alleyInPlace){
                    alleyInPlace = true;
                    
            }
            placementIndicator.SetActive(false);


            //    if (placementPoseIsValid)
            // {
            //     placementPose = hits[0].pose;

            //     var cameraForward = Camera.current.transform.forward;
            //     var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            //     placementPose.rotation = Quaternion.LookRotation(cameraBearing);
            // }
            Vector3 cameraBearing;
            var cameraForward = Camera.current.transform.forward;
            cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            Instantiate(alleyPrefab, placementPose.position,  Quaternion.Euler(cameraBearing.x, cameraBearing.y, cameraBearing.z));
        } 
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            Vector3 cameraBearing;
            var cameraForward = Camera.current.transform.forward;
            cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, Quaternion.Euler(cameraBearing.x, cameraBearing.y, cameraBearing.z));
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

     private void UpdatePlacementPose()
    {
        attempts++;
        s_Hits = new List<ARRaycastHit>();

        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        
        
        if (m_RaycastManager.Raycast(screenCenter, s_Hits, TrackableType.All))
        {
             //p_3dText_Debug.text="Attempt " + attempts + " and got a hit!";
            // Raycast hits are sorted by distance, so the first one
            // will be the closest hit.
            placementPose = s_Hits[0].pose;
            
            placementPoseIsValid = true;

            //p_3dText_Debug.text="Attempt " + attempts + " and got a hit and placement pose and rotation!";
            
            // try {
            // var cameraForward = Camera.main.transform.forward;
            // var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            // placementPose.rotation = Quaternion.LookRotation(cameraBearing);
            // p_3dText_Debug.text="Attempt " + attempts + " and got a hit and placement pose and rotation!";
            // }
            // catch(Exception e) {
            //    p_3dText_Debug.text="Attempt " + attempts + " and exception " + e.Message;
            // }
        }
        else {
             //p_3dText_Debug.text="Attempt " + attempts + " and no hits!";
           placementPoseIsValid = false; 
        }
       

    }

}
