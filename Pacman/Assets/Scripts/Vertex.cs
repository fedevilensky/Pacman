using System;
using System.Collections;
using System.Collections.Generic;

public class Vertex : IComparable
{
    public int x;
    public int y;

    public int CompareTo(object obj)
    {
        Vertex aux = obj as Vertex;
        if (x == aux.x && y == aux.y)
        {
            return 0;
        }
        return 1;
    }

    public override string ToString()
    {
        return "x = " + x + " | y = " + y;
    }
}

public class VertexEqualityComparer : IEqualityComparer<Vertex>
{
    public bool Equals(Vertex v1, Vertex v2)
    {
        return v1.x == v2.x && v1.y == v2.y;
    }

    public int GetHashCode(Vertex obj)
    {
        return obj.x + obj.x * obj.y + obj.y;
    }
}

public class VertexEqualityComparerGenericObject : IEqualityComparer
{
    public new bool Equals(object o1, object o2)
    {
        Vertex x = (Vertex)o1;
        Vertex y = (Vertex)o2;

        return x.x == y.x && x.y == y.y;
    }

    public int GetHashCode(object o)
    {
        Vertex obj = (Vertex)o;

        return obj.x + obj.x * obj.y + obj.y;
    }
}