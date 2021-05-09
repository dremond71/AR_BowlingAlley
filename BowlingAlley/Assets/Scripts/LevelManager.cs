using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    private static object olock = new object();

    private bool debug = false;

    private TextMesh debugText;

    private int totalSpawnPositionsAvailable = 9;
    private int currentAllowedSpawnPositions = 0;
    System.Random rnd = new System.Random();

    List<int> randomSpawnPositions = new List<int>();

    private AudioSource[] empireSources;
    private AudioClip[] empireClips;

    private bool empireMode = true;
    static int numberOfSpawnedItems = 0;

    static bool tieFighterAllowedToAttack = true;
    public float enemySpeed = 35.0f;

    private GameObject boxPrefab;

    private int numberOfTimesXWingsSpawned = 0;
    private GameObject xwingPrefab;
    private GameObject meteorite1Prefab;

    private GameObject rebelStarshipPrefab;

    private GameObject falconPrefab;
    private bool falconExists = false;

    private bool starshipExists = false;

    public int totalSpawnsBeforeClip = 8;
    private int numberOfSpawnsSinceLastClip = 0;

    private bool clipIsPlaying = false;
    private int nextEmpireClipIndex = 0;

    private float startGamePauseDuration = 10.0f;
    private float startGameTimer;

    public static int falconIntroIndex = 2;

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

    float getSlightlyRandomizedSpeed()
    {
        return enemySpeed + GetRandomSpeedAdjustment();

    }

    void setRandomSpawnAmount()
    {
        // choose a random amount between 1 and 9
        //currentAllowedSpawnPositions = rnd.Next(1, totalSpawnPositionsAvailable);
        currentAllowedSpawnPositions = rnd.Next(1, 7); // nine enemies is too much ;S
        MyDebug("Chosen amount : " + currentAllowedSpawnPositions);
    }
    void getRandomSpawnPositions()
    {
        randomSpawnPositions.Clear();

        int totalCollected = 0;

        while (totalCollected < currentAllowedSpawnPositions)
        {
            int someNumber = GetRandomSpawnPosition();
            if (!randomSpawnPositions.Contains(someNumber))
            {
                randomSpawnPositions.Add(someNumber);
                totalCollected++;
            } //if

        }

    }

    float GetRandomSpeedAdjustment()
    {

        return Random.Range(0.0f, 20.0f);

    }
    int GetRandomSpawnPosition()
    {
        return rnd.Next(1, totalSpawnPositionsAvailable);
    }

    int GetRandomVehicleType()
    {
        // let's return only 2 possible values right now

        // Random.value returns a number between 0 and 1
        if(Random.value<0.5f){
          return 1;
        }
        else {
          return 2;
        }
    }

    void Awake()
    {
        xwingPrefab = PrefabFactory.getPrefab("miniTargetXWingFighter");
        meteorite1Prefab = PrefabFactory.getPrefab("meteorite1");

        boxPrefab = PrefabFactory.getPrefab("boxTarget");
        //rebelStarshipPrefab = PrefabFactory.getPrefab ("rebelTantiveIV");
        rebelStarshipPrefab = PrefabFactory.getPrefab("newTantiveIV");
        falconPrefab = PrefabFactory.getPrefab("falcon");
        debugText = GameObject.Find("debugText").GetComponent<TextMesh>();

        if (empireMode)
        {
            load_EmpireSounds();
        }

    }

    bool targetsExist()
    {
        GameObject[] tvs;
        GameObject[] boxes;
        GameObject[] meteorites;
        GameObject[] falcons;

        if (empireMode)
        {
            tvs = GameObject.FindGameObjectsWithTag("targetXWing");
            meteorites = GameObject.FindGameObjectsWithTag("targetMeteorite");
            boxes = GameObject.FindGameObjectsWithTag("box");
            falcons = GameObject.FindGameObjectsWithTag("falcon");
        }
        else
        {
            tvs = new GameObject[0];
            meteorites = new GameObject[0];
            boxes = new GameObject[0];
            falcons = new GameObject[0];
        }

        return (tvs.Length > 0) || (meteorites.Length > 0) || (boxes.Length > 0) || (falcons.Length > 0);
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

    }

    void spawnRebelStarship()
    {

        GameObject starshipSpawner = GameObject.FindGameObjectWithTag("starshipSpawnPoint");
        float x = starshipSpawner.transform.position.x;
        float y = starshipSpawner.transform.position.y;
        float z = starshipSpawner.transform.position.z;
        GameObject go = (GameObject)Instantiate(rebelStarshipPrefab, new Vector3(x, y, z), starshipSpawner.transform.rotation);

    }

    void Update22()
    {

        if (!falconExists)
        {
            falconExists = true;
            setTieFighterAllowedToShoot(false); // let hero say something before battle starts
            spawnFalconAtSpawnPosition1();
        }

    }
    void Update()
    {

        if (startGameTimer > 0.0f)
            startGameTimer -= Time.deltaTime;

        if (startGameTimer <= 0f)
        {

            MyDebug("numberOfSpawnsSinceLastClip==" + numberOfSpawnsSinceLastClip);
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

    void spawnHero()
    {

        setTieFighterAllowedToShoot(false); // let hero say something before battle starts
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

            // randon number generator for a choice of two things
            bool value = ((numberOfTimesXWingsSpawned % 3) == 0);

            if (value)
            {
                spawnHero();
            }
            else
            {
                spawnSquadronOfXWings();

            }

        }

    }
    void spawnNewXWingAtPosition(int position)
    {

        string spawnPositionNumber = "spawn" + position;
        GameObject enemySpawner = GameObject.Find(spawnPositionNumber);
        float x = enemySpawner.transform.position.x;
        float y = enemySpawner.transform.position.y;
        float z = enemySpawner.transform.position.z;
        GameObject go = (GameObject)Instantiate(xwingPrefab, new Vector3(x, y, z), enemySpawner.transform.rotation);
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

        string spawnPositionNumber = "spawn" + position;
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

        string spawnPositionNumber = "spawn" + position;
        GameObject enemySpawner = GameObject.Find(spawnPositionNumber);
        float x = enemySpawner.transform.position.x;
        float y = enemySpawner.transform.position.y;
        float z = enemySpawner.transform.position.z;
        GameObject go = (GameObject)Instantiate(meteorite1Prefab, new Vector3(x, y, z), enemySpawner.transform.rotation);
        go.GetComponent<Rigidbody>().velocity = getSlightlyRandomizedSpeed() * enemySpawner.transform.forward * -1f * Time.deltaTime;
        incrementNumSpawned();
    }

    void spawnNewTargets()
    {
        getRandomSpawnPositions();

        // get an integer: 1, or 2
        int vehicleType = GetRandomVehicleType();
        
        foreach (var pos in randomSpawnPositions)
        {
            if (vehicleType == 1){
              spawnNewXWingAtPosition(pos);
            }
            else if (vehicleType == 2) {
              spawnNewMeteoriteAtPosition(pos);
            }
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