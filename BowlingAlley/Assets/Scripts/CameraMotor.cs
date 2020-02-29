using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
  private Vector3 desiredPosition;
  public Transform lookAt;
  private Vector3 offset;
  private float smoothSpeed = 7.5f;
  private float distance = 25.0f;
  private float height = 5.0f;
  private float rotationDamping = 3.0f;
 
  private float yOffset = 3.5f;


  private void Start() {
      offset = new Vector3(0,yOffset, -1.0f * distance);
     
  }
  
//   private void Update() {

//       if (Input.GetKeyDown(KeyCode.LeftArrow))
//           SlideCamera(true);
//       else if  (Input.GetKeyDown(KeyCode.RightArrow))
//         SlideCamera(false);

//   }

  private void FixedUpdate() {
    float wantedRotationAngleSide = lookAt.eulerAngles.y;
    float currentRotationAngleSide = transform.eulerAngles.y;
   
   float wantedRotationAngleUp = lookAt.eulerAngles.x;
    float currentRotationAngleUp = transform.eulerAngles.x;
   
    currentRotationAngleSide = Mathf.LerpAngle(currentRotationAngleSide, wantedRotationAngleSide, rotationDamping * Time.deltaTime);
   
    currentRotationAngleUp = Mathf.LerpAngle(currentRotationAngleUp, wantedRotationAngleUp, rotationDamping * Time.deltaTime);
   
   Quaternion currentRotation = Quaternion.Euler(currentRotationAngleUp, currentRotationAngleSide, 0);
   
    transform.position = lookAt.position;
    transform.position -= currentRotation * Vector3.forward * distance;
   
    transform.LookAt(lookAt);
   
    transform.position += transform.up * height;

  }


  /*
  var target : Transform;
var distance = 5.0;
var height = 4.0;
 
var rotationDamping = 3.0;
 
 
function LateUpdate () {
    if (!target)
        return;
       
    wantedRotationAngleSide = target.eulerAngles.y;
    currentRotationAngleSide = transform.eulerAngles.y;
   
    wantedRotationAngleUp = target.eulerAngles.x;
    currentRotationAngleUp = transform.eulerAngles.x;
   
    currentRotationAngleSide = Mathf.LerpAngle(currentRotationAngleSide, wantedRotationAngleSide, rotationDamping * Time.deltaTime);
   
    currentRotationAngleUp = Mathf.LerpAngle(currentRotationAngleUp, wantedRotationAngleUp, rotationDamping * Time.deltaTime);
   
    currentRotation = Quaternion.Euler(currentRotationAngleUp, currentRotationAngleSide, 0);
   
    transform.position = target.position;
    transform.position -= currentRotation * Vector3.forward * distance;
   
    transform.LookAt(target);
   
    transform.position += transform.up * height;
}
  */
  private void FixedUpdate2() {
      desiredPosition = lookAt.position + offset;
      transform.position = Vector3.Lerp(transform.position,desiredPosition,smoothSpeed * Time.deltaTime);
    //   transform.LookAt(lookAt.position + Vector3.up);
    //  transform.position = lookAt.position + offset;
      transform.rotation =  Quaternion.Lerp(transform.rotation,lookAt.rotation,smoothSpeed * Time.deltaTime);

       transform.LookAt(lookAt.position  + Vector3.up);
      //transform.LookAt(lookAt.position  + Vector3.up);
          
//           transform.position = lookAt.position + offset;
//            transform.LookAt(lookAt.position  + Vector3.up);
   }
  


  /*


   Transform from;
    Transform to;
    float speed = 0.1f;
    void Update()
    {
        transform.rotation = Quaternion.Lerp(from.rotation, to.rotation, Time.time * speed);
    }

  https://forum.unity.com/threads/smooth-follow-on-all-axis.148870/
  var target : Transform;
var aircraft : Transform;
var speed = 5.0;
 
function LateUpdate() {
 
transform.position = Vector3.Lerp ( transform.position, target.position,Time.deltaTime * speed);
 
transform.rotation = transform.rotation = Quaternion.Lerp(transform.rotation,aircraft.rotation,speed*Time.deltaTime);          
}
  */
  public void SlideCamera(bool left)
  {
      if (left){
          offset = Quaternion.Euler(0,90,0) * offset;
      }
      else {
          offset = Quaternion.Euler(0,-90,0) * offset;
      }
  }
}
