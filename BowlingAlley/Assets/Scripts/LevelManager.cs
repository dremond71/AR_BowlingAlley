using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelManager : MonoBehaviour
{

    public int totalSpawnPositions = 9;
    System.Random rnd = new System.Random();
    int GetRandomSpawnPosition()
    {
        return rnd.Next(1, totalSpawnPositions);
    }

    int numberOfSpawnedItems = 0;

    public float enemySpeed = 50.0f;

    private float pauseDuration = 5.0f;
    private float pauseTimer;

    private GameObject boxPrefab;

    private TextMesh debugText;
    private bool debug = false;

    private int spawnIndexChosen = 0;
    void Awake()
    {
        boxPrefab = PrefabFactory.getPrefab("boxTarget");
        debugText = GameObject.Find("debugText").GetComponent<TextMesh>();
    }
    void Start()
    {
        pauseTimer = pauseDuration;
    }

    // Update is called once per frame
    void Update()
    {
        MyDebug("Spawn pos: " + spawnIndexChosen + ", # created: " + numberOfSpawnedItems);
        pauseTimer -= Time.deltaTime;
        if (pauseTimer <= 0f)
        {
            pauseTimer = pauseDuration;
            if (numberOfSpawnedItems < 100)
            {
                spawnNewBox();
                numberOfSpawnedItems++;
            }
        }


    }

    void spawnNewBox()
    {


        //get random number, then find the spawn position number with it.
        spawnIndexChosen = GetRandomSpawnPosition();
        string spawnPositionNumber = "spawn" + spawnIndexChosen;
        GameObject enemySpawner = GameObject.Find(spawnPositionNumber);
        float x = enemySpawner.transform.position.x;
        float y = enemySpawner.transform.position.y;
        float z = enemySpawner.transform.position.z;
        GameObject go = (GameObject)Instantiate(boxPrefab, new Vector3(x, y, z), enemySpawner.transform.rotation);
        go.GetComponent<Rigidbody>().velocity = enemySpeed * enemySpawner.transform.forward * -1f * Time.deltaTime;
    }

    void MyDebug(string someText)
    {

        if (debugText != null & debug)
        {
            debugText.text = someText;
        }

    }
}
