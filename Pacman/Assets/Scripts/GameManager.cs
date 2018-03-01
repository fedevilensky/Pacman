using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Cinemachine;
using System;

public class GameManager : MonoBehaviour
{

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
    [HideInInspector] public bool hasEnded;
    public bool gunIsSpawned;
    [HideInInspector] public bool playerHasGun;
    public GameObject gunAppearedText;
    public float gunSpawnRate = 15;
    public float gunTextTimerLimit = 2f;
    public float restartLevelDelay = 2f;


    private int level = 1;
    public List<Vertex> waypointList;
    private float lastGunSpawn;
    private GameObject gun;
    private float gunTextTimer;
    private GameObject endGameInstance;


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

        //DontDestroyOnLoad(gameObject);
        InitGame();
    }
    /*
    private void OnLevelWasLoaded(int index)
    {
        level++;
        InitGame();
    }*/

    private void InitGame()
    {
        DestroyInstances();
        ResetBools();
        tileManager = new TilemapManager();
        waypointList = new List<Vertex>();
        CreateWaypointList();
        RandomSpawns();
        AssignCamera();
        tileManager.CreateMap(wallMap);
        Destroy(endGameInstance);
        lastGunSpawn = gunSpawnRate;
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
        if (gameOver && !hasEnded)
        {
            hasEnded = true;
            GameOver();
        }
    }
    
    public void DestroyGun()
    {
        Destroy(gun);
    }

    void FixedUpdate()
    {
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

    private void CreateWaypointList()
    {
        tileManager.CreateMap(waypoints);
        for (int i = 0; i < tileManager.booleanMap.GetLength(0); i++)
        {
            for (int j = 0; j < tileManager.booleanMap.GetLength(1); j++)
            {
                if (!tileManager.booleanMap[i, j])
                {
                    Vertex aux = new Vertex
                    {
                        x = i - tileManager.booleanMap.GetLength(0) / 2,
                        y = j - tileManager.booleanMap.GetLength(1) / 2
                    };
                    waypointList.Add(aux);
                }
            }
        }
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
        Vertex v = waypointList[position];
        Vector3 spawnPos = new Vector3(v.x, v.y, 0);
        spawnPos.x += 0.5f;
        spawnPos.y += 2.5f;
        return spawnPos;

    }
    
    
    private void GameOver()
    {
        Vector3 pos = camera.transform.position;
        pos.z = 0;
        endGameInstance = Instantiate(endGame, pos, Quaternion.identity) as GameObject;
        endGameInstance.transform.parent = camera.transform;
        Invoke("InitGame", restartLevelDelay);
    }
    

}
