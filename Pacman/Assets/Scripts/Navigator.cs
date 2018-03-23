using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigator
{
    private static int[,] pathMatrix;


    public Navigator()
    {
        int[,] aux = Graph.GetAdjacencyMatrix();
        pathMatrix = new int[aux.GetLength(0), aux.GetLength(1)];
        for (int i = 0; i < aux.GetLength(0); i++)
        {
            for (int j = 0; j < aux.GetLength(1); j++)
            {
                pathMatrix[i, j] = Graph.INF;
                if (aux[i, j] != Graph.INF)
                {
                    pathMatrix[i, j] = j;
                }
            }
        }
    }

    public Vertex GetNextStep(Vertex from, Vertex to)
    {
        Vertex destination = GetClosestVertex(to);
        if (Graph.ContainsVertex(from))
        {
            int verNumber = pathMatrix[Graph.GetVertexPos(from), Graph.GetVertexPos(destination)];
            if (verNumber == Graph.INF)
            {
                CalculatePath(from, destination);
                verNumber = pathMatrix[Graph.GetVertexPos(from), Graph.GetVertexPos(destination)];
            }
            return Graph.GetVertexPos(verNumber);
        }
        else
            return GetClosestVertex(from);
    }

    private Vertex GetClosestVertex(Vertex pos)
    {
        if (Graph.ContainsVertex(pos))
        {
            return pos;
        }
        else
        {
            Vertex nextPos = pos;
            int maxCost = 100;
            bool[,] boolMap = GameManager.instance.tileManager.booleanMap;
            int[] movX = new int[] { 1, -1, 0, 0 };
            int[] movY = new int[] { 0, 0, 1, -1 };
            for (int i = 0; i < 4; i++)
            {
                int xCord = pos.x + movX[i];
                int yCord = pos.y + movY[i];
                int thisCost = 1;
                if (boolMap[xCord, yCord])
                {
                    Vertex tryPos = new Vertex() { x = xCord, y = yCord };
                    while (!Graph.ContainsVertex(tryPos) && maxCost > thisCost)
                    {
                        xCord = pos.x + movX[i];
                        yCord = pos.y + movY[i];
                        tryPos = new Vertex() { x = xCord, y = yCord };
                        thisCost++;
                    }
                    if (maxCost > thisCost && boolMap[xCord, yCord])
                    {
                        nextPos = tryPos;
                        maxCost = thisCost;
                    }
                }
            }
            return nextPos;
        }
    }

    //Aca va el Dikjstra
    private void CalculatePath(Vertex from, Vertex to)
    {
        int[,] adjacencyMatrix = Graph.GetAdjacencyMatrix();
        VertexEqualityComparer comp = new VertexEqualityComparer();
        PriorityQueue<Vertex> queue = new ImplementedPriorityQueue<Vertex>(new VertexEqualityComparerGenericObject());
        int[] previousStep = new int[Graph.CountVertexes()];
        int[] dist = new int[Graph.CountVertexes()];
        for (int i = 0; i < dist.Length; i++)
        {
            dist[i] = Graph.INF;
            previousStep[i] = Graph.INF;
        }
        queue.InsertarConPrioridad(from, 0);

        while (!queue.EstaVacia())
        {
            Vertex actualVertex = queue.EliminarElementoMayorPrioridad();
            if (comp.Equals(actualVertex, to))
            {
                break;
            }
            foreach (Vertex v in Graph.GetAdjacents(actualVertex))
            {
                int posV = Graph.GetVertexPos(v);
                int posActualVertex = Graph.GetVertexPos(actualVertex);
                if (dist[posV] == Graph.INF
                    || dist[posV] > dist[posActualVertex] + adjacencyMatrix[posActualVertex, posV])
                {
                    dist[posV] = dist[posActualVertex] + adjacencyMatrix[posActualVertex, posV];
                    queue.InsertarConPrioridad(v, -dist[posV]);
                    previousStep[posV] = posActualVertex;
                }
            }
        }

        PrintInMatrix(previousStep, from, to);
    }

    private void PrintInMatrix(int[] previousStep, Vertex from, Vertex to)
    {
        int fromPos = Graph.GetVertexPos(from);
        int toPos = Graph.GetVertexPos(to);
        int current = toPos;
        List<int> movesList = new List<int>();
        while (previousStep[current] != Graph.INF)
        {
            pathMatrix[current, fromPos] = previousStep[current];
            movesList.Add(current);
            current = previousStep[current];
        }
        foreach(int i in movesList)
        {
            pathMatrix[pathMatrix[i, fromPos], toPos] = i;
        }
    }
}
