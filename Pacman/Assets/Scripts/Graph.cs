using System.Collections;
using System.Collections.Generic;
public static class Graph
{
    //public int CountVertex { get; private set; }
    public static int CountArchs { get; private set; }
    private static int[,] adjacencyMatrix;
    private static Hashtable internalRepresentation = new Hashtable(new VertexEqualityComparerGenericObject());
    private static Hashtable inverseRepresentation = new Hashtable();
    public const int INF = -1;

    public static void Reset(int size)
    {
        adjacencyMatrix = new int[size, size];
        for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < adjacencyMatrix.GetLength(1); j++)
            {
                adjacencyMatrix[i, j] = INF;
            }
        }
        CountArchs = 0;

    }
    
    public static void AddVertex(Vertex v)
    {
        if (!IsFull() && !internalRepresentation.Contains(v))
        {
            int i = CountVertexes();
            while (inverseRepresentation.Contains(i))
            {
                i = (i++) %adjacencyMatrix.GetLength(0);
            }
            internalRepresentation.Add(v, i);
            inverseRepresentation.Add(i, v);

        }
    }

    public static void AddArch(Vertex to, Vertex from, int cost)
    {
        if (adjacencyMatrix[(int)internalRepresentation[to], (int)internalRepresentation[from]] == INF && adjacencyMatrix[(int)internalRepresentation[from], (int)internalRepresentation[to]] == INF)
        {
            CountArchs++;
            adjacencyMatrix[(int)internalRepresentation[to], (int)internalRepresentation[from]] = cost;
            adjacencyMatrix[(int)internalRepresentation[from], (int)internalRepresentation[to]] = cost;
        }
    }
    
    public static void RemoveVertex(Vertex v)
    {
        if (internalRepresentation.Contains(v))
        {
            int aux = (int)internalRepresentation[v];
            for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
            {
                Vertex toCompare = (Vertex)inverseRepresentation[i];
                RemoveArch(v, toCompare);

            }
            internalRepresentation.Remove(v);
            inverseRepresentation.Remove(aux);

        }
    }
    

    public static void RemoveArch(Vertex v1, Vertex v2)
    {
        if (v1.CompareTo(v2)==1 && internalRepresentation.Contains(v1) && internalRepresentation.Contains(v2))
        {
            int a = (int)internalRepresentation[v1];
            int b = (int)internalRepresentation[v2];
            if (adjacencyMatrix[a, b] != INF && adjacencyMatrix[b,a]!=INF)
            {
                CountArchs--;
                adjacencyMatrix[a, b] = INF;
                adjacencyMatrix[b, a] = INF;
            }
        }
    }

    public static IEnumerable<Vertex> GetEveryVertex()
    {
        List<Vertex> aux = new List<Vertex>();
        for (int i = 0; i < CountVertexes(); i++)
        {
            if (inverseRepresentation.Contains(i))
            {
                aux.Add((Vertex)inverseRepresentation[i]);
            }
        }

        return aux;
    }

    public static int GetVertexPos(Vertex v)
    {
        return (int)internalRepresentation[v];
    }
    public static Vertex GetVertexPos(int i)
    {
        return (Vertex)inverseRepresentation[i];
    }

    public static IEnumerable<Vertex> GetAdjacents(Vertex v)
    {
        List<Vertex> aux = new List<Vertex>();
        int pos = (int)internalRepresentation[v];


        for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
        {
            if (adjacencyMatrix[pos, i] != INF)
            {
                Vertex adjVertex = (Vertex)inverseRepresentation[i];
                aux.Add(adjVertex);
            }
        }

        return aux;
    }

    public static int CountAdjacents(Vertex v)
    {
        int pos = (int)internalRepresentation[v];
        int count = 0;

        for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
        {
            if (adjacencyMatrix[pos, i] != INF)
            {
                count++;
            }
        }
        return count;
    }


    public static bool IsFull()
    {
        return CountVertexes() == adjacencyMatrix.Length;
    }

    public static int CountVertexes()
    {
        return internalRepresentation.Count;
    }

    public static int CountArches()
    {
        return CountArchs;
    }
}

