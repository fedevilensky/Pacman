using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager
{
    [HideInInspector] public bool[,] booleanMap;
    

    struct celda
    {
        int costo, anterior;
        bool conocido;
    }

    public void CreateMap(Tilemap wallMap)
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