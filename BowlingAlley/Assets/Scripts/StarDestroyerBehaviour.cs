using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarDestroyerBehaviour : MonoBehaviour
{
    private AudioSource metalHitSource;
    private AudioClip metalHit;
    private float blasterVolume = 0.1f;
    
    Vector3 contactPoint;

    public float health = 999999999f;
    public GameObject muzzleFlashEffect;

    // Start is called before the first frame update
    void Start()
    {
            metalHitSource = GameObject.FindGameObjectWithTag("swMetalHit_Sound").GetComponent<AudioSource>();
            metalHit = metalHitSource.clip;        
    }

    void PlayMetalHitSound_Immediately()
    {
        metalHitSource.PlayOneShot(metalHit, blasterVolume); //0.8

    }


        private void OnCollisionEnter(Collision collision)
    {

        ContactPoint cp = collision.contacts[0];
        contactPoint = cp.point;

        if (collision.gameObject.tag == "miniTieBlast")
        {          
            health -= 1f;
            handleHit();
            
        }
        else if (collision.gameObject.tag == "miniXWingBlast")
        {
            health -= 1f;
            handleHit();
        }
        else if (collision.gameObject.tag == "falconBlast")
        {
            health -= 1f;
            handleHit();
        }        
        else if (collision.gameObject.tag == "miniTieMissle")
        {
            health -= 1f;
            handleHit(); 
        }
        else if (collision.gameObject.tag == "deathStarMissle")
        {
            health -= 1f;
            handleHit();
        }
        else
        {
            health -= 1f;
            handleHit();
        }

       // MyDebug("box collided with : " + collision.gameObject.tag);
   
    }

    void handleHit()
    {

        handleDamage();

    }
    void handleDamage()
    {

        GameObject damageObject = Instantiate(muzzleFlashEffect, contactPoint, muzzleFlashEffect.transform.rotation);
        damageObject.transform.Rotate(0f, 45f, 0f);
        damageObject.transform.SetParent(this.transform);


    }

    
    void destroyIfIrrelevantNow()
    {
        // this object becomes irrelevant 
        // IF it has flown past the death star; having been spawned from starDestroyerSpawnPoint1
        // some other condition if spawned from somewhere else

        GameObject deathStar = GameObject.FindGameObjectWithTag("deathStar");
        float deathStarZ = deathStar.transform.position.z;

        float myZ = transform.position.z;

        if (myZ > (deathStarZ + 10f))
        {
            LevelManager.starDestroyerAttackFinished(); // tells LevelManager that the cavalry attack is finished
            destroySelf();
        }

    }
    void destroySelf()
    {
        Destroy(gameObject);
    }
    void FixedUpdate()
    {
           
           destroyIfIrrelevantNow();

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
