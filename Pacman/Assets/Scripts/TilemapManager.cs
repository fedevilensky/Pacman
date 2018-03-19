using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using UnityEditor;

/* ORDEN DE ROOMS EN MAPA
 * 1 2
 * 3 4
 * GetLength(0) es y
 * GetLength(0) es x
 */

public enum Direction
{
    NO,
    UP,
    DOWN,
    LEFT,
    RIGHT
}

public enum Orientation
{
    VERTICAL,
    HORIZONTAL
}


public class TilemapManager : MonoBehaviour
{
    [HideInInspector] public bool[,] booleanMap;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public int maxHCons = 2;
    public int maxVCons = 1;
    public String impresion = "";
    public int random;

    private RoomCollection roomCollection;
    private TileInfo[,] roomMap;

    struct celda
    {
        int costo, anterior;
        bool conocido;
    }

    public void DrawMap()
    {
        roomCollection = new RoomCollection();
        List<TileInfo[,]> roomList = roomCollection.GetList();

        TileInfo[,] selectedRoom1 = (TileInfo[,])roomList[Random.Range(0, roomList.Count)].Clone();
        TileInfo[,] selectedRoom2 = (TileInfo[,])roomList[Random.Range(0, roomList.Count)].Clone();
        TileInfo[,] selectedRoom3 = (TileInfo[,])roomList[Random.Range(0, roomList.Count)].Clone();
        TileInfo[,] selectedRoom4 = (TileInfo[,])roomList[Random.Range(0, roomList.Count)].Clone();

        MakeConnections(Orientation.VERTICAL, selectedRoom1, selectedRoom2);
        MakeConnections(Orientation.VERTICAL, selectedRoom3, selectedRoom4);
        MakeConnections(Orientation.HORIZONTAL, selectedRoom1, selectedRoom3);/*
       MakeConnections(Orientation.HORIZONTAL, selectedRoom2, selectedRoom4);
       */
        roomMap = new TileInfo[19, 29];
        for (int y = 0; y < roomMap.GetLength(0) / 2 + 1; y++)
        {
            for (int x = 0; x < roomMap.GetLength(1) / 2 + 1; x++)
            {
                roomMap[y, x] = selectedRoom1[y, x];

                roomMap[y, x + roomMap.GetLength(1) / 2] = selectedRoom2[y, x];

                roomMap[y + roomMap.GetLength(0) / 2, x] = selectedRoom3[y, x];

                roomMap[y + roomMap.GetLength(0) / 2, x + roomMap.GetLength(1) / 2] = selectedRoom4[y, x];

            }
        }
        impresion += "x: " + roomMap.GetLength(1);
        impresion += "\n";
        impresion += "y: " + roomMap.GetLength(0);
        impresion += "\n";
        Printmap(roomMap);
        InstantiateMap();
    }

    private void Printmap(TileInfo[,] map)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] == TileInfo.FLOOR || map[i, j] == TileInfo.WAYPOINT)
                    impresion += "1";
                else
                    impresion += "0";
            }
            impresion += "\n";
        }
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

    private void InstantiateMap()
    {
        booleanMap = new bool[roomMap.GetLength(1), roomMap.GetLength(0)];
        for (int x = 0; x < roomMap.GetLength(1); x++)
        {
            for (int y = 0; y < roomMap.GetLength(0); y++)
            {
                booleanMap[x, y] = true;
                int xCoord = x;
                int yCoord = y;
                if (roomMap[y, x] == TileInfo.WALL || roomMap[y, x] == TileInfo.CONNECTION || roomMap[y, x] == TileInfo.ERROR)
                {
                    GameObject instance = Instantiate(wallTiles[Random.Range(0, wallTiles.Length)], new Vector3(xCoord - 13.5f, -yCoord + 11.5f, 0f), Quaternion.identity);
                    instance.transform.SetParent(GameManager.instance.wallMap.transform);
                    booleanMap[x, y] = false;
                }
                else if (roomMap[y, x] == TileInfo.WAYPOINT)
                {
                    Vertex aux = new Vertex
                    {
                        x = xCoord,
                        y = yCoord
                    };
                    GameManager.instance.waypointList.Add(aux);
                }
            }
        }
    }

    private void MakeConnections(Orientation orientation, TileInfo[,] room1, TileInfo[,] room2)
    {
        int cons = 0;
        List<int> connections = new List<int>();
        if (orientation == Orientation.HORIZONTAL)
        {
            for (int i = 0; i < room1.GetLength(1); i++)
            {
                if (room1[room1.GetLength(0) - 1, i] == TileInfo.CONNECTION && room2[0, i] == TileInfo.CONNECTION)
                {
                    connections.Add(i);
                }
            }
            while (cons < maxHCons)
            {
                int position = (int)Random.Range(0, connections.Count);
                room1[room1.GetLength(0) - 1, connections[position]] = TileInfo.FLOOR;
                room2[0, connections[position]] = TileInfo.FLOOR;
                cons++;
                connections.RemoveAt(position);
            }
        }
        else if (orientation == Orientation.VERTICAL)
        {
            for (int i = 0; i < room1.GetLength(0); i++)
            {
                if (room1[i, room1.GetLength(1) - 1] == TileInfo.CONNECTION && room2[i, 0] == TileInfo.CONNECTION)
                {
                    connections.Add(i);
                }
            }
            while (cons < maxVCons)
            {
                int position = (int)Random.Range(0, connections.Count);
                room1[connections[position], room1.GetLength(1) - 1] = TileInfo.FLOOR;
                room2[connections[position], 0] = TileInfo.FLOOR;
                cons++;
                connections.RemoveAt(position);
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