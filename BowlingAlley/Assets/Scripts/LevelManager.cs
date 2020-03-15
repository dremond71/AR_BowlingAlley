using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    private static object olock = new object ();

    private bool debug = false;

    private TextMesh debugText;

    private int totalSpawnPositionsAvailable = 9;
    private int currentAllowedSpawnPositions = 0;
    System.Random rnd = new System.Random ();

    List<int> randomSpawnPositions = new List<int> ();

    private AudioSource[] empireSources;
    private AudioClip[] empireClips;

    private bool empireMode = true;
    static int numberOfSpawnedItems = 0;

    public float enemySpeed = 35.0f;

    private GameObject boxPrefab;

    private GameObject xwingPrefab;

    private GameObject rebelStarshipPrefab;

    private bool starshipExists = false;

    public int totalSpawnsBeforeClip = 3;
    private int numberOfSpawnsSinceLastClip = 0;

    private bool clipIsPlaying = false;
    private int nextEmpireClipIndex = 0;

    private float startGamePauseDuration = 10.0f;
    private float startGameTimer;

    void load_EmpireSounds () {
        empireSources = new AudioSource[3];
        empireClips = new AudioClip[3];

        empireSources[0] = GameObject.FindGameObjectWithTag ("emperorEndOfAlliance_Sound").GetComponent<AudioSource> ();
        empireClips[0] = empireSources[0].clip;

        empireSources[1] = GameObject.FindGameObjectWithTag ("emperorDesign_Sound").GetComponent<AudioSource> ();
        empireClips[1] = empireSources[1].clip;

        empireSources[2] = GameObject.FindGameObjectWithTag ("emperorLaugh_Sound").GetComponent<AudioSource> ();
        empireClips[2] = empireSources[2].clip;

    }

    int getNextEmpireClipIndex () {
        int value = nextEmpireClipIndex;
        nextEmpireClipIndex++;
        if (nextEmpireClipIndex > (empireSources.Length - 1)) {
            nextEmpireClipIndex = 0;
        }

        return value;
    }

    float getSlightlyRandomizedSpeed () {
        return enemySpeed + GetRandomSpeedAdjustment ();

    }

    void setRandomSpawnAmount () {
        // choose a random amount between 1 and 9
        //currentAllowedSpawnPositions = rnd.Next(1, totalSpawnPositionsAvailable);
        currentAllowedSpawnPositions = rnd.Next (1, 5); // nine enemies is too much ;S
        MyDebug ("Chosen amount : " + currentAllowedSpawnPositions);
    }
    void getRandomSpawnPositions () {
        randomSpawnPositions.Clear ();

        int totalCollected = 0;

        while (totalCollected < currentAllowedSpawnPositions) {
            int someNumber = GetRandomSpawnPosition ();
            if (!randomSpawnPositions.Contains (someNumber)) {
                randomSpawnPositions.Add (someNumber);
                totalCollected++;
            } //if

        }

    }

    float GetRandomSpeedAdjustment () {

        return Random.Range (0.0f, 20.0f);

    }
    int GetRandomSpawnPosition () {
        return rnd.Next (1, totalSpawnPositionsAvailable);
    }

    void Awake () {
        xwingPrefab = PrefabFactory.getPrefab ("miniTargetXWingFighter");
        boxPrefab = PrefabFactory.getPrefab ("boxTarget");
        rebelStarshipPrefab = PrefabFactory.getPrefab ("rebelTantiveIV");
        debugText = GameObject.Find ("debugText").GetComponent<TextMesh> ();

        if (empireMode) {
            load_EmpireSounds ();
        }

    }

    bool targetsExist () {
        GameObject[] tvs;

        if (empireMode) {
            tvs = GameObject.FindGameObjectsWithTag ("targetXWing");
        } else {
            tvs = new GameObject[0];
        }

        return (tvs.Length > 0);
    }
    void Start () {
        numberOfSpawnsSinceLastClip = totalSpawnsBeforeClip; //we want to play a clip at the start
        startGameTimer = startGamePauseDuration;
    }

    void spawnRebelStarship () {

        GameObject starshipSpawner = GameObject.FindGameObjectWithTag ("starshipSpawnPoint");
        float x = starshipSpawner.transform.position.x;
        float y = starshipSpawner.transform.position.y;
        float z = starshipSpawner.transform.position.z;
        GameObject go = (GameObject) Instantiate (rebelStarshipPrefab, new Vector3 (x, y, z), starshipSpawner.transform.rotation);

    }
    void Update () {

        if (startGameTimer > 0.0f)
            startGameTimer -= Time.deltaTime;

        if (startGameTimer <= 0f) {

            if (!starshipExists) {
                starshipExists = true;
                spawnRebelStarship ();
            }

            if (targetsExist ()) {
                // spawned targets exist; and making noise

                // no spawning and no clips to play
            } else {
                // no targets exist; and there's silence 

                // it's time to play a sound clip, then spawn targets
                if (numberOfSpawnsSinceLastClip == totalSpawnsBeforeClip) {

                    if (!clipIsPlaying)
                        playClipOnDifferentThreadThenAllowSpawnedTargets ();
                } else {
                    //it's time to spawn targets, and NOT play a clip
                    handleSpawning ();
                }

            }

        } //if - start game

    }

    void handleSpawning () {
        setRandomSpawnAmount (); //determine amount to spawn
        spawnNewTargets ();
    }
    void spawnNewXWingAtPosition (int position) {

        string spawnPositionNumber = "spawn" + position;
        GameObject enemySpawner = GameObject.Find (spawnPositionNumber);
        float x = enemySpawner.transform.position.x;
        float y = enemySpawner.transform.position.y;
        float z = enemySpawner.transform.position.z;
        GameObject go = (GameObject) Instantiate (xwingPrefab, new Vector3 (x, y, z), enemySpawner.transform.rotation);
        go.transform.RotateAround (go.transform.position, go.transform.up, 180f);
        go.GetComponent<Rigidbody> ().velocity = getSlightlyRandomizedSpeed () * enemySpawner.transform.forward * -1f * Time.deltaTime;
        incrementNumSpawned ();
    }

    void incrementNumSpawned () {
        lock (olock) {
            numberOfSpawnedItems++;
        }
    }

    public static void decrementNumSpawned () {
        lock (olock) {
            numberOfSpawnedItems--;
        }
    }

    void spawnNewBoxAtPosition (int position) {

        string spawnPositionNumber = "spawn" + position;
        GameObject enemySpawner = GameObject.Find (spawnPositionNumber);
        float x = enemySpawner.transform.position.x;
        float y = enemySpawner.transform.position.y;
        float z = enemySpawner.transform.position.z;
        GameObject go = (GameObject) Instantiate (boxPrefab, new Vector3 (x, y, z), enemySpawner.transform.rotation);
        go.GetComponent<Rigidbody> ().velocity = getSlightlyRandomizedSpeed () * enemySpawner.transform.forward * -1f * Time.deltaTime;
        incrementNumSpawned ();
    }
    void spawnNewTargets () {
        getRandomSpawnPositions ();
        numberOfSpawnsSinceLastClip++; // keep track of spawn events (not number of targets spawned)
        foreach (var pos in randomSpawnPositions) {
            //spawnNewBoxAtPosition(pos);
            spawnNewXWingAtPosition (pos);
        }

    }

    void MyDebug (string someText) {

        if (debugText != null & debug) {
            debugText.text = someText;
        }

    }

    void playClipOnDifferentThreadThenAllowSpawnedTargets () {

        clipIsPlaying = true;
        AudioSource audioSource = null;
        AudioClip audioClip = null;

        if (empireMode) {
            int index = getNextEmpireClipIndex ();
            audioSource = empireSources[index];
            audioClip = empireClips[index];
        }

        StartCoroutine (PlayClipThenAllowSpawnedTargets (audioSource, audioClip));

    }
    IEnumerator PlayClipThenAllowSpawnedTargets (AudioSource audioSource, AudioClip audioClip) {
        //https://answers.unity.com/questions/904981/how-to-play-an-audio-file-after-another-finishes.html

        yield return new WaitForSeconds (3.0f);
        audioSource.PlayOneShot (audioClip, 3.0f);
        yield return new WaitForSeconds (audioClip.length);
        yield return new WaitForSeconds (1.5f);
        numberOfSpawnsSinceLastClip = 0; //reset to zero before we play a sound clip.
        clipIsPlaying = false;

    }
}