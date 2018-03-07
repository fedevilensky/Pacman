using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphVertex
{
    private readonly int _cost;
    public Vertex vertex;
    public int Cost
    {
        get; set;
    }

    public void IncrementCost()
    {
        Cost++;
    }

    public void DecrementCost()
    {
        Cost--;
    }
}

