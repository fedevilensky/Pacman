using System;
using System.Collections;
using System.Collections.Generic;

public class TestMap  {

    private Navigator navigator;
    public string pasos;

    public bool[,] booleanMap = new bool[,] {
        {false,false,false,false,false ,false,false,false,false},
        {false,true,true,true,false ,true,true,true,false},
        {false,true,false,true,false,true,false,true,false },
        {false,true,true,true,true ,true,true,true,false},
        {false,false,false,false,false ,false,false,false,false},
    };
    private Vertex myPos;
    private Vertex playerPos;
    public Vertex lastVer;

    public TestMap()
    {
        myPos = new Vertex() { x = 1, y = 1 };
        playerPos = new Vertex() { x = 1, y = 7 };
    }

    public void CreateGraph()
    {
        Graph.Reset(booleanMap.GetLength(0) * booleanMap.GetLength(1));

        int[] movX = new int[] { 1, -1, 0, 0 };
        int[] movY = new int[] { 0, 0, 1, -1 };

        Vertex actualVertex = new Vertex() { x = 1, y = 1 };

        Vertex lastVertex = actualVertex;
        Graph.AddVertex(actualVertex);
        bool[,] visited = new bool[booleanMap.GetLength(0), booleanMap.GetLength(1)];
        visited[actualVertex.x, actualVertex.y] = true;
        CreateMapGraphBT(visited, movX, movY, actualVertex.x, actualVertex.y, lastVertex, 1);

    }


    private void CreateMapGraphBT(bool[,] visited,
        int[] movX, int[] movY, int xPos, int yPos,
        Vertex lastVertex, int lastCost)
    {
        for (int i = 0; i < movX.Length; i++)
        {
            int newX = xPos + movX[i];
            int newY = yPos + movY[i];

            if (IsFloor(newX, newY))
            {
                Vertex actualVertex = new Vertex() { x = newX, y = newY };
                lastVer = actualVertex;
                if (!visited[newX, newY])
                {
                    bool dirChange = false;
                    for (int j = 0; j < movX.Length; j++)
                    {
                        int nextX = newX + movX[j];
                        int nextY = newY + movY[j];
                        if (IsFloor(nextX, nextY))
                            if (Math.Abs(nextX - xPos) > 0 && Math.Abs(nextY - yPos) > 0)
                                dirChange = true;
                    }
                    visited[newX, newY] = true;
                    if (dirChange)
                    {
                        Graph.AddVertex(actualVertex);
                        Graph.AddArch(actualVertex, lastVertex, lastCost);
                        CreateMapGraphBT(visited, movX, movY, actualVertex.x, actualVertex.y, actualVertex, 1);
                    }
                    else
                    {
                        CreateMapGraphBT(visited, movX, movY, actualVertex.x, actualVertex.y, lastVertex, lastCost + 1);
                    }
                }
                else if (Graph.ContainsVertex(actualVertex)&&(actualVertex.x!=lastVertex.x|| actualVertex.y != lastVertex.y))
                {
                    Graph.AddArch(actualVertex, lastVertex, lastCost);
                }
            }
        }
    }



    public bool IsFloor(int x, int y)
    {
        return x > 0 && y > 0 && x < booleanMap.GetLength(0) - 1 && y < booleanMap.GetLength(1) - 1 && booleanMap[x, y];
    }

    public String TestWalk()
    {
        string ret = "";
        navigator = new Navigator();
        myPos = navigator.GetNextStep(myPos, playerPos);
        ret += myPos.ToString();
        ret+="\n";
        return ret;
    }


    public String Printmap()
    {
        String impresion = "";
        for (int i = 0; i < booleanMap.GetLength(0); i++)
        {
            for (int j = 0; j < booleanMap.GetLength(1); j++)
            {
                if (booleanMap[i, j] == true)
                    impresion += "1";
                else
                    impresion += "0";
            }
            impresion += "\n";
        }
        return impresion;

    }
}
