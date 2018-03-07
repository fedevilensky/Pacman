using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Direction
{
    NO,
    UP,
    DOWN,
    LEFT,
    RIGHT
}

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
        booleanMap = new bool[bounds.size.x, bounds.size.y];

        TileBase[] allTiles = wallMap.GetTilesBlock(bounds);
        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile == null)
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



    public Graph CreateMapGraph()
    {
        Graph ret = new Graph(booleanMap.Length);

        int[] movX = new int[] { 1, -1, 0, 0 };
        int[] movY = new int[] { 0, 0, 1, -1 };

        Direction lastDirection = Direction.NO;
        //Va este
        //GraphVertex actualVertex = new GraphVertex() {vertex = new Vertex() {x = 6, y = 9 }, Cost =1 };


        //hay que borrarlo
        GraphVertex actualVertex = new GraphVertex() { vertex = new Vertex() { x = 5, y = 8 }, Cost = 1 };

        ////////////
        GraphVertex lastVertex = actualVertex;
        ret.AddVertex(actualVertex);
        bool[,] visited = new bool[booleanMap.GetLength(0), booleanMap.GetLength(1)];
        CreateMapGraphBT(booleanMap, visited, movX, movY, actualVertex.vertex.x, actualVertex.vertex.y,
            lastDirection, actualVertex, lastVertex, ret);

        return ret;
    }

    private void CreateMapGraphBT(bool[,] map, bool[,] visited,
        int[] movX, int[] movY, int xPos, int yPos,
        Direction lastDirection, GraphVertex actualVertex,
        GraphVertex lastVertex, Graph solution)
    {

        for (int i = 0; i < movX.Length; i++)
        {
            int newX = xPos + movX[i];
            int newY = yPos + movY[i];

            if (IsFloor(newX, newY, map))
            {
                map[xPos, yPos] = false;

                Direction newDirection = CalculateDirection(xPos, yPos, newX, newY);
                int count = 0;
                for (int j = 0; j < movX.Length; j++)
                {
                    if (IsFloor(xPos + movX[j], yPos + movY[j], map))
                    {
                        count++;
                    }
                }
                if (count != 1 || newDirection != lastDirection)
                {
                    solution.AddVertex(actualVertex);//if vertex already exists does nothing
                    solution.AddArc(actualVertex, lastVertex);
                    GameManager.instance.error += "x: " + xPos + " - y: " + yPos + " - count: " + count + " - direccion anterior: " + lastDirection + " - nueva direccion: " + newDirection + "\n";
                    if (!visited[newX, newY])
                    {
                        Vertex newVertex = new Vertex() { x = newX, y = newY };
                        GraphVertex newGVertex = new GraphVertex() { vertex = newVertex, Cost = 1 };

                        CreateMapGraphBT(map, visited, movX, movY, newX, newY, newDirection, newGVertex, actualVertex, solution);
                    }
                }
                else
                {
                    if (!visited[newX, newY])
                    {
                        actualVertex.IncrementCost();
                        actualVertex.vertex = new Vertex() { x = newX, y = newY };
                        CreateMapGraphBT(map, visited, movX, movY, newX, newY, newDirection, actualVertex, lastVertex, solution);

                        actualVertex.DecrementCost();
                        actualVertex.vertex = new Vertex() { x = xPos, y = yPos };
                    }
                }
                map[xPos, yPos] = true;
                visited[newX, newY] = true;
            }

        }
    }

    private Direction CalculateDirection(int xPos, int yPos, int newX, int newY)
    {
        int x = xPos - newX;
        int y = yPos - newY;
        Direction ret = Direction.NO;
        if (x < 0 && y == 0)
        {
            ret = Direction.LEFT;
        }
        else if (x > 0 && y == 0)
        {
            ret = Direction.RIGHT;
        }
        else if (x == 0 && y < 0)
        {
            ret = Direction.UP;
        }
        else if (x == 0 && y > 0)
        {
            ret = Direction.DOWN;
        }

        return ret;

    }

    private bool IsFloor(int x, int y, bool[,] map)
    {
        return x >= 0 && y >= 0 && x < map.GetLength(0) && y < map.GetLength(1) && map[x, y];
    }
}