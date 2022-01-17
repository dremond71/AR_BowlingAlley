using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    private static object olock = new object();

    static string launchTheDSAttack = "";

    static string launchTheOptionsDialog = "";

    static string launchTheCavalryAttack = "";

    private float pauseAfterShootingMissles = 1.5f;

    private static bool debug = false;

    private static TextMesh debugText;

    private int totalSpawnPositionsAvailable = 9;
    private int currentAllowedSpawnPositions = 0;
    private int previous_currentAllowedSpawnPositions = -1;
    System.Random rnd = new System.Random();

    List<int> randomSpawnPositions = new List<int>();
    private static int randomSpawnMatrixNumber = 1;
    private int numberOfSpawnMatrices = 3;

    private AudioSource[] empireSources;
    private AudioClip[] empireClips;

    private AudioSource generalAkbarItsATrapSource;
    private AudioClip generalAkbarItsATrap;
    private float generalAkbarItsATrapVolume = 4.0f;

    private AudioSource landoBreakOffAttackSource;
    private AudioClip landoBreakOffAttack;
    private float landoBreakOffAttackVolume = 4.0f;

    private AudioSource emperorDoItSource;
    private AudioClip emperorDoIt;
    private float emperorDoItVolume = 4.0f;
    private AudioSource tarkinFireWhenReadySource;
    private AudioClip tarkinFireWhenReady;
    private float tarkinFireWhenReadyVolume = 4.0f;
    private AudioSource deathStarBlastSource;
    private AudioClip deathStarBlast;
    private float deathStarBlastVolume = 0.8f;

    private bool empireMode = true;
    static int numberOfSpawnedItems = 0;

    static bool tieFighterAllowedToAttack = true;

    static bool playSoundtrackMusic = true;

    public float enemySpeed = 15.0f;

    private GameObject boxPrefab;

    private int numberOfTimesXWingsSpawned = 0;
    private GameObject xwingPrefab;
    private GameObject awingPrefab;
    private GameObject ywingPrefab;
    private GameObject meteorite1Prefab;

    private GameObject rebelStarshipPrefab;
    private GameObject starDestroyerPrefab;

    private GameObject soundTrackOptionsPrefab;
    private static GameObject optionsDialog;

    private GameObject falconPrefab;

    private GameObject slave1Prefab;

    private GameObject viperDroidPrefab;

    private GameObject deathStarMisslePrefab;

    private bool starshipExists = false;

    public int totalSpawnsBeforeClip = 8;
    private int numberOfSpawnsSinceLastClip = 0;

    private bool clipIsPlaying = false;
    private int nextEmpireClipIndex = 0;

    private float startGamePauseDuration = 10.0f;
    private float startGameTimer;

    public static int falconIntroIndex = 2;

    // 1 - x-wing
    // 2 - a-wing
    // 3 - y-wing
    private int[] targetRebelVehicleTypes = new[] { 1, 2, 3 };

    // determine what mixture we want of rebel vehicles
    // 1 - swarm of specific vehicle type 
    // 2 - swarm of mixture of all types
    // 3 - asteroid swarm
    // 4 - millenium falcon only
    private int[] targetRebelVehicleMixture = new[] { 1, 2, 3, 4 };
    static private int vehicleMixtureLevel = 1;//default
    private int previous_vehicleMixtureLevel = -1;


    private int[] deathStarClipMixture = new[] { 1, 2, 3, 4, 5 };
    // determine which deathstar clip to play
    // 1 - Emperor DoIt
    // 2 - Tarkin YouMayFireWhenReady
    // 3 - General AkBar It's A Trap
    // 4 - Lando Break Off the Attack
    // 5 - Nothing
    private int deathStarClipLevelToPlay = 1;//default
    private int previous_DeathStarClipLevel = -1;

    private int starDestroyerSpawnAmount = 1;

    private int cavalryAttackIndex = 0;

    public static bool areAsteroidsPresent() {
        return vehicleMixtureLevel == 3;
    }

        int GetRandomCavalry()
    {
        int someValue = 1;

        bool evenValue = ((cavalryAttackIndex % 5) == 0);
        if (evenValue)
        {
            someValue = 1;
        }
        else
        {
            someValue = 2;
        }
        cavalryAttackIndex++;

        return someValue;
    }

    int GetRandomStarDestroyerSpawnPoint()
    {
        int someValue = 1;

        bool evenValue = ((starDestroyerSpawnAmount % 2) == 0);
        if (evenValue)
        {
            someValue = 1;
        }
        else
        {
            someValue = 2;
        }
        starDestroyerSpawnAmount++;

        return someValue;
    }

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

        emperorDoItSource = GameObject.FindGameObjectWithTag("emperor_DoIt_Sound").GetComponent<AudioSource>();
        emperorDoIt = emperorDoItSource.clip;

        tarkinFireWhenReadySource = GameObject.FindGameObjectWithTag("tarkin_FireWhenReady_Sound").GetComponent<AudioSource>();
        tarkinFireWhenReady = tarkinFireWhenReadySource.clip;

        generalAkbarItsATrapSource = GameObject.FindGameObjectWithTag("generalAkbar_ItsATrap_Sound").GetComponent<AudioSource>();
        generalAkbarItsATrap = generalAkbarItsATrapSource.clip;

        landoBreakOffAttackSource = GameObject.FindGameObjectWithTag("lando_BreakOffAttack_Sound").GetComponent<AudioSource>();
        landoBreakOffAttack = landoBreakOffAttackSource.clip;

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
        int newNumber = Random.Range(1, (totalSpawnPositionsAvailable + 1));
        while (newNumber == previous_currentAllowedSpawnPositions)
        {
            newNumber = Random.Range(1, (totalSpawnPositionsAvailable + 1));
        }
        currentAllowedSpawnPositions = newNumber;
        previous_currentAllowedSpawnPositions = currentAllowedSpawnPositions;

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
        return enemySpeed + 30.0f * 2.0f;
    }

    float getSlave1Speed()
    {
        return enemySpeed + 30.0f * 2.0f;
    }

    float getViperDroidSpeed()
    {
        return enemySpeed + 10.0f;
    }


    float GetRandomSpeedAdjustment()
    {
        return Random.Range(5.0f, 20.0f);
    }

    public static int getTheMatrixNumber()
    {
        return randomSpawnMatrixNumber;
    }
    void getRandomMatrixNumber()
    {
        randomSpawnMatrixNumber = Random.Range(1, (numberOfSpawnMatrices + 1));
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
        int newNumber = Random.Range(rangeBeginningValue, rangeEndingValue);

        // make sure we don't return a similar value as before
        while (newNumber == previous_vehicleMixtureLevel)
        {
            newNumber = Random.Range(rangeBeginningValue, rangeEndingValue);
        }
        previous_vehicleMixtureLevel = newNumber;
        return newNumber;
    }

    int GetRandomMixtureOfDeathStarClipsValue()
    {
        int firstIndex = 0;
        int lastIndex = deathStarClipMixture.Length - 1;

        int rangeBeginningValue = deathStarClipMixture[firstIndex];    // e.g. 1
        int rangeEndingValue = deathStarClipMixture[lastIndex] + 1; // e.g. 2 + 1

        // Random.Range(1, 3);
        // return a number (1, or 2) - does not include 3
        int newNumber = Random.Range(rangeBeginningValue, rangeEndingValue);

        // make sure we don't return a similar value as before
        while (newNumber == previous_DeathStarClipLevel)
        {
            newNumber = Random.Range(rangeBeginningValue, rangeEndingValue);
        }
        previous_DeathStarClipLevel = newNumber;
        return newNumber;
    }

    int GetRandomVehicleType()
    {


        int firstIndex = 0;
        int lastIndex = targetRebelVehicleTypes.Length - 1;

        int rangeBeginningValue = targetRebelVehicleTypes[firstIndex];    // e.g. 1
        int rangeEndingValue = targetRebelVehicleTypes[lastIndex] + 1; // e.g. 2 + 1

        // Random.Range(1, 3);
        // return a number (1, or 2) - does not include 3
        return Random.Range(rangeBeginningValue, rangeEndingValue);

    }

    void Awake()
    {
        xwingPrefab = PrefabFactory.getPrefab("miniTargetXWingFighter");
        awingPrefab = PrefabFactory.getPrefab("a-wing");
        ywingPrefab = PrefabFactory.getPrefab("yWing");
        meteorite1Prefab = PrefabFactory.getPrefab("meteorite1");

        boxPrefab = PrefabFactory.getPrefab("boxTarget");

        rebelStarshipPrefab = PrefabFactory.getPrefab("newTantiveIV");
        starDestroyerPrefab = PrefabFactory.getPrefab("starDestroyer");

        falconPrefab = PrefabFactory.getPrefab("falcon");
        deathStarMisslePrefab = PrefabFactory.getPrefab("deathStarMissle");

        debugText = GameObject.Find("debugText").GetComponent<TextMesh>();

        deathStarBlastSource = GameObject.FindGameObjectWithTag("deathStarBlast_Sound").GetComponent<AudioSource>();
        deathStarBlast = deathStarBlastSource.clip;
      
        slave1Prefab = PrefabFactory.getPrefab("slave-1");

        viperDroidPrefab = PrefabFactory.getPrefab("imperialViperDroid");
        // soundTrackOptionsPrefab
        soundTrackOptionsPrefab = PrefabFactory.getPrefab("optionsDialog");

        if (empireMode)
        {
            load_EmpireSounds();
        }

    }

    GameObject[] getAllTargetVehicles()
    {

        GameObject[] xwings;
        GameObject[] awings;
        GameObject[] ywings;
        GameObject[] meteorites;
        GameObject[] falcons;
        GameObject[] tantiveIV;

        List<GameObject> list = new List<GameObject>();

        if (empireMode)
        {
            xwings = GameObject.FindGameObjectsWithTag("targetXWing");
            meteorites = GameObject.FindGameObjectsWithTag("targetMeteorite");
            awings = GameObject.FindGameObjectsWithTag("targetAWing");
            ywings = GameObject.FindGameObjectsWithTag("targetYWing");
            falcons = GameObject.FindGameObjectsWithTag("falcon");
            tantiveIV = GameObject.FindGameObjectsWithTag("tantiveIV");
        }
        else
        {
            xwings = new GameObject[0];
            meteorites = new GameObject[0];
            awings = new GameObject[0];
            ywings = new GameObject[0];
            falcons = new GameObject[0];
            tantiveIV = new GameObject[0];
        }


        if (xwings.Length > 0)
            list.AddRange(xwings);

        if (awings.Length > 0)
            list.AddRange(awings);

        if (ywings.Length > 0)
            list.AddRange(ywings);

        if (meteorites.Length > 0)
            list.AddRange(meteorites);

        if (falcons.Length > 0)
            list.AddRange(falcons);

        if (tantiveIV.Length > 0)
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
        GameObject[] ywings;
        GameObject[] falcons;

        if (empireMode)
        {
            tvs = GameObject.FindGameObjectsWithTag("targetXWing");
            meteorites = GameObject.FindGameObjectsWithTag("targetMeteorite");
            awings = GameObject.FindGameObjectsWithTag("targetAWing");
            ywings = GameObject.FindGameObjectsWithTag("targetYWing");
            boxes = GameObject.FindGameObjectsWithTag("box");
            falcons = GameObject.FindGameObjectsWithTag("falcon");
        }
        else
        {
            tvs = new GameObject[0];
            meteorites = new GameObject[0];
            awings = new GameObject[0];
            ywings = new GameObject[0];
            boxes = new GameObject[0];
            falcons = new GameObject[0];
        }

        return (tvs.Length > 0) || (meteorites.Length > 0) || (ywings.Length > 0) || (awings.Length > 0) || (boxes.Length > 0) || (falcons.Length > 0);
    }
    void Start()
    {
        numberOfSpawnsSinceLastClip = totalSpawnsBeforeClip; //we want to play a clip at the start
        startGameTimer = startGamePauseDuration;

        //spawnOptionsDialog();
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

        //MyDebug("shootAllTargetVehicles");
        GameObject[] allTargets = getAllTargetVehicles();



       // MyDebug("shootAllTargetVehicles : " + allTargets.Length);
        if (allTargets.Length > 0)
        {

            //
            // Play a movie clip BEFORE firing death star
            //
            deathStarClipLevelToPlay = GetRandomMixtureOfDeathStarClipsValue();

            if (deathStarClipLevelToPlay == 1)
            {
                PlayEmperorDoItSound_Immediately();
                yield return new WaitForSeconds(emperorDoIt.length);
            }
            else if (deathStarClipLevelToPlay == 2)
            {
                PlayTarkinFireWhenReadySound_Immediately();
                yield return new WaitForSeconds(tarkinFireWhenReady.length);
            }
            else if (deathStarClipLevelToPlay == 3)
            {
                if (!LevelManager.areAsteroidsPresent()){
                    PlayGeneralAkbarItsATrapSound_Immediately();
                    yield return new WaitForSeconds(generalAkbarItsATrap.length);
                }

            }
            else if (deathStarClipLevelToPlay == 4)
            {
                if (!LevelManager.areAsteroidsPresent()){
                    PlayLandoBreakOffAttackSound_Immediately();
                    yield return new WaitForSeconds(landoBreakOffAttack.length);
                }

            }
            else if (deathStarClipLevelToPlay == 5)
            {
                // play nothing  
            }

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



        }//if
        else
        {
            MyDebug("No targets found");
        }

        onDifferentThread_PauseAfterShootingMissle();

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

        //MyDebug("About to shoot all missles");
        shootAllMisslesNow();


    }

    void handleLaunchingOptionsDialog()
    {
        launchTheOptionsDialog = "launched";// we only want launch the dialog once
       // MyDebug("spawnOptionsDialog()");
        try {
          spawnOptionsDialog();
        }
        catch(System.Exception e) {
          MyDebug(e.Message);
        }
    }

    void handleCavalryAttack_StarDestroyer() {

        int sdSpawnPosition = GetRandomStarDestroyerSpawnPoint();
        if (sdSpawnPosition == 1){
           spawnStarDestroyer_AtPositionOne();
        }
        else if (sdSpawnPosition == 2) {
           spawnStarDestroyer_AtPositionTwo();
        }
    }

    void handleCavalryAttack_BobaFett() {

       spawnSlave1();

    }

    void handleCavalryAttack_ViperDroidSwarm() {

       spawnViperDroid();

    }

    void handleLaunchingCavalryAttack()
    {
        launchTheCavalryAttack = "launched";// we only want launch the attack once for the initial request

        int cavalryAttackType = GetRandomCavalry();

        if (cavalryAttackType == 1)
          //handleCavalryAttack_StarDestroyer();
          handleCavalryAttack_ViperDroidSwarm();
        else if (cavalryAttackType == 2)
          //handleCavalryAttack_BobaFett();
          handleCavalryAttack_ViperDroidSwarm();
        
    }

    void FixedUpdate()
    {   
        //MyDebug("theValue : " + launchTheOptionsDialog);

        try {
           handleInputAndroid();
        }
        catch (System.Exception e) {
           MyDebug(e.Message);
        }
        

        if (launchTheCavalryAttack == "launch")
        {
            handleLaunchingCavalryAttack();
        }

        if (launchTheDSAttack == "launch")
        {
            //MyDebug("calling: handleLaunchingDeathStarAttack ");
            handleLaunchingDeathStarAttack();
        }

        if (launchTheOptionsDialog == "launch")
        {
            handleLaunchingOptionsDialog();
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

    void spawnSlave1()
    {

        try
        {
            
            string spawnPositionNumber = "slave1SpawnPosition" + randomSpawnMatrixNumber;
            GameObject enemySpawner = GameObject.Find(spawnPositionNumber);
            float x = enemySpawner.transform.position.x;
            float y = enemySpawner.transform.position.y;
            float z = enemySpawner.transform.position.z;
            GameObject go = (GameObject)Instantiate(slave1Prefab, new Vector3(x, y, z), enemySpawner.transform.rotation);
            //GameObject go = (GameObject)Instantiate(viperDroidPrefab, new Vector3(x, y, z), enemySpawner.transform.rotation);
            go.transform.RotateAround(go.transform.position, go.transform.up, 180f);
            go.GetComponent<Rigidbody>().velocity = getSlave1Speed() * enemySpawner.transform.forward * -1f * Time.deltaTime;
          
        }
        catch (System.Exception e)
        {
            MyDebug("spawnSlave1 error: " + e.ToString());
        }


    }

    void spawnViperDroid()
    {

        try
        {
            string spawnPositionNumber = "vp_spawn18";
            GameObject enemySpawner = GameObject.Find(spawnPositionNumber);
            float x = enemySpawner.transform.position.x;
            float y = enemySpawner.transform.position.y;
            float z = enemySpawner.transform.position.z;

            GameObject go = (GameObject)Instantiate(viperDroidPrefab, new Vector3(x, y, z), enemySpawner.transform.rotation);
            go.GetComponent<Rigidbody>().velocity = getViperDroidSpeed() * enemySpawner.transform.forward * 1f * Time.deltaTime;
          
        }
        catch (System.Exception e)
        {
            MyDebug("spawnViperDroid error: " + e.ToString());
        }


    }
    void spawnNormalFalcon()
    {
        int position = 5; // middle position of any matrix
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

    void spawnNewYWingAtPosition(int position)
    {

        string spawnPositionNumber = "spawn" + randomSpawnMatrixNumber + "" + position;
        GameObject enemySpawner = GameObject.Find(spawnPositionNumber);
        float x = enemySpawner.transform.position.x;
        float y = enemySpawner.transform.position.y;
        float z = enemySpawner.transform.position.z;
        GameObject go = (GameObject)Instantiate(ywingPrefab, new Vector3(x, y, z), enemySpawner.transform.rotation);
        go.transform.RotateAround(go.transform.position, go.transform.up, 180f);
        go.GetComponent<Rigidbody>().velocity = getSlightlyRandomizedSpeed() * enemySpawner.transform.forward * -1f * Time.deltaTime;
        incrementNumSpawned();
    }

    void spawnOptionsDialog()
    {
        GameObject spawner = GameObject.Find("soundTrackOptionsSpawnPoint");
        float x = spawner.transform.position.x;
        float y = spawner.transform.position.y;
        float z = spawner.transform.position.z;
        
        optionsDialog = (GameObject)Instantiate(soundTrackOptionsPrefab, new Vector3(x, y, z), spawner.transform.rotation);
      
       
      
    }

    void spawnStarDestroyer_AtPositionOne()
    {

        float theSpeed = 30.0f;
        string spawnPositionNumber = "starDestroyerSpawnPoint" + 1;
        GameObject spawner = GameObject.Find(spawnPositionNumber);
        float x = spawner.transform.position.x;
        float y = spawner.transform.position.y;
        float z = spawner.transform.position.z;
        GameObject go = (GameObject)Instantiate(starDestroyerPrefab, new Vector3(x, y, z), spawner.transform.rotation);
        go.SendMessage("setSpawnPositionIndex", 1);
        go.GetComponent<Rigidbody>().velocity = theSpeed * spawner.transform.forward * Time.deltaTime;

    }

    void spawnStarDestroyer_AtPositionTwo()
    {

        float theSpeed = 30.0f;
        string spawnPositionNumber = "starDestroyerSpawnPoint" + 2;
        GameObject spawner = GameObject.Find(spawnPositionNumber);
        float x = spawner.transform.position.x;
        float y = spawner.transform.position.y;
        float z = spawner.transform.position.z;
        GameObject go = (GameObject)Instantiate(starDestroyerPrefab, new Vector3(x, y, z), spawner.transform.rotation);
        go.SendMessage("setSpawnPositionIndex", 2);
        go.GetComponent<Rigidbody>().velocity = theSpeed * spawner.transform.forward * Time.deltaTime;

    }

    void incrementNumSpawned()
    {
        lock (olock)
        {
            numberOfSpawnedItems++;
        }
    }

    public static GameObject[] filterGameObjectsInFrontOfViperDroid(GameObject viperDroid,GameObject[] given)
    {

        List<GameObject> list = new List<GameObject>();

        // get viper droid's Z position
        GameObject shooter = viperDroid;
        float shooterZ = shooter.transform.position.z;

        if (given != null)
        {
            for (int i = 0; i < given.Length; i++)
            {
                if (given[i] != null)
                {
                    float givenZ = given[i].transform.position.z;
                    // opposite logic to vehicles destroying themselves
                    // shortly after passing shooter.
                    if (givenZ >= (shooterZ + 0.5f))
                    {
                        list.Add(given[i]);
                    }
                }
            }
        }//if

        return list.ToArray();
    }

    public static GameObject[] filterGameObjectsInFrontOfPlayer(GameObject[] given)
    {

        List<GameObject> list = new List<GameObject>();

        // get player's Z position
        GameObject shooter = GameObject.FindGameObjectWithTag("PlayerShooter");
        float shooterZ = shooter.transform.position.z;

        if (given != null)
        {
            for (int i = 0; i < given.Length; i++)
            {
                if (given[i] != null)
                {
                    float givenZ = given[i].transform.position.z;
                    // opposite logic to vehicles destroying themselves
                    // shortly after passing shooter.
                    if (givenZ >= (shooterZ + 0.5f))
                    {
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



    public static void starDestroyerAttackFinished()
    {
        lock (olock)
        {

            // when star destroyer is finished its mission, it calls this method
            // to allow the LevelManager to know it is done.
            launchTheCavalryAttack = "";

        }

    }

        public static void bobaFettAttackFinished()
    {
        lock (olock)
        {

            // when Boba Fett is finished his mission, it calls this method
            // to allow the LevelManager to know it is done.
            launchTheCavalryAttack = "";

        }

    }
        public static void viperSwarmAttackFinished()
    {
        lock (olock)
        {

            // when viper droid swarm is finished the mission, it calls this method
            // to allow the LevelManager to know it is done.
            launchTheCavalryAttack = "";

        }

    }    
    public static void launchCavalryAttack()
    {
        lock (olock)
        {

            if (launchTheCavalryAttack == "")
            {
                launchTheCavalryAttack = "launch";
            }

        }

    }
    public static void launchDeathStarAttack()
    {

        lock (olock)
        {

            //MyDebug("deathstar attack requested");
            if (launchTheDSAttack == "")
            {
                launchTheDSAttack = "launch";
            }
        }


    }

    public static void launchOptionsDialog()
    {

        lock (olock)
        {

            //MyDebug("launchOptionsDialog()");
            if (launchTheOptionsDialog == "")
            {
                launchTheOptionsDialog = "launch";
            }
        }


    }

        public static void closeOptionsDialog()
    {

        lock (olock)
        {
              if (optionsDialog != null){
                 Destroy(optionsDialog);
              }
              launchTheOptionsDialog = "";
              
        }

    }

    public static void setTieFighterAllowedToShoot(bool value)
    {
        lock (olock)
        {
            tieFighterAllowedToAttack = value;
        }
    }

    public static bool getPlaySoundtrackMusic()
    {
        lock (olock)
        {
            return playSoundtrackMusic;
        }
    }

    public static void setPlaySoundtrackMusic(bool value)
    {
        lock (olock)
        {
            playSoundtrackMusic = value;
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
                else if (vehicleType == 3)
                {
                    spawnNewYWingAtPosition(pos);
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
                else if (vehicleType == 3)
                {
                    spawnNewYWingAtPosition(pos);
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
        else if (vehicleMixtureLevel == 4)
        {
            // falcon flyby
            spawnNormalFalcon();
        }

        numberOfSpawnsSinceLastClip++; // keep track of spawn events (not number of targets spawned)
    }

    static void MyDebug(string someText)
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

    void PlayLandoBreakOffAttackSound_Immediately()
    {
        landoBreakOffAttackSource.PlayOneShot(landoBreakOffAttack, landoBreakOffAttackVolume);

    }

    void PlayGeneralAkbarItsATrapSound_Immediately()
    {
        generalAkbarItsATrapSource.PlayOneShot(generalAkbarItsATrap, generalAkbarItsATrapVolume);

    }

    void PlayEmperorDoItSound_Immediately()
    {
        emperorDoItSource.PlayOneShot(emperorDoIt, emperorDoItVolume);

    }

    void PlayTarkinFireWhenReadySound_Immediately()
    {
        tarkinFireWhenReadySource.PlayOneShot(tarkinFireWhenReady, tarkinFireWhenReadyVolume);

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
                     
                    if (name == "DeathStarDisc"){  
                        //MyDebug("touched: DeathStar");
                        LevelManager.launchOptionsDialog();
                    }
                    else if (name == "closeIcon"){     
                        if (optionsDialog != null){
                            LevelManager.closeOptionsDialog();
                        }
                    }                    
                    else if (name == "SoundCheckBoxOn"){ 
                        //MyDebug("touched: SoundCheckBoxOn");
                        LevelManager.setPlaySoundtrackMusic(false);      
                    }
                    else if (name == "SoundCheckBoxOff") { 
                      // MyDebug("touched: SoundCheckBoxOff");    
                       LevelManager.setPlaySoundtrackMusic(true);  
                    }
                }//if
            }//if


      }// 

   }

}