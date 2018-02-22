using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cinemachine;
using System;

public class GameManager : MonoBehaviour
{

    [HideInInspector] public GameObject player;
    public GameObject playerPrefab;
    [HideInInspector] public GameObject enemy;
    public GameObject enemyPrefab;
    public static GameManager instance;
    public Tilemap wallMap;
    public Tilemap waypoints;
    [HideInInspector] public TilemapManager tileManager;
    public CinemachineVirtualCamera camera;
    public String positions = "";


    private List<Vertex> waypointList;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            tileManager = new TilemapManager();
            waypointList = new List<Vertex>();
            CreateWaypointList();
            printSpawns();
            RandomSpawns();
            AssignCamera();
            tileManager.CreateMap(wallMap);

        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
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
                        x = i- tileManager.booleanMap.GetLength(0)/2,
                        y = j- tileManager.booleanMap.GetLength(1)/2
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

    private void InstantiatePrefab(int playerSpawn, out GameObject gameObject, GameObject prefab)
    {
        Vector3 position = VertexToMapVector(playerSpawn);
        gameObject = Instantiate(prefab, position, Quaternion.identity) as GameObject;
        gameObject.transform.position = position;
    }

    private Vector3 VertexToMapVector(int position)
    {
        Vertex v = waypointList[position];
        Vector3 spawnPos = new Vector3(v.x, v.y, 0);
        spawnPos.x += 0.5f;
        spawnPos.y += 2.5f;
        return spawnPos;

    }

    private void printSpawns()
    {
        for (int i = 0; i < waypointList.Count; i++)
        {
            Vertex v = waypointList[i];
            Vector3Int vec3 = new Vector3Int(v.x, v.y, 0);
            Vector3 spawnPos = waypoints.CellToWorld(vec3);
            positions += "x = " + (int) v.x + " - y = " + (int)v.y+"\n";
        }
    }

}
