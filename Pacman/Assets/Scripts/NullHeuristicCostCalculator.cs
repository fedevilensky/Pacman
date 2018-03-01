
public class NullHeuristicCostCalculator : HeuristicCostCalculator
{

    public override int Calculate(Vertex v1, Vertex v2)
    {
        return 0;
    }
}
