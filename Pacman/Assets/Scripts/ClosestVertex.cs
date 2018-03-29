using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClosestVertex {

    public static bool[,] boolMap;

    public static void SetBoolMap(bool[,] map)
    {
        boolMap = map;
    }

    public static Vertex Find(Vertex pos)
    {
        if (Graph.ContainsVertex(pos))
        {
            return pos;
        }
        else
        {
            Vertex nextPos = pos;
            int maxCost = 100;
            int[] movX = new int[] { 1, -1, 0, 0 };
            int[] movY = new int[] { 0, 0, 1, -1 };
            for (int i = 0; i < 4; i++)
            {
                int xCord = pos.x + movX[i];
                int yCord = pos.y + movY[i];
                int thisCost = 1;
                if (GameManager.instance.tileManager.IsFloor(xCord, yCord))
                {
                    bool foundVer = false;
                    Vertex tryPos = new Vertex() { x = xCord, y = yCord };
                    while (!foundVer && GameManager.instance.tileManager.IsFloor(xCord, yCord) && maxCost > thisCost)
                    {
                        if (!Graph.ContainsVertex(tryPos))
                        {
                            tryPos.x += movX[i];
                            tryPos.y += movY[i];
                            thisCost++;
                        }
                        else
                        {
                            foundVer = true;
                        }
                    }
                    if (foundVer)
                    {
                        nextPos = tryPos;
                        maxCost = thisCost;
                    }
                }
            }
            return nextPos;
        }
    }


}
