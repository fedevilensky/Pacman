using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    public Tilemap wallMap;
    [HideInInspector] public bool[,] booleanMap;
    public static TilemapManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
        CreateMap();
    }

    struct celda
    {
        int costo, anterior;
        bool conocido;
    }

    void CreateMap()
    {
        BoundsInt bounds = wallMap.cellBounds;
        booleanMap = new bool[bounds.size.x,bounds.size.y];

        TileBase[] allTiles = wallMap.GetTilesBlock(bounds);

        for (int x = 0; x<bounds.size.x; x++) {
            for (int y = 0; y<bounds.size.y; y++) {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if(tile == null)
                {
                    booleanMap[x, y] = true;
                }
                else
                {
                    booleanMap[x, y] = false;
                }
            }
        }
    }
}