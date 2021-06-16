using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarDestroyerBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
           
            GameObject[] allSwivels = GameObject.FindGameObjectsWithTag("sdTopLeftTurboLaserSwivel");
            if (allSwivels.Length > 0) {
                GameObject target         = GameObject.FindGameObjectWithTag ("PlayerShooter");
                for (int i = 0; i < allSwivels.Length; i++){
                    GameObject oneSwivel = allSwivels[i]; 

                       // move the turbo laser base left or right depending on the target's position
                       Vector3 targetPosition = new Vector3(target.transform.position.x, oneSwivel.transform.position.y, target.transform.position.z);
                       oneSwivel.transform.LookAt(targetPosition);


                       //
                       // Since I cannot seem to succeed at making the barrels to move up or down (without also changing x)
                       // then perhaps I should make bullets that track their target just like the missles?
                       // lobbing bullets at targets?
                       // OR maybe I should make the barrels the child of the barrel rod, and rotate the barrel rod up or down?


                    //    GameObject theBlastersParent = PrefabFactory.GetChildWithName(oneSwivel,"theBlasters");
                    //    if (theBlastersParent != null){
                    //       GameObject rod = PrefabFactory.GetChildWithName(theBlastersParent,"blasterEndCapRod");

                    //         // figuring out how to move barrel up and down is brutal
                    //         // similar question here: https://forum.unity.com/threads/help-aiming-a-tank-barrel-locked-y-look-rotation-to-target-turret-movement-is-fine-though.688750/

                            
                    //         // works with Y, but also tracks x :S
                    //         Vector3 lookPos = target.transform.position - rod.transform.position;
                    //         Quaternion lookRot = Quaternion.LookRotation(lookPos, Vector3.up);
                    //         float eulerY = lookRot.eulerAngles.y;
                    //         float eulerX = lookRot.eulerAngles.x;
                    //         float eulerZ = lookRot.eulerAngles.z;
                    //         // definitely NOT : Quaternion rotation = Quaternion.Euler (0, eulerY, 0);
                    //         Quaternion rotation = Quaternion.Euler (-eulerX, 0, 0);//best
                    //         //Quaternion rotation = Quaternion.Euler (-eulerX, 0, eulerZ);// nope
                    //         rod.transform.rotation = rotation;
                    //         //rod.transform.Rotate(90,-90,180);// almost perfect 
                    //         //rod.transform.Rotate(90,-45,180);// rod tilting to left
                    //         //rod.transform.Rotate(-90,-90,180);// almost perfect but pointing straight up
                    //         //rod.transform.Rotate(90,-90,180);// almost perfect but point straight down
                    //         //rod.transform.Rotate(135,-90,180);// 45 degrees up off horizontal facing me
                    //         //rod.transform.Rotate(90,-135,180);// 45 degrees down off horizontal facing me
                    //         //rod.transform.Rotate(90,-90,90);// crooked
                    //         //rod.transform.Rotate(90,-90,270);// 45 degrees down off horizontal facing me
                    //         //rod.transform.Rotate(-90,-90,180);// almost perfect but pointing straight up
                    //          //rod.transform.Rotate(-180,-90,180);// crooked
                    //          //rod.transform.Rotate(-45,-45,180);//crooked
                    //          //rod.transform.Rotate(180,180,180);//vertical pointing backwards
                    //          //rod.transform.Rotate(90,-90,180);// points at sky, but also tracks X :S
                    //          //rod.transform.Rotate(0,-90,0);// points to left
                    //          rod.transform.Rotate(90,-90,0);// horizontal points down
                    //          //rod.transform.Rotate(90,-45,0);// 45 degrees off horizontal points down - away from me
                    //          //rod.transform.Rotate(90,-90,45);// 45 degrees off horizontal points down - towards me
                             

                    //    }
           
                }
            }
            


    }
}
