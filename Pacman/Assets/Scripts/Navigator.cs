using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigator
{
    private static int[,] pathMatrix;
    public string print;


    public Navigator()
    {
        int[,] aux = Graph.GetAdjacencyMatrix();
        if (pathMatrix == null || pathMatrix.GetLength(0)<aux.GetLength(0))
        {
            pathMatrix = new int[aux.GetLength(0), aux.GetLength(1)];
            for (int i = 0; i < aux.GetLength(0); i++)
            {
                for (int j = 0; j < aux.GetLength(1); j++)
                {
                    pathMatrix[i, j] = Graph.INF;
                    if (aux[i, j] != Graph.INF || i==j)
                    {
                        pathMatrix[i, j] = j;
                    }
                }
            }
        }
    }
    

    public Vertex GetNextStep(Vertex from, Vertex to)
    {
        Vertex destination = ClosestVertex.Find(to);
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
            return ClosestVertex.Find(from);
    }

    //Aca va el Dikjstra
    private void CalculatePath(Vertex from, Vertex to)
    {
        int[,] adjacencyMatrix = Graph.GetAdjacencyMatrix();
        VertexEqualityComparer comp = new VertexEqualityComparer();
        PriorityQueue<Vertex> queue = new ImplementedPriorityQueue<Vertex>(new VertexEqualityComparerGenericObject());
        int[] previousStep = new int[Graph.GetAdjacencyMatrix().GetLength(0)];
        int[] dist = new int[Graph.GetAdjacencyMatrix().GetLength(0)];
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
                if (dist[posV] == Graph.INF || dist[posV] > dist[posActualVertex] + adjacencyMatrix[posActualVertex, posV])
                {
                    dist[posV] = dist[posActualVertex] + adjacencyMatrix[posActualVertex, posV];
                    if (!queue.Pertenece(v))
                        queue.InsertarConPrioridad(v, -dist[posV]);
                    else
                        queue.CambiarPrioridad(v, -dist[posV]);
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
        while (previousStep[current] != Graph.INF&&fromPos!=current)
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


    private bool IsFloor(int x, int y, bool[,] booleanMap)
    {
        return x > 0 && y > 0 && x < booleanMap.GetLength(0) - 1 && y < booleanMap.GetLength(1) - 1 && booleanMap[x, y];
    }
}
