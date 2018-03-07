using System.Collections;
using System.Collections.Generic;
public class Graph
{
    public int CountVertex { get; private set; }
    public int CountArcs { get; private set; }
    private int[,] adjacencyMatrix;
    private Hashtable internalRepresentation = new Hashtable(new VertexEqualityComparerGenericObject());
    private Hashtable inverseRepresentation = new Hashtable();


    public Graph(int size)
    {
        adjacencyMatrix = new int[size, size];
        for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < adjacencyMatrix.GetLength(1); j++)
            {
                adjacencyMatrix[i, j] = 0;
            }
        }
        CountArcs = 0;
        CountVertex = 0;

    }

    public void AddVertex(GraphVertex gV)
    {
        AddVertex(gV.vertex);
    }

    public void AddVertex(Vertex v)
    {
        if (!IsFull() && !internalRepresentation.Contains(v))
        {
            CountVertex++;
            int i = 0;
            bool aux;
            do
            {
                aux = inverseRepresentation.Contains(i);
                i = (aux) ? i + 1 : i;
            } while (aux);
            internalRepresentation.Add(v, i);
            inverseRepresentation.Add(i, v);

        }
    }

    public void AddArc(GraphVertex to, GraphVertex from)
    {
        if (adjacencyMatrix[(int)internalRepresentation[to.vertex], (int)internalRepresentation[from.vertex]] != 0)
        {
            CountArcs++;
            adjacencyMatrix[(int)internalRepresentation[to.vertex], (int)internalRepresentation[from.vertex]] = to.Cost;
        }
    }

    public void RemoveVertex(GraphVertex gV)
    {
        RemoveVertex(gV.vertex);
    }

    public void RemoveVertex(Vertex v)
    {
        if (internalRepresentation.Contains(v))
        {
            CountVertex++;
            int aux = (int)internalRepresentation[v];
            for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
            {
                adjacencyMatrix[aux, i] = 0;
                adjacencyMatrix[i, aux] = 0;
            }
            internalRepresentation.Remove(v);
            inverseRepresentation.Remove(aux);

        }
    }

    public void RemoveArc(GraphVertex gV1, GraphVertex gV2)
    {
        RemoveArc(gV1.vertex, gV2.vertex);
    }

    public void RemoveArc(Vertex v1, Vertex v2)
    {
        if (internalRepresentation.Contains(v1) && internalRepresentation.Contains(v2))
        {
            int a = (int)internalRepresentation[v1];
            int b = (int)internalRepresentation[v2];
            if (adjacencyMatrix[a, b] != 0)
            {
                CountArcs--;
                adjacencyMatrix[a, b] = 0;
                adjacencyMatrix[b, a] = 0;
            }
        }
    }

    public IEnumerable<Vertex> GetEveryVertex()
    {
        List<Vertex> aux = new List<Vertex>();
        for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
        {
            if (inverseRepresentation.Contains(i))
            {
                aux.Add((Vertex)inverseRepresentation[i]);
            }
        }

        return aux;
    }

    public IEnumerable<GraphVertex> GetAdjacents(GraphVertex gV)
    {
        return GetAdjacents(gV.vertex);
    }

    public IEnumerable<GraphVertex> GetAdjacents(Vertex v)
    {
        List<GraphVertex> aux = new List<GraphVertex>();
        int pos = (int)internalRepresentation[v];


        for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
        {
            if (adjacencyMatrix[pos, i] != 0)
            {
                GraphVertex newGVertex = new GraphVertex()
                {
                    vertex = (Vertex)inverseRepresentation[i],
                    Cost = adjacencyMatrix[pos, i]
                };
                aux.Add(newGVertex);
            }
        }

        return aux;
    }

    public int CountAdjacents(GraphVertex gV)
    {
        return CountAdjacents(gV.vertex);
    }

    public int CountAdjacents(Vertex v)
    {
        int pos = (int)internalRepresentation[v];
        int count = 0;

        for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
        {
            if (adjacencyMatrix[pos, i] != 0)
            {
                count++;
            }
        }
        return count;
    }


    public bool IsFull()
    {
        return internalRepresentation.Count == adjacencyMatrix.Length;
    }
}

