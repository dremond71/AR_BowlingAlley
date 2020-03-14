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

    float getSlightlyRandomizedSpeed()
    {
        return enemySpeed + GetRandomSpeedAdjustment();

    }

    void setRandomSpawnAmount()
    {
        // choose a random amount between 1 and 9
        //currentAllowedSpawnPositions = rnd.Next(1, totalSpawnPositionsAvailable);
        currentAllowedSpawnPositions = rnd.Next(1, 5); // nine enemies is too much ;S
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

    static int numberOfSpawnedItems = 0;

    public float enemySpeed = 35.0f;

    public float spawningPauseDuration = 15.0f;
    private float pauseTimer;

    private GameObject boxPrefab;

    private GameObject xwingPrefab;

    private GameObject rebelStarshipPrefab;

    private bool starshipExists = false;
    private int spawnIndexChosen = 0;

    private bool spawnedTemp = false;
    void Awake()
    {
        xwingPrefab = PrefabFactory.getPrefab("miniTargetXWingFighter");
        boxPrefab = PrefabFactory.getPrefab("boxTarget");
        rebelStarshipPrefab = PrefabFactory.getPrefab("rebelTantiveIV");
        debugText = GameObject.Find("debugText").GetComponent<TextMesh>();
    }
    void Start()
    {
        pauseTimer = spawningPauseDuration;
    }

    // void Update()
    // {
    //     if (!spawnedTemp)
    //     {
    //         spawnedTemp = true;
    //         GameObject ss = GameObject.FindGameObjectWithTag("tempSpawnPosition");
    //         float x = ss.transform.position.x;
    //         float y = ss.transform.position.y;
    //         float z = ss.transform.position.z;
    //         GameObject go = (GameObject)Instantiate(xwingPrefab, new Vector3(x, y, z), ss.transform.rotation);
    //         go.transform.RotateAround(go.transform.position, go.transform.up, 180f);

    //     }
    // }
    // Update is called once per frame

    void spawnRebelStarship()
    {

        GameObject starshipSpawner = GameObject.FindGameObjectWithTag("starshipSpawnPoint");
        float x = starshipSpawner.transform.position.x;
        float y = starshipSpawner.transform.position.y;
        float z = starshipSpawner.transform.position.z;
        GameObject go = (GameObject)Instantiate(rebelStarshipPrefab, new Vector3(x, y, z), starshipSpawner.transform.rotation);

    }
    void Update()
    {

        if (!starshipExists)
        {
            starshipExists = true;
            spawnRebelStarship();
        }

        //MyDebug("Spawn pos: " + spawnIndexChosen + ", # created: " + numberOfSpawnedItems);
        pauseTimer -= Time.deltaTime;
        if (pauseTimer <= 0f)
        {
            pauseTimer = spawningPauseDuration;
            if (numberOfSpawnedItems < 100)
            {
                setRandomSpawnAmount(); //determine amount to spawn
                spawnNewTargets();

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
    void spawnNewTargets()
    {
        getRandomSpawnPositions();
        foreach (var pos in randomSpawnPositions)
        {
            //spawnNewBoxAtPosition(pos);
            spawnNewXWingAtPosition(pos);
        }

    }

    void MyDebug(string someText)
    {

        if (debugText != null & debug)
        {
            debugText.text = someText;
        }

    }
}