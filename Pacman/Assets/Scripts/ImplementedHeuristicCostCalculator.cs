using System;

public class ImplementedHeuristicCostCalculator : HeuristicCostCalculator
{

    public override int Calculate(Vertex v1, Vertex v2)
    {

        int squaredDistance = -10000 + (int)(Math.Pow(v1.x + v2.x, 2) + Math.Pow(v1.y + v2.y, 2));


        return squaredDistance;
    }
}
