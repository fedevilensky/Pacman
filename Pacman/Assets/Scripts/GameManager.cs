using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Cinemachine;
using System;

public class GameManager : MonoBehaviour
{

    public GameObject borrarPrefab;

    [HideInInspector] public GameObject player;
    public GameObject playerPrefab;
    [HideInInspector] public GameObject enemy;
    [HideInInspector] public GameObject[] collectibles;
    public GameObject collectiblePrefab;
    public GameObject gunPrefab;
    public GameObject enemyPrefab;
    public static GameManager instance;
    public Tilemap wallMap;
    public Tilemap waypoints;
    [HideInInspector] public TilemapManager tileManager;
    public CinemachineVirtualCamera camera;
    public GameObject endGame;
    [HideInInspector] public bool gameOver;
    [HideInInspector]public bool hasEnded;
    [HideInInspector]public bool gunIsSpawned;
    [HideInInspector]public bool playerHasGun;
    public GameObject gunAppearedText;
    public Text levelText;
    public float gunSpawnRate = 15;
    public float gunTextTimerLimit = 2f;
    public float restartLevelDelay = 2f;
    public GameObject farPoint;
    [HideInInspector]public bool loading;
    public float marginError = 0.1f;
    public float gameSpeed = 2f;
    [HideInInspector] public int itemsCollected;
    [HideInInspector] public bool enemyDead;
    [HideInInspector] public float respawnTime;

    public String myError = "";

    private HashSet<int> waypointsUsed;
    private int level = 1;
    public List<Vector3> waypointList;
    private float lastGunSpawn;
    private GameObject gun;
    private float gunTextTimer;
    private GameObject endGameInstance;
    public string boolMapPrint;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        StartLevel();
    }

    private void StartLevel()
    {
        loading = true;
        levelText.text = "Collect the 6 items";
        camera.Follow = farPoint.transform;
        Invoke("InitGame", restartLevelDelay);
    }

    private void InitGame()
    {
        levelText.text = "";
        DestroyInstances();
        ResetBools();
        waypointList = new List<Vector3>();
        tileManager = gameObject.GetComponent<TilemapManager>();
        collectibles = new GameObject[6];
        tileManager.DrawMap();

        waypointsUsed = new HashSet<int>();
        RandomSpawns();
        AssignCamera();


        
        lastGunSpawn = 1f;
        respawnTime = 20f;
        GameObject[] enemies = new GameObject[1];
        enemies[0] = enemy;
        enemy.GetComponent<Enemy>().strategy = new StrategyWanderTest();
        SoundManager.instance.enemies = enemies;
        gunAppearedText.SetActive(false);
        itemsCollected = 0;
        loading = false;
    }

    private void ResetBools()
    {
        enemyDead = false;
        gameOver = false;
        hasEnded = false;
        gunIsSpawned = false;
        playerHasGun = false;
    }

    private void DestroyInstances()
    {
        Destroy(gun);
        Destroy(enemy);
        Destroy(player);
        foreach(GameObject c in collectibles)
        {
            Destroy(c);
        }
    }

    private void Update()
    {
        if (!loading)
        {
            if (enemyDead)
            {
                respawnTime -= Time.deltaTime;
            }
            if (respawnTime <= 0f && enemyDead)
            {
                Destroy(enemy);
                 int   spawnPoint = UnityEngine.Random.Range(0, waypointList.Count);
                InstantiatePrefab(spawnPoint, out enemy, enemyPrefab);
                enemyDead = false;
            }
            if (gameOver && !hasEnded)
            {
                hasEnded = true;
                GameOver();
            }
            else if (!gameOver && hasEnded)
            {
                Victory();
            }
        }
    }

    public void EquipGun()
    {
        playerHasGun = true;
        enemy.GetComponent<Enemy>().strategy = new StrategyEscapeTest();
    }

    public void DestroyGun()
    {
        Destroy(gun);
        enemy.GetComponent<Enemy>().strategy = new StrategyWanderTest();
    }

    void FixedUpdate()
    {
        if (loading)
            return;
        if (!gunIsSpawned)
        {
            gunTextTimer = gunTextTimerLimit;
            lastGunSpawn -= Time.deltaTime;
            if (lastGunSpawn < 0)
            {
                gunIsSpawned = true;

                lastGunSpawn = gunSpawnRate;
                SpawnGun();
            }
        }
        else
        {
            if (gunTextTimer > 0)
            {
                gunAppearedText.SetActive(true);
                gunTextTimer -= Time.deltaTime;
            }
            else
            {
                gunAppearedText.SetActive(false);
            }
        }
    }

    private void SpawnGun()
    {

        int spawnPoint = UnityEngine.Random.Range(0, waypointList.Count);
        InstantiatePrefab(spawnPoint, out gun, gunPrefab);
    }

    private void AssignCamera()
    {
        camera.Follow = player.transform;
    }

    private void RandomSpawns()
    {
        //aca instanciamos al jugador
        int spawnPoint = UnityEngine.Random.Range(0, waypointList.Count);
        waypointsUsed.Add(spawnPoint);
        InstantiatePrefab(spawnPoint, out player, playerPrefab);
        //aca instanciamos a el/los enemigos
        do
        {
            spawnPoint = UnityEngine.Random.Range(0, waypointList.Count);
        } while (ValidSpawn(spawnPoint));
        waypointsUsed.Add(spawnPoint);
        InstantiatePrefab(spawnPoint, out enemy, enemyPrefab);

        //aca poner el spawn de los objetos
        for (int i = 0; i < 6; i++)
        {
            int n = 0;
            do
            {
                spawnPoint = UnityEngine.Random.Range(0, waypointList.Count);
                n++;
            } while (ValidSpawn(spawnPoint)&&n<60);
            waypointsUsed.Add(spawnPoint);
            InstantiatePrefab(spawnPoint, out collectibles[i], collectiblePrefab);
        }
    }

    public void AddCollectible()
    {
        itemsCollected++;
        if (itemsCollected == 6)
        {
            hasEnded = true;
        }
    }

    private bool ValidSpawn(int spawnPoint)
    {
        float minDist = 7f;
        bool invalid = false;
        foreach(int usedPos in waypointsUsed)
        {
            float thisDistance = Vector3.Distance(GetSpawn(usedPos), GetSpawn(spawnPoint));
            if (minDist > thisDistance || waypointsUsed.Contains(spawnPoint))
            {
                invalid = true;
                break;
            }
        }
        return invalid;
    }

    private void InstantiatePrefab(int spawnPoint, out GameObject thisGameObject, GameObject prefab)
    {
        Vector3 position = GetSpawn(spawnPoint);
        thisGameObject = Instantiate(prefab, position, Quaternion.identity) as GameObject;
        thisGameObject.transform.position = position;
    }

    private Vector3 GetSpawn(int position)
    {
        Vector3 spawnPos = waypointList[position];
        //spawnPos.x += 0.5f;
        //spawnPos.y += 0.5f;
        return spawnPos;

    }

    private void GameOver()
    {
        loading = true;
        Vector3 pos = camera.transform.position;
        pos.z = 0;
        endGameInstance = Instantiate(endGame, pos, Quaternion.identity) as GameObject;
        endGameInstance.transform.parent = camera.transform;
        Invoke("LostAtLevel", restartLevelDelay);
    }

    private void Victory()
    {
        loading = true;
        Vector3 pos = camera.transform.position;
        pos.z = 0;
        camera.Follow = farPoint.transform;
        levelText.text = "Enemy Killed";
        level++;
        hasEnded = false;
        Invoke("StartLevel", restartLevelDelay);
    }


    private void LostAtLevel()
    {
        Destroy(endGameInstance);
        camera.Follow = farPoint.transform;
        levelText.text = "Collected " + itemsCollected+" out of 6";
        level = 1;
        Invoke("StartLevel", restartLevelDelay);
    }


    public Vector3 MapToRealCoords(int x, int y)
    {
        return new Vector3(x - 13.5f, -y + 11.5f, 0f);
    }

    public Vertex RealCoordsToMap(Vector2 pos)
    {
        int xPos = (pos.x) >= 0 ? ((int)(pos.x+0f)+14) : ((int)(pos.x - 0f) + 13);
        int yPos = (pos.y) >= 0 ? (-(int)(pos.y+0f)+11) : (-(int)(pos.y-0f) + 12);
        return new Vertex() { x = xPos, y = yPos };
    }

}
