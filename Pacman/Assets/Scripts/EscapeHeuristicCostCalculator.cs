using System;

public class EscapeHeuristicCostCalculator : IHeuristicCostCalculator
{
    private Vertex stayFar;

    public EscapeHeuristicCostCalculator(Vertex stayFar)
    {
        this.stayFar = stayFar;
    }

    public int Calculate(Vertex v)
    {

        int distance = (int)Math.Sqrt(Math.Pow(v.x + stayFar.x, 2) + Math.Pow(v.y + stayFar.y, 2));


        return distance;
    }
}
