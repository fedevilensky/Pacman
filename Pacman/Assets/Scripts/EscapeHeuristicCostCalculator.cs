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

        double distance = Math.Sqrt(Math.Pow(v.x - stayFar.x, 2) + Math.Pow(v.y - stayFar.y, 2));
        if(distance < 1)
        {
            return (int)Math.Pow(10, 3);
        }
        int inverseDistance = (int)(Math.Pow(8 / distance, 3)+1);

        return inverseDistance;
    }
}
