using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigator {
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
    //Falta ajustar
    public Vertex GetNextStep(Vertex from, Vertex to)
    {
        if (Graph.ContainsVertex(to))
        {
            return null;
        }
        else
            return null;
    }
}
