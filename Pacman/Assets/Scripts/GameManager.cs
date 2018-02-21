using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour {

    public GameObject player;
    public static GameManager instance;
    public Tilemap wallMap;
    [HideInInspector] public TilemapManager tileManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            tileManager = new TilemapManager();
            tileManager.CreateMap(wallMap);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    

}
