using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cinemachine;

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


    private List<Vertex> waypointList;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            tileManager = new TilemapManager();
            waypointList = new List<Vertex>();
            CreateWaypointList();
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
                if (tileManager.booleanMap[i, j])
                {
                    Vertex aux = new Vertex
                    {
                        x = i,
                        y = j
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
        int spawnPoint = Random.Range(0, waypointList.Count);
        waypointsUsed.Add(spawnPoint);
        InstantiatePrefab(spawnPoint, out player, playerPrefab);
        //aca instanciamos a el/los enemigos
        do
        {
            spawnPoint = Random.Range(0, waypointList.Count);
        } while (waypointsUsed.Contains(spawnPoint));
        waypointsUsed.Add(spawnPoint);
        InstantiatePrefab(spawnPoint, out enemy, enemyPrefab);
        //aca poner el spawn del objeto
    }

    private void InstantiatePrefab(int playerSpawn, out GameObject gameObject, GameObject prefab)
    {
        Vector3 position = VertexToMap(playerSpawn);
        gameObject = Instantiate(prefab, position, Quaternion.identity) as GameObject;
        gameObject.transform.position = position;
    }

    private Vector3 VertexToMap(int position)
    {
        Vertex v = waypointList[position];
        Vector3Int vec3 = new Vector3Int(v.x, v.y, 0);
        Vector3 spawnPos = waypoints.CellToWorld(vec3);
        spawnPos.x -= 1.5f;
        spawnPos.y += 0.5f;
        return spawnPos;

    }


}
