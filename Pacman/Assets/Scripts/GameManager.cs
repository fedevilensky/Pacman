using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Cinemachine;
using System;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public float gunSpawnRate = 15f;
    public float enemySpawnRate = 20f;
    public float gunTextTimerLimit = 2f;
    public float restartLevelDelay = 2f;
    public float marginError = 0.1f;
    public float gameSpeed = 2f;
    public float gunRespawnTime;
    public int totalItemQuantity;
    public List<Vector3> waypointList;
    public Tilemap wallMap;
    public Tilemap waypoints;
    public CinemachineVirtualCamera camera;
    public GameObject farPoint;
    public GameObject endGame;
    public GameObject gunAppearedText;
    public GameObject playerPrefab;
    public GameObject collectiblePrefab;
    public GameObject gunPrefab;
    public GameObject enemyPrefab;
    public Text levelText;
    [HideInInspector] public int itemsCollected;
    [HideInInspector] public bool enemyDead;
    [HideInInspector] public bool gameOver;
    [HideInInspector] public bool hasEnded;
    [HideInInspector] public bool gunIsSpawned;
    [HideInInspector] public bool playerHasGun;
    [HideInInspector] public bool loading;
    [HideInInspector] public GameObject enemy;
    [HideInInspector] public GameObject player;
    [HideInInspector] public GameObject[] collectibles;
    [HideInInspector] public TilemapManager tileManager;



    private HashSet<int> waypointsUsed;
    private int level = 1;
    private float lastGunSpawn;
    private GameObject gun;
    private float gunTextTimer;
    private GameObject endGameInstance;


    /**Borrar**/
    /////////////////////////////////////////////////////
    public String myError = "";
    public string boolMapPrint;
    /////////////////////////////////////////////////////




    private void Awake() {
        if (instance == null) {
            instance = this;

        }
        else if (instance != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        StartLevel();
    }

    private void StartLevel() {
        loading = true;
        levelText.text = "Collect the " + totalItemQuantity + " items";
        camera.Follow = farPoint.transform;
        Invoke("InitGame", restartLevelDelay);
    }

    private void InitGame() {
        levelText.text = "";
        DestroyInstances();
        ResetBools();
        waypointList = new List<Vector3>();
        tileManager = gameObject.GetComponent<TilemapManager>();
        collectibles = new GameObject[totalItemQuantity];
        tileManager.DrawMap();

        waypointsUsed = new HashSet<int>();
        RandomSpawns();
        AssignCamera();



        lastGunSpawn = 1f;
        gunRespawnTime = 0f;
        GameObject[] enemies = new GameObject[1];
        enemies[0] = enemy;
        enemy.GetComponent<Enemy>().strategy = new StrategyWanderTest();
        SoundManager.instance.enemies = enemies;
        gunAppearedText.SetActive(false);
        itemsCollected = 0;
        loading = false;
    }

    private void ResetBools() {
        enemyDead = false;
        gameOver = false;
        hasEnded = false;
        gunIsSpawned = false;
        playerHasGun = false;
    }

    private void DestroyInstances() {
        Destroy(gun);
        Destroy(enemy);
        Destroy(player);
        foreach (GameObject c in collectibles) {
            Destroy(c);
        }
    }

    private void Update() {
        if (!loading) {
            if (enemyDead) {
                gunRespawnTime -= Time.deltaTime;
            }
            if (gunRespawnTime <= 0f && enemyDead) {
                Destroy(enemy);
                int spawnPoint = UnityEngine.Random.Range(0, waypointList.Count);
                InstantiatePrefab(spawnPoint, out enemy, enemyPrefab);
                enemyDead = false;
            }
            if (gameOver && !hasEnded) {
                hasEnded = true;
                GameOver();
            }
            else if (!gameOver && hasEnded) {
                Victory();
            }
        }
    }

    public void EquipGun() {
        playerHasGun = true;
        enemy.GetComponent<Enemy>().strategy = new StrategyEscapeTest();
    }

    public void DestroyGun() {
        Destroy(gun);
        enemy.GetComponent<Enemy>().strategy = new StrategyWanderTest();
    }

    void FixedUpdate() {
        if (loading)
            return;
        if (!gunIsSpawned) {
            gunTextTimer = gunTextTimerLimit;
            lastGunSpawn -= Time.deltaTime;
            if (lastGunSpawn < 0) {
                gunIsSpawned = true;

                lastGunSpawn = gunSpawnRate;
                SpawnGun();
            }
        }
        else {
            if (gunTextTimer > 0) {
                gunAppearedText.SetActive(true);
                gunTextTimer -= Time.deltaTime;
            }
            else {
                gunAppearedText.SetActive(false);
            }
        }
    }

    private void SpawnGun() {
        int spawnPoint;
        do {
            spawnPoint = UnityEngine.Random.Range(0, waypointList.Count);
        } while (!FarFromPlayerSpawn(spawnPoint));
        InstantiatePrefab(spawnPoint, out gun, gunPrefab);
    }

    public bool FarFromPlayerSpawn(int spawnPoint) {
        float minDist = 9f;
        
        float thisDistance = Vector3.Distance(player.transform.position, GetSpawn(spawnPoint));
        return thisDistance > minDist;
    }

    private void AssignCamera() {
        camera.Follow = player.transform;
    }

    private void RandomSpawns() {
        //aca instanciamos al jugador
        int spawnPoint = UnityEngine.Random.Range(0, waypointList.Count);
        waypointsUsed.Add(spawnPoint);
        InstantiatePrefab(spawnPoint, out player, playerPrefab);
        //aca instanciamos a el/los enemigos
        do {
            spawnPoint = UnityEngine.Random.Range(0, waypointList.Count);
        } while (!FarFromPlayerSpawn(spawnPoint));
        waypointsUsed.Add(spawnPoint);
        InstantiatePrefab(spawnPoint, out enemy, enemyPrefab);

        //aca poner el spawn de los objetos
        float minDistance = 9f;
        for (int i = 0; i < totalItemQuantity; i++) {
            int n = 0;
            do {
                spawnPoint = UnityEngine.Random.Range(0, waypointList.Count);
                n++;

                if (n == 500) {
                    minDistance--;
                }
            } while (!ValidCollectibleSpawn(spawnPoint, minDistance));

            waypointsUsed.Add(spawnPoint);
            InstantiatePrefab(spawnPoint, out collectibles[i], collectiblePrefab);
        }
    }

    public void AddCollectible() {
        itemsCollected++;
        if (itemsCollected == totalItemQuantity) {
            hasEnded = true;
        }
    }

    private bool ValidCollectibleSpawn(int spawnPoint, float minDistance) {
        bool invalid = true;
        foreach (int usedPos in waypointsUsed) {
            float thisDistance = Vector3.Distance(GetSpawn(usedPos), GetSpawn(spawnPoint));
            if (thisDistance < minDistance || spawnPoint == usedPos) {
                invalid = false;
                break;
            }
        }
        return invalid;
    }

    private void InstantiatePrefab(int spawnPoint, out GameObject thisGameObject, GameObject prefab) {
        Vector3 position = GetSpawn(spawnPoint);
        thisGameObject = Instantiate(prefab, position, Quaternion.identity) as GameObject;
        thisGameObject.transform.position = position;
    }

    private Vector3 GetSpawn(int position) {
        Vector3 spawnPos = waypointList[position];
        //spawnPos.x += 0.5f;
        //spawnPos.y += 0.5f;
        return spawnPos;

    }

    private void GameOver() {
        loading = true;
        Vector3 pos = camera.transform.position;
        pos.z = 0;
        endGameInstance = Instantiate(endGame, pos, Quaternion.identity) as GameObject;
        endGameInstance.transform.parent = camera.transform;
        Invoke("LostAtLevel", restartLevelDelay);
    }

    private void Victory() {
        loading = true;
        Vector3 pos = camera.transform.position;
        pos.z = 0;
        camera.Follow = farPoint.transform;
        levelText.text = "Enemy Killed";
        level++;
        hasEnded = false;
        Invoke("StartLevel", restartLevelDelay);
    }


    private void LostAtLevel() {
        Destroy(endGameInstance);
        camera.Follow = farPoint.transform;
        levelText.text = "Collected " + itemsCollected + " out of " + totalItemQuantity;
        level = 1;
        Invoke("StartLevel", restartLevelDelay);
    }


    public Vector3 MapToRealCoords(int x, int y) {
        return new Vector3(x - 13.5f, -y + 11.5f, 0f);
    }

    public Vertex RealCoordsToMap(Vector2 pos) {
        int xPos = (pos.x) >= 0 ? ((int)(pos.x + 0f) + 14) : ((int)(pos.x - 0f) + 13);
        int yPos = (pos.y) >= 0 ? (-(int)(pos.y + 0f) + 11) : (-(int)(pos.y - 0f) + 12);
        return new Vertex() { x = xPos, y = yPos };
    }

}
