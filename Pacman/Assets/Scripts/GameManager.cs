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

    public String error = "";


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
    /*
    private void OnLevelWasLoaded(int index)
    {
        level++;
        InitGame();
    }*/

    private void StartLevel()
    {
        loading = true;
        levelText.text = "Level " + level;
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
        tileManager.DrawMap();
        RandomSpawns();
        AssignCamera();


        
        lastGunSpawn = 1f;
        GameObject[] enemies = new GameObject[1];
        enemies[0] = enemy;
        SoundManager.instance.enemies = enemies;
        SoundManager.instance.LoadEnemySound();
        gunAppearedText.SetActive(false);
        loading = false;
    }

    private void ResetBools()
    {
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
    }

    private void Update()
    {
        if (!loading)
        {
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
    }

    public void DestroyGun()
    {
        Destroy(gun);
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
        HashSet<int> waypointsUsed = new HashSet<int>();
        int spawnPoint = UnityEngine.Random.Range(0, waypointList.Count);
        waypointsUsed.Add(spawnPoint);
        InstantiatePrefab(spawnPoint, out player, playerPrefab);
        //aca instanciamos a el/los enemigos
        do
        {
            spawnPoint = UnityEngine.Random.Range(0, waypointList.Count);
        } while (waypointsUsed.Contains(spawnPoint));
        waypointsUsed.Add(spawnPoint);
        InstantiatePrefab(spawnPoint, out enemy, enemyPrefab);
        //aca poner el spawn del objeto
    }

    private void InstantiatePrefab(int spawnPoint, out GameObject thisGameObject, GameObject prefab)
    {
        Vector3 position = VertexToMapVector(spawnPoint);
        thisGameObject = Instantiate(prefab, position, Quaternion.identity) as GameObject;
        thisGameObject.transform.position = position;
    }

    private Vector3 VertexToMapVector(int position)
    {
        Vector3 spawnPos = waypointList[position];
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
        levelText.text = "Lost at level " + level;
        level = 1;
        Invoke("StartLevel", restartLevelDelay);
    }


    public Vector3 MapToRealCoords(int x, int y)
    {
        return new Vector3(x - 13.5f, -y + 11.5f, 0f);
    }

    public Vertex RealCoordsToMap(Vector2 pos)
    {
        return new Vertex(){x=(int)(pos.x - 13.5),y=(int)(pos.y - 11.5f)};
    }

}
