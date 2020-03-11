using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
https://www.youtube.com/watch?v=N73EWquTGSY

Animation transition conditions:

https://www.youtube.com/watch?v=HVCsg_62xYw


Animation events:

https://www.youtube.com/watch?v=-IuvXTnQS4U

*/
public class miniHothTieBehaviour : MonoBehaviour
{

    //private Animator anim;
    private AudioSource hoverSource1;
    private AudioClip hover1;
    private AudioSource hoverSource2;
    private AudioClip hover2;
    private AudioSource roarSource;
    private AudioClip roar;

    private AudioSource blasterSource;
    private AudioClip blaster;
    private GameObject tieBlastPrefab;

    private GameObject boxPrefab;

    private GameObject tieBlastOrigin;
    private Vector2 fingerDown;
    private Vector2 fingerUp;
    private bool detectSwipeOnlyAfterRelease = true;
    private float SWIPE_THRESHOLD = 20f;
    private float changeInY;
    private float changeInX;
    private Vector2 direction;
    float touchTimeStart, touchTimeFinish, timeInterval;
    private TextMesh debugText;

    private float horizontalMove = 0.0f;
    private float verticalMove = 0.0f;
    private bool debug = false;
    public bool androidDevice = false;

    bool roarSoundIsPlaying = false;

    bool performingLoop = false;
    bool performingLongLoop = false;

    private float pauseAfterShooting = 0.15f;
    bool shooting = false;
    private float blasterSpeed = 400.0f;

    public Joystick leftJoyStick;
    public Joystick rightJoyStick;
    private float movementSpeed = 1.0f;

    public float switchHoverSoundDuration = 2.0f;
    public float hoverSoundVolume = 0.7f;
    private float switchHoverSoundTimer = 0.0f;

    private bool playingHover1 = true;// flag used to determine which of 2 sounds to play to create a continuous hover sound for tie fighter

    public float increment = 0.001f;

    void Start()
    {
        playHoveringSound1();
        switchHoverSoundTimer = switchHoverSoundDuration;

    }
    void Awake()
    {
        // anim = GetComponent<Animator>();
        // anim.speed = 0.25f;

        blasterSource = GameObject.FindGameObjectWithTag("tieFighterBlaster_Sound").GetComponent<AudioSource>();
        blaster = blasterSource.clip;

        roarSource = GameObject.FindGameObjectWithTag("tieFighterRoar_Sound").GetComponent<AudioSource>();
        roar = roarSource.clip;

        hoverSource1 = GameObject.FindGameObjectWithTag("hoveringTie_Sound").GetComponent<AudioSource>();
        hover1 = hoverSource1.clip;
        hoverSource2 = GameObject.FindGameObjectWithTag("hoveringTie_Sound").GetComponent<AudioSource>();
        hover2 = hoverSource2.clip;

        tieBlastPrefab = PrefabFactory.getPrefab("miniTieBlast");

        boxPrefab = PrefabFactory.getPrefab("boxTarget");

        debugText = GameObject.Find("debugText").GetComponent<TextMesh>();
    }
    void Update()
    {

        switchHoverSoundTimer -= Time.deltaTime;
        MyDebug(switchHoverSoundTimer + " , playingHover1: " + playingHover1);
        if (switchHoverSoundTimer <= 0f)
        {
            //start duplicate sound 
            if (playingHover1)
            {

                playingHover1 = false;
                playHoveringSound2();

            }
            else
            {
                playingHover1 = true;
                playHoveringSound1();

            }

            switchHoverSoundTimer = switchHoverSoundDuration;

        }


        if (androidDevice)
            handleInputAndroid();
        else
            handleWindowsInput();

        // if (!performingLoop) {
        //       Debug.Log("Setting booleans to true again.");
        //       performingLoop = true;
        //       anim.SetBool("shouldPerformLoop",true);
        // }
        // else {
        //     Debug.Log("In Update and performingLoop is true");
        // }

        // if (Input.GetKeyDown("1") ) {
        //     anim.SetBool("shouldPerformLoop",true);
        // }
        // else {
        //     anim.SetBool("shouldPerformLoop",false);
        // }    
    }

    void shoot()
    {
        spawnNewBlasterBolt();
        PlayBlasterSound_Immediately();
    }

    void spawnNewBox()
    {
        GameObject boxSpawner = GameObject.FindGameObjectWithTag("boxTargetSpawner");
        float x = boxSpawner.transform.position.x;
        float y = boxSpawner.transform.position.y;
        float z = boxSpawner.transform.position.z;
        GameObject go = (GameObject)Instantiate(boxPrefab, new Vector3(x, y, z), boxSpawner.transform.rotation);

    }

    void playHoveringSound1()
    {


        try
        {
            hoverSource1.PlayOneShot(hover1, hoverSoundVolume);
            MyDebug("Hover 1 started");
        }
        catch (System.Exception e)
        {
            MyDebug("error sound1 : " + e.Message);
        }



    }



    void playHoveringSound2()
    {
        try
        {
            hoverSource2.PlayOneShot(hover2, hoverSoundVolume);
            MyDebug("Hover 2 started");
        }
        catch (System.Exception e)
        {
            MyDebug("error sound2 : " + e.Message);
        }




    }

    void spawnNewBlasterBolt()
    {

        tieBlastOrigin = GameObject.FindGameObjectWithTag("tieFighterBlastOrigin");
        float x = tieBlastOrigin.transform.position.x;
        float y = tieBlastOrigin.transform.position.y;
        float z = tieBlastOrigin.transform.position.z;
        GameObject go = (GameObject)Instantiate(tieBlastPrefab, new Vector3(x, y, z), tieBlastOrigin.transform.rotation);
        GameObject bolt1 = PrefabFactory.GetChildWithName(go, "leftBlast");
        GameObject bolt2 = PrefabFactory.GetChildWithName(go, "rightBlast");

        bolt1.GetComponent<Rigidbody>().velocity = blasterSpeed * tieBlastOrigin.transform.forward * Time.deltaTime;
        bolt2.GetComponent<Rigidbody>().velocity = blasterSpeed * tieBlastOrigin.transform.forward * Time.deltaTime;

    }
    void handleShoot()
    {

        if (!shooting)
        {
            shooting = true;
            shoot();
            onDifferentThread_PauseAfterShooting();
        }
    }

    void handleRoar()
    {
        if (!roarSoundIsPlaying)
            playRoarOnDifferentThread();
    }
    void handleWindowsInput()
    {

        if (Input.GetKey("up"))
        {
            increaseYvalue();
            //handleRoar();

        }
        else if (Input.GetKey("down"))
        {
            decreaseYvalue();
            //handleRoar();

        }
        else if (Input.GetKey("left"))
        {
            decreaseXvalue();
            //handleRoar();

        }
        else if (Input.GetKey("right"))
        {
            increaseXvalue();
            //handleRoar();

        }
        else if (Input.GetKey("f"))
        {
            handleShoot();

        }


    }
    void handleWindowsInput2()
    {
        if (Input.GetKeyDown("1"))
        {
            MyDebug("Pressed 1");
            // letsStartTheAnimation();
        }
        else if (Input.GetKeyDown("2"))
        {
            MyDebug("Pressed 2");
            // letsStartTheAnimation2();
        }
        else if (Input.GetKeyDown("f"))
        {
            MyDebug("Pressed f");
            handleShoot();
        }
        else if (Input.GetKeyDown("s"))
        {
            MyDebug("Pressed s");
            //spawn new box
            spawnNewBox();

        }
        else if (Input.GetKeyDown("w"))
        {

        }

    }
    // Update is called once per frame

    // void letsStartTheAnimation()
    // {

    //     if (!performingLoop)
    //     {

    //         if (!roarSoundIsPlaying)
    //             playRoarOnDifferentThread();

    //         performingLoop = true;
    //         anim.SetBool("shouldPerformLoop", true);
    //     }

    // }

    // void letsStartTheAnimation2()
    // {

    //     if (!performingLongLoop)
    //     {

    //         if (!roarSoundIsPlaying)
    //             playRoarOnDifferentThread();

    //         performingLongLoop = true;
    //         anim.SetBool("shouldPerformLongLoop", true);
    //     }

    // }

    void handleFireEventFromAnimation()
    {
        // PlayBlasterSound_Immediately();
    }
    void PlayRoarSound_Immediately()
    {
        // we want a seperate object for each fire; so we can handle multi blasts in quick succession

        // Method I used to achieve multiple blasts that don't interrupt each other:
        // https://docs.unity3d.com/ScriptReference/AudioSource.PlayOneShot.html

        roarSource.PlayOneShot(roar, 0.7f);

    }
    IEnumerator pauseThenStartAnimationAgain()
    {

        float myDelay = 1.0f;
        yield return new WaitForSeconds(myDelay);

        performingLoop = false;

    }

    void startAnimationAgainOnDifferentThread()
    {

        StartCoroutine(pauseThenStartAnimationAgain());

    }

    // void handleFinishLoop()
    // {

    //     anim.SetBool("shouldPerformLoop", false);
    //     anim.SetBool("shouldPerformLongLoop", false);
    //     performingLoop = false;
    //     performingLongLoop = false;
    //     //startAnimationAgainOnDifferentThread();

    // }

    void PlayBlasterSound_Immediately()
    {
        // we want a seperate object for each fire; so we can handle multi blasts in quick succession

        // Method I used to achieve multiple blasts that don't interrupt each other:
        // https://docs.unity3d.com/ScriptReference/AudioSource.PlayOneShot.html
        blasterSource.PlayOneShot(blaster, 0.7f);

    }

    // public bool shouldPerformLoop() {
    //     return performLoop;
    // }


    // void handleInputAndroid()
    // {
    //     if (Input.touchCount > 0)
    //     {
    //         // The screen has been touched so store the touch

    //         Touch touch = Input.GetTouch(0);

    //         if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
    //         {

    //             // If the finger is on the screen, move the object smoothly to the touch position

    //             Vector3 touchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0.0f));

    //             transform.position = Vector3.Lerp(transform.position, new Vector3(touchPosition.x, 0.0f, touchPosition.z), Time.deltaTime * 5.0f);
    //         }

    //     }
    // }
    void handleInputAndroid()
    {

        if (leftJoyStick.Horizontal >= 0.2f)
        {
            MyDebug("Move right");

            increaseXvalue();
            //handleRoar();
        }
        else if (leftJoyStick.Horizontal <= -0.2f)
        {
            MyDebug("Move left");

            decreaseXvalue();
            //handleRoar();
        }



        if (leftJoyStick.Vertical >= 0.5f)
        {
            MyDebug("Move up");

            increaseYvalue();
            //handleRoar();
        }
        else if (leftJoyStick.Vertical <= -0.5f)
        {
            MyDebug("Move down");

            decreaseYvalue();
            //handleRoar();
        }


        if (rightJoyStick.Vertical >= 0.5f)
        {
            MyDebug("Fire");
            handleShoot();
        }

    }

    void increaseYvalue()
    {
        float x = this.transform.position.x;
        float y = this.transform.position.y;
        float z = this.transform.position.z;

        y += increment;

        this.transform.position = new Vector3(x, y, z);

    }
    void increaseXvalue()
    {
        float x = this.transform.position.x;
        float y = this.transform.position.y;
        float z = this.transform.position.z;

        x += increment;

        this.transform.position = new Vector3(x, y, z);

    }
    void decreaseYvalue()
    {
        float x = this.transform.position.x;
        float y = this.transform.position.y;
        float z = this.transform.position.z;

        y -= increment;

        this.transform.position = new Vector3(x, y, z);

    }

    void decreaseXvalue()
    {
        float x = this.transform.position.x;
        float y = this.transform.position.y;
        float z = this.transform.position.z;

        x -= increment;

        this.transform.position = new Vector3(x, y, z);

    }



    void delayInSeconds(float seconds)
    {
        StartCoroutine(PauseInSeconds(seconds));
    }

    IEnumerator PauseInSeconds(float seconds)
    {

        yield return new WaitForSeconds(seconds);

    }

    void playRoarOnDifferentThread()
    {

        StartCoroutine(PlayRoarSound());

    }

    IEnumerator PlayRoarSound()
    {
        //https://answers.unity.com/questions/904981/how-to-play-an-audio-file-after-another-finishes.html
        roarSoundIsPlaying = true;
        roarSource.PlayOneShot(roar, 0.7f);
        yield return new WaitForSeconds(roar.length);
        roarSoundIsPlaying = false;

    }

    void MyDebug(string someText)
    {

        if (debugText != null & debug)
        {
            debugText.text = someText;
        }

    }

    void onDifferentThread_PauseAfterShooting()
    {

        StartCoroutine(DoThePausing());

    }

    IEnumerator DoThePausing()
    {
        yield return new WaitForSeconds(pauseAfterShooting);
        shooting = false;

    }


    void moveTieFighter_Horizontal_Swipe(float signFactor)
    {
        // float horizontalAmount = horizontalValMove() * signFactor;
        // MyDebug("want to move X by " + horizontalAmount);
        // try
        // {

        //     float x = this.transform.position.x;
        //     float y = this.transform.position.y;
        //     float z = this.transform.position.z;

        //     x += horizontalAmount;

        //     this.transform.position = new Vector3(x, y, z);

        //     // Vector3 position = this.transform.position;
        //     // position.x += horizontalAmount; //update the position of the ball to that X coordinate
        //     // this.transform.position = position;

        // }
        // catch (System.Exception e)
        // {

        //     MyDebug("error swipe horiz: " + e.Message);
        // }

        // // ONLY thing that moved bowling ball to proper position.

        // Ray ray = Camera.main.ScreenPointToRay(fingerDown);// the last position of the swipe
        // RaycastHit hit;
        // // https://docs.unity3d.com/ScriptReference/RaycastHit.html
        // if (Physics.Raycast(ray, out hit, 5000.0f))
        // {//see what object it hit in the virtual world
        //     MyDebug("hit something on x");
        //     float new_X = hit.point.x;//get the point X in the virtual world where there was a hit

        //     Vector3 position = this.transform.position;
        //     position.x = new_X; //update the position of the ball to that X coordinate
        //     this.transform.position = position;

        // }
        // else
        // {
        //     MyDebug("nothing hit on x");
        // }


    }

    void moveTieFighter_Vertical_Swipe(float signFactor)
    {

        //     float verticalAmount = verticalMove() * signFactor;
        //     MyDebug("want to move Y by " + verticalAmount);

        //     try
        //     {
        //         float x = this.transform.position.x;
        //         float y = this.transform.position.y;
        //         float z = this.transform.position.z;

        //         y += verticalAmount;

        //         this.transform.position = new Vector3(x, y, z);

        //         // Vector3 position = this.transform.position;
        //         // position.y += verticalAmount; //update the position of the ball to that Y coordinate
        //         // this.transform.position = position;
        //     }
        //     catch (System.Exception e)
        //     {

        //         MyDebug("error swipe vert: " + e.Message);
        //     }


        //     // ONLY thing that moved bowling ball to proper position.

        //     // Ray ray = Camera.main.ScreenPointToRay(fingerDown);// the last position of the swipe
        //     // RaycastHit hit;
        //     // // https://docs.unity3d.com/ScriptReference/RaycastHit.html
        //     // if (Physics.Raycast(ray, out hit, 5000.0f))
        //     // {//see what object it hit in the virtual world

        //     //     float new_Y = hit.point.y;//get the point Y in the virtual world where there was a hit

        //     //     Vector3 position = this.transform.position;
        //     //     position.y = new_Y; //update the position of the ball to that Y coordinate
        //     //     this.transform.position = position;

        //     //     MyDebug("something hit on y");
        //     // }
        //     // else
        //     // {
        //     //     MyDebug("nothing hit on y");
        //     // }


    }

}