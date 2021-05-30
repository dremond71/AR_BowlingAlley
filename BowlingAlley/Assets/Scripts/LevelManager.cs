﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    private static object olock = new object();

    static string launchTheDSAttack = "";
    
    private float pauseAfterShootingMissles = 1.5f;

    private bool debug = false;

    private TextMesh debugText;

    private int totalSpawnPositionsAvailable = 9;
    private int currentAllowedSpawnPositions = 0;
    System.Random rnd = new System.Random();

    List<int> randomSpawnPositions = new List<int>();
    private int randomSpawnMatrixNumber = 0;
    private int numberOfSpawnMatrices = 3;

    private AudioSource[] empireSources;
    private AudioClip[] empireClips;

    private AudioSource deathStarBlastSource;
    private AudioClip deathStarBlast;
    private float deathStarBlastVolume = 0.8f;

    private bool empireMode = true;
    static int numberOfSpawnedItems = 0;

    static bool tieFighterAllowedToAttack = true;

    public float enemySpeed = 15.0f;

    private GameObject boxPrefab;

    private int numberOfTimesXWingsSpawned = 0;
    private GameObject xwingPrefab;
    private GameObject awingPrefab;
    private GameObject meteorite1Prefab;

    private GameObject rebelStarshipPrefab;

    private GameObject falconPrefab;
    private GameObject deathStarMisslePrefab;

    private bool starshipExists = false;

    public int totalSpawnsBeforeClip = 8;
    private int numberOfSpawnsSinceLastClip = 0;

    private bool clipIsPlaying = false;
    private int nextEmpireClipIndex = 0;

    private float startGamePauseDuration = 10.0f;
    private float startGameTimer;

    public static int falconIntroIndex = 2;

    private int[] targetRebelVehicleTypes = new[] {1,2};

    // determine what mixture we want of rebel vehicles
    // 1 - swarm of specific vehicle type 
    // 2 - swarm of mixture of all types
    // 3 - asteroid swarm
    // 4 - millenium falcon only
    private int[] targetRebelVehicleMixture = new[] { 1, 2, 3, 4 };
    private int vehicleMixtureLevel = 1;//default

    void load_EmpireSounds()
    {
        empireSources = new AudioSource[3];
        empireClips = new AudioClip[3];

        empireSources[0] = GameObject.FindGameObjectWithTag("emperorEndOfAlliance_Sound").GetComponent<AudioSource>();
        empireClips[0] = empireSources[0].clip;

        empireSources[1] = GameObject.FindGameObjectWithTag("emperorDesign_Sound").GetComponent<AudioSource>();
        empireClips[1] = empireSources[1].clip;

        empireSources[2] = GameObject.FindGameObjectWithTag("emperorLaugh_Sound").GetComponent<AudioSource>();
        empireClips[2] = empireSources[2].clip;

    }

    int getNextEmpireClipIndex()
    {
        int value = nextEmpireClipIndex;
        nextEmpireClipIndex++;
        if (nextEmpireClipIndex > (empireSources.Length - 1))
        {
            nextEmpireClipIndex = 0;
        }

        return value;
    }


    void setRandomSpawnAmount()
    {
        // choose a random amount between 1 and 9
        // OLD WAY - currentAllowedSpawnPositions = rnd.Next(1, totalSpawnPositionsAvailable);
        // Random.Range(1, 10);
        // return a number (1,2,3,4,5,6,7,8,9 ) - does not include 10
        currentAllowedSpawnPositions = Random.Range(1, (totalSpawnPositionsAvailable + 1)); 
        //  MyDebug("Chosen amount : " + currentAllowedSpawnPositions);
    }
    void getRandomSpawnPositions()
    {
        randomSpawnPositions.Clear();

        int totalCollected = 0;

        while (totalCollected < currentAllowedSpawnPositions)
        {
            int someNumber = GetRandomSpawnPosition();
            // if ((someNumber > 9) || (someNumber < 1)) {
            //   MyDebug("Unexpected Spawn Position");
            // }
            if (!randomSpawnPositions.Contains(someNumber))
            {
                randomSpawnPositions.Add(someNumber);
                totalCollected++;
            } //if

        }

    }

    float getSlightlyRandomizedSpeed()
    {    
        return enemySpeed + GetRandomSpeedAdjustment();
    }

    float getFalconSpeed()
    {    
        return enemySpeed + 30.0f;
    }

    float GetRandomSpeedAdjustment()
    {
        return Random.Range(0.0f, 20.0f);
    }

    void getRandomMatrixNumber()
    {
        randomSpawnMatrixNumber =  Random.Range(1, (numberOfSpawnMatrices + 1));
    }

    int GetRandomSpawnPosition()
    {

        // return a number (1,2,3,4,5,6,7,8,9) - does not include 10
        return Random.Range(1, (totalSpawnPositionsAvailable + 1)); 
        //return rnd.Next(1, totalSpawnPositionsAvailable);//OLD WAY
    }

    int GetRandomMixtureOfVehiclesValue()
    {
        int firstIndex = 0;
        int lastIndex = targetRebelVehicleMixture.Length - 1;

        int rangeBeginningValue = targetRebelVehicleMixture[firstIndex];    // e.g. 1
        int rangeEndingValue = targetRebelVehicleMixture[lastIndex] + 1; // e.g. 2 + 1

        // Random.Range(1, 3);
        // return a number (1, or 2) - does not include 3
        return Random.Range(rangeBeginningValue, rangeEndingValue);

    }

    int GetRandomVehicleType()
    {

        
        int firstIndex = 0;
        int lastIndex = targetRebelVehicleTypes.Length - 1;

        int rangeBeginningValue = targetRebelVehicleTypes[firstIndex];    // e.g. 1
        int rangeEndingValue    = targetRebelVehicleTypes[lastIndex] + 1; // e.g. 2 + 1

        // Random.Range(1, 3);
        // return a number (1, or 2) - does not include 3
        return Random.Range(rangeBeginningValue, rangeEndingValue); 

        // // let's return only 2 possible values right now
        // // Random.value returns a number between 0 and 1
        // if(Random.value<0.5f){
        //   return 1;
        // }
        // else {
        //   return 2;
        // }
    }

    void Awake()
    {
        xwingPrefab = PrefabFactory.getPrefab("miniTargetXWingFighter");
        awingPrefab = PrefabFactory.getPrefab("a-wing");
        meteorite1Prefab = PrefabFactory.getPrefab("meteorite1");

        boxPrefab = PrefabFactory.getPrefab("boxTarget");
        //rebelStarshipPrefab = PrefabFactory.getPrefab ("rebelTantiveIV");
        rebelStarshipPrefab = PrefabFactory.getPrefab("newTantiveIV");
        falconPrefab = PrefabFactory.getPrefab("falcon");
        deathStarMisslePrefab = PrefabFactory.getPrefab("deathStarMissle");

        debugText = GameObject.Find("debugText").GetComponent<TextMesh>();

        deathStarBlastSource = GameObject.FindGameObjectWithTag("deathStarBlast_Sound").GetComponent<AudioSource>();
        deathStarBlast = deathStarBlastSource.clip;


        if (empireMode)
        {
            load_EmpireSounds();
        }

    }

    GameObject[] getAllTargetVehicles()
    {

        GameObject[] xwings;
        GameObject[] awings;
        GameObject[] meteorites;
        GameObject[] falcons;
        GameObject[] tantiveIV;

        if (empireMode)
        {
            xwings     = GameObject.FindGameObjectsWithTag("targetXWing");
            meteorites = GameObject.FindGameObjectsWithTag("targetMeteorite");
            awings     = GameObject.FindGameObjectsWithTag("targetAWing");
            falcons    = GameObject.FindGameObjectsWithTag("falcon");
            tantiveIV  = GameObject.FindGameObjectsWithTag("tantiveIV");
        }
        else
        {
            xwings     = new GameObject[0];
            meteorites = new GameObject[0];
            awings     = new GameObject[0];
            falcons    = new GameObject[0];
            tantiveIV  = new GameObject[0];
        }

         List<GameObject> list  = new List<GameObject>();
         list.AddRange(xwings);
         list.AddRange(awings);
         list.AddRange(meteorites);
         list.AddRange(falcons);
         list.AddRange(tantiveIV);

        GameObject[] allTargetVehicles = list.ToArray();

        return allTargetVehicles;
        
    }

    bool targetsExist()
    {
        GameObject[] tvs;
        GameObject[] boxes;
        GameObject[] meteorites;
        GameObject[] awings;
        GameObject[] falcons;

        if (empireMode)
        {
            tvs = GameObject.FindGameObjectsWithTag("targetXWing");
            meteorites = GameObject.FindGameObjectsWithTag("targetMeteorite");
            awings = GameObject.FindGameObjectsWithTag("targetAWing");
            boxes = GameObject.FindGameObjectsWithTag("box");
            falcons = GameObject.FindGameObjectsWithTag("falcon");
        }
        else
        {
            tvs = new GameObject[0];
            meteorites = new GameObject[0];
            awings = new GameObject[0];
            boxes = new GameObject[0];
            falcons = new GameObject[0];
        }

        return (tvs.Length > 0) || (meteorites.Length > 0) || (awings.Length > 0)|| (boxes.Length > 0) || (falcons.Length > 0);
    }
    void Start()
    {
        numberOfSpawnsSinceLastClip = totalSpawnsBeforeClip; //we want to play a clip at the start
        startGameTimer = startGamePauseDuration;
    }

    void spawnFalconAtSpawnPosition1()
    {

        GameObject falconSpawner = GameObject.FindGameObjectWithTag("falconSpawnPoint1");
        float x = falconSpawner.transform.position.x;
        float y = falconSpawner.transform.position.y;
        float z = falconSpawner.transform.position.z;
        GameObject go = (GameObject)Instantiate(falconPrefab, new Vector3(x, y, z), falconSpawner.transform.rotation);
        go.SendMessage("setAnimationScenario", true);
        

    }

    void spawnRebelStarship()
    {

        GameObject starshipSpawner = GameObject.FindGameObjectWithTag("starshipSpawnPoint");
        float x = starshipSpawner.transform.position.x;
        float y = starshipSpawner.transform.position.y;
        float z = starshipSpawner.transform.position.z;
        GameObject go = (GameObject)Instantiate(rebelStarshipPrefab, new Vector3(x, y, z), starshipSpawner.transform.rotation);

    }

    void shootAllMisslesNow()
    {
        StartCoroutine(shootAllTargetVehicles());
    }

    void makeDeathStarTriangleStemsVisible(bool visible)
    {
        Renderer rend = null;
        GameObject[] stems = GameObject.FindGameObjectsWithTag("deathStarTriangleStem");
        for (int i = 0; i < stems.Length; i++)
        {
            rend = stems[i].GetComponent<Renderer>();
            if (rend != null)
            {
                rend.enabled = visible;
            }
        }
    }

    IEnumerator shootAllTargetVehicles()
    {

        float shotDelay = 0.20f;
        float stemDelay = 0.05f;

        GameObject[] allTargets = getAllTargetVehicles();

        if (allTargets.Length > 0)
        {

            // play firing sound
            PlayDeathStarBlastSound_Immediately();

            //
            // show firing stems with slight delay before each (like in the movies ;) )
            //
            Renderer rend = null;
            GameObject[] stems = GameObject.FindGameObjectsWithTag("deathStarTriangleStem");
            for (int i = 0; i < stems.Length; i++)
            {
                rend = stems[i].GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.enabled = true;
                    yield return new WaitForSeconds(stemDelay);
                }
            }

            //
            // fire all missles
            //
            for (int i = 0; i < allTargets.Length; i++)
            {
                try
                {
                    spawnNewDeathStarMissle(allTargets[i]);
                }
                catch (System.Exception e)
                {
                    MyDebug("shootAllTargetVehicles() error: " + e.ToString());
                }
                
                yield return new WaitForSeconds(shotDelay);
            }

            yield return new WaitForSeconds(0.2f);

            //
            // turn off firing stems - all at once (no delay)
            //
            for (int i = 0; i < stems.Length; i++)
            {
                rend = stems[i].GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.enabled = false;
                }
            }

           

            makeDeathStarTriangleStemsVisible(false);

            onDifferentThread_PauseAfterShootingMissle();

        }

 

    }

        void spawnNewDeathStarMissle(GameObject targetGameObject)
    {

        //missleLauncher
        GameObject weaponOrigin = GameObject.FindGameObjectWithTag("deathStarMissleLauncher");
        float x = weaponOrigin.transform.position.x;
        float y = weaponOrigin.transform.position.y;
        float z = weaponOrigin.transform.position.z;
        GameObject go = (GameObject)Instantiate(deathStarMisslePrefab, new Vector3(x, y, z), weaponOrigin.transform.rotation);

        // pass a parameter to an instantiated prefab : https://answers.unity.com/questions/254003/instantiate-gameobject-with-parameters.html 
        // https://docs.unity3d.com/ScriptReference/GameObject.SendMessage.html
        go.SendMessage("setTargetObject", targetGameObject);
        go.GetComponent<Rigidbody>().velocity = 9f * this.transform.forward;
    }

    void onDifferentThread_PauseAfterShootingMissle()
    {

        StartCoroutine(DoThePausingForMissle());

    }

    IEnumerator DoThePausingForMissle()
    {
        yield return new WaitForSeconds(pauseAfterShootingMissles);
        launchTheDSAttack = "";
    }

    void handleLaunchingDeathStarAttack()
    {
        launchTheDSAttack = "launched";// we only want launch the attack once for the initial request

        shootAllMisslesNow();

       
    }

    void Update()
    {

        
        if (launchTheDSAttack == "launch")
        {
            handleLaunchingDeathStarAttack();
        }
        

        if (startGameTimer > 0.0f)
            startGameTimer -= Time.deltaTime;

        if (startGameTimer <= 0f)
        {

           // MyDebug("numberOfSpawnsSinceLastClip==" + numberOfSpawnsSinceLastClip);
            if (!starshipExists)
            {
                starshipExists = true;
                spawnRebelStarship();
            }

            if (targetsExist())
            {
                // spawned targets exist; and making noise

                // no spawning and no clips to play
            }
            else
            {
                // no targets exist; and there's silence 

                // it's time to play a sound clip, then spawn targets
                if (numberOfSpawnsSinceLastClip == totalSpawnsBeforeClip)
                {

                    if (!clipIsPlaying)
                        playClipOnDifferentThreadThenAllowSpawnedTargets();
                }
                else
                {
                    //it's time to spawn targets, and NOT play a clip
                    handleSpawning();
                }

            }

        } //if - start game

    }

    void spawnSquadronOfXWings()
    {
        setRandomSpawnAmount(); //determine amount to spawn
        spawnNewTargets(); // this updates numberOfSpawnsSinceLastClip++;
        numberOfTimesXWingsSpawned++;
    }

    void spawnHeroWithAnimation()
    {

        //setTieFighterAllowedToShoot(false); // let hero say something before battle starts
        spawnFalconAtSpawnPosition1();
        numberOfTimesXWingsSpawned++;
        numberOfSpawnsSinceLastClip++; // keep track of spawn events (not number of targets spawned)

    }

    void handleSpawning()
    {

        // spawn a group of xwings, or spawn a hero

        if (numberOfTimesXWingsSpawned == 0)
        {
            // we don't want a HERO to be the 1st thing to shoot at, so ensure it is a group of xwings
            spawnSquadronOfXWings();
        }
        else
        {
            // after user has shot first squadron of xwings, it is fine to introduce a HERO, or another squadron of xwings

            // let's do it after 15 swarms of target vehicles
            bool value = ((numberOfTimesXWingsSpawned % 15) == 0);

            if (value)
            {
                spawnHeroWithAnimation();
            }
            else
            {
                spawnSquadronOfXWings();

            }

        }

    }

    
        void spawnNormalFalcon()
    {
        int position=5; // middle position of any matrix
        string spawnPositionNumber = "spawn" + randomSpawnMatrixNumber + "" + position;
        GameObject enemySpawner = GameObject.Find(spawnPositionNumber);
        float x = enemySpawner.transform.position.x;
        float y = enemySpawner.transform.position.y;
        float z = enemySpawner.transform.position.z;
        GameObject go = (GameObject)Instantiate(falconPrefab, new Vector3(x, y, z), enemySpawner.transform.rotation);
        go.transform.RotateAround(go.transform.position, go.transform.up, 180f);
        go.SendMessage("setAnimationScenario", false);// <=== normal falcon
        go.GetComponent<Rigidbody>().velocity = getFalconSpeed() * enemySpawner.transform.forward * -1f * Time.deltaTime;
        incrementNumSpawned();
    }

    void spawnNewXWingAtPosition(int position)
    {

        string spawnPositionNumber = "spawn" + randomSpawnMatrixNumber + "" + position;
        GameObject enemySpawner = GameObject.Find(spawnPositionNumber);
        //GameObject enemySpawner = GameObject.Find("specialSpawn1");
        float x = enemySpawner.transform.position.x;
        float y = enemySpawner.transform.position.y;
        float z = enemySpawner.transform.position.z;
        GameObject go = (GameObject)Instantiate(xwingPrefab, new Vector3(x, y, z), enemySpawner.transform.rotation);
        go.transform.RotateAround(go.transform.position, go.transform.up, 180f);
        go.GetComponent<Rigidbody>().velocity = getSlightlyRandomizedSpeed() * enemySpawner.transform.forward * -1f * Time.deltaTime;
        incrementNumSpawned();
    }

    void spawnNewAWingAtPosition(int position)
    {

        string spawnPositionNumber = "spawn" + randomSpawnMatrixNumber + "" + position;
        GameObject enemySpawner = GameObject.Find(spawnPositionNumber);
        float x = enemySpawner.transform.position.x;
        float y = enemySpawner.transform.position.y;
        float z = enemySpawner.transform.position.z;
        GameObject go = (GameObject)Instantiate(awingPrefab, new Vector3(x, y, z), enemySpawner.transform.rotation);
        go.transform.RotateAround(go.transform.position, go.transform.up, 180f);
        go.GetComponent<Rigidbody>().velocity = getSlightlyRandomizedSpeed() * enemySpawner.transform.forward * -1f * Time.deltaTime;
        incrementNumSpawned();
    }

    void incrementNumSpawned()
    {
        lock (olock)
        {
            numberOfSpawnedItems++;
        }
    }

    public static GameObject[] filterGameObjectsInFrontOfPlayer(GameObject[] given) {

       List<GameObject> list  = new List<GameObject>();
         
        // get player's Z position
        GameObject shooter = GameObject.FindGameObjectWithTag("PlayerShooter");
        float shooterZ = shooter.transform.position.z;

        if (given != null) {
            for (int i = 0; i < given.Length; i++)
            {
                if (given[i] != null) {
                    float givenZ = given[i].transform.position.z;
                    // opposite logic to vehicles destroying themselves
                    // shortly after passing shooter.
                    if (givenZ >= (shooterZ + 0.5f)){
                        list.Add(given[i]);
                    }
                }
            }
        }//if

      return list.ToArray();
    }

    public static int getFalconIntroIndex()
    {
        if (falconIntroIndex == 5)
        {
            falconIntroIndex = 1;
        }
        int value = falconIntroIndex;
        falconIntroIndex++;

        return value;
    }
    public static bool getTieFighterAllowedToShoot()
    {
        lock (olock)
        {
            return tieFighterAllowedToAttack;
        }
    }

    public static void launchDeathStarAttack()
    {

        lock (olock)
        {
            if (launchTheDSAttack == "")
            {
                launchTheDSAttack = "launch";
            }
        }
       
       
    }

    public static void setTieFighterAllowedToShoot(bool value)
    {
        lock (olock)
        {
            tieFighterAllowedToAttack = value;
        }
    }

    public static void decrementNumSpawned()
    {
        lock (olock)
        {
            numberOfSpawnedItems--;
        }
    }

    void spawnNewBoxAtPosition(int position)
    {

        string spawnPositionNumber = "spawn" + randomSpawnMatrixNumber + "" + position;
        GameObject enemySpawner = GameObject.Find(spawnPositionNumber);
        float x = enemySpawner.transform.position.x;
        float y = enemySpawner.transform.position.y;
        float z = enemySpawner.transform.position.z;
        GameObject go = (GameObject)Instantiate(boxPrefab, new Vector3(x, y, z), enemySpawner.transform.rotation);
        go.GetComponent<Rigidbody>().velocity = getSlightlyRandomizedSpeed() * enemySpawner.transform.forward * -1f * Time.deltaTime;
        incrementNumSpawned();

    }

    void spawnNewMeteoriteAtPosition(int position)
    {

        string spawnPositionNumber = "spawn" + randomSpawnMatrixNumber + "" + position;
        GameObject enemySpawner = GameObject.Find(spawnPositionNumber);
        float x = enemySpawner.transform.position.x;
        float y = enemySpawner.transform.position.y;
        float z = enemySpawner.transform.position.z;
        GameObject go = (GameObject)Instantiate(meteorite1Prefab, new Vector3(x, y, z), enemySpawner.transform.rotation);
        go.GetComponent<Rigidbody>().velocity = getSlightlyRandomizedSpeed() * enemySpawner.transform.forward * -1f * Time.deltaTime;
        incrementNumSpawned();
    }

    void spawnNewTargets2()
    {
        // spawn at specialSpawn1
        spawnNewXWingAtPosition(1);
    }

        void spawnNewTargets()
    {
        getRandomMatrixNumber(); // sets matrix number to 1 or 2

        getRandomSpawnPositions();

        // determine the mixture level of target vehicles, or special hero flyby
        vehicleMixtureLevel = GetRandomMixtureOfVehiclesValue();

        if (vehicleMixtureLevel == 1)
        {
            //
            // swarm of specific vehicle type
            //

            // get specific vehicle type up front
            int vehicleType = GetRandomVehicleType();

            foreach (var pos in randomSpawnPositions)
            {

                if (vehicleType == 1)
                {
                    spawnNewXWingAtPosition(pos);
                }
                else if (vehicleType == 2)
                {
                    spawnNewAWingAtPosition(pos);
                }

            }
        }
        else if (vehicleMixtureLevel == 2)
        {
            //
            // mixture of all vehicle types
            //

            foreach (var pos in randomSpawnPositions)
            {
                //
                // get random vehicle type within loop
                //
                int vehicleType = GetRandomVehicleType();
                
                if (vehicleType == 1)
                {
                    spawnNewXWingAtPosition(pos);
                }
                else if (vehicleType == 2)
                {
                    spawnNewAWingAtPosition(pos);
                }

            }

        }
        else if (vehicleMixtureLevel == 3)
        {
            //
            // asteroid swarm (no vehicles)
            //
            foreach (var pos in randomSpawnPositions)
            {
               spawnNewMeteoriteAtPosition(pos);
            }

        }
        else if (vehicleMixtureLevel == 4) {
          // falcon flyby
          spawnNormalFalcon();
        }

        numberOfSpawnsSinceLastClip++; // keep track of spawn events (not number of targets spawned)
    }

    void MyDebug(string someText)
    {

        if (debugText != null & debug)
        {
            debugText.text = someText;
        }

    }

    void PlayDeathStarBlastSound_Immediately()
    {
        deathStarBlastSource.PlayOneShot(deathStarBlast, deathStarBlastVolume);

    }

    void playClipOnDifferentThreadThenAllowSpawnedTargets()
    {

        clipIsPlaying = true;
        AudioSource audioSource = null;
        AudioClip audioClip = null;

        if (empireMode)
        {
            int index = getNextEmpireClipIndex();
            audioSource = empireSources[index];
            audioClip = empireClips[index];
        }

        StartCoroutine(PlayClipThenAllowSpawnedTargets(audioSource, audioClip));

    }
    IEnumerator PlayClipThenAllowSpawnedTargets(AudioSource audioSource, AudioClip audioClip)
    {
        //https://answers.unity.com/questions/904981/how-to-play-an-audio-file-after-another-finishes.html

        yield return new WaitForSeconds(3.0f);
        audioSource.PlayOneShot(audioClip, 3.0f);
        yield return new WaitForSeconds(audioClip.length);
        yield return new WaitForSeconds(1.5f);
        numberOfSpawnsSinceLastClip = 0; //reset to zero before we play a sound clip.
        clipIsPlaying = false;

    }
}