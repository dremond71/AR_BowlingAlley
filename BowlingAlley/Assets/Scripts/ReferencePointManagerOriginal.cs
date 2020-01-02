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
public class ReferencePointManagerOriginal : MonoBehaviour
{
    [SerializeField]
    private Text debugLog;
    
    [SerializeField]
    private Text referencePointCount;

    [SerializeField]
    private Button toggleButton;


    private ARRaycastManager arRayCastManager;
    private ARReferencePointManager arReferencePointManager;
    private ARPlaneManager arPlaneManager;
    private List<ARReferencePoint> referencePointList = new List<ARReferencePoint>();
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

     void Awake()
    {
        arRayCastManager        = GetComponent<ARRaycastManager>();
        arReferencePointManager = GetComponent<ARReferencePointManager>();
        arPlaneManager          = GetComponent<ARPlaneManager>();
        toggleButton.onClick.AddListener(TogglePlaneDetection);
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
        
        toggleButton.GetComponentInChildren<Text>().text = arPlaneManager.enabled ? 
            "Disable Plane Detection" : "Enable Plane Detection";
    }

    // Update is called once per frame
    void Update(){

      if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
                  Touch touch = Input.GetTouch(0);
                  if (arRayCastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                   {
                       Pose hitPose = hits[0].pose;
                       ARReferencePoint referencePoint = arReferencePointManager.AddReferencePoint(hitPose);

                       if (referencePoint == null){
                            string msg = "The reference point is null\n";
                            Debug.Log(msg);
                            debugLog.text += msg;
                       }
                       else {
                            referencePointList.Add(referencePoint);
                            referencePointCount.text = "Reference point Count: " + referencePointList.Count;
                       }

                   }
        }   
        
    }
}
