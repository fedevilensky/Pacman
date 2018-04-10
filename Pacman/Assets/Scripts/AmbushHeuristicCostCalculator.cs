using System;

public class AmbushHeuristicCostCalculator : IHeuristicCostCalculator {
    private Vertex stayFar;

    public AmbushHeuristicCostCalculator(Vertex stayFar) {
        this.stayFar = stayFar;
    }

    public int Calculate(Vertex v) {
        int squaredDistance = (int)(Math.Pow(v.x + stayFar.x, 2) + Math.Pow(v.y + stayFar.y, 2));


        return squaredDistance;
    }
}
