using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategyEscapeTest : StrategySelector
{
    private float chaseTimer = 0f;
    //private Behaviour chase = new BehaviourChase();
    private Behaviour escape = new BehaviourEscape();
    private float MaxWanderTime = 13f;
    private float MaxChaseTime = 5f;
    private float distance;
    private float minDistance = 10f;

    public StrategyEscapeTest()
    {
        ChangeBehaviour(escape);
    }

    public override bool InteractOnSight()
    {
        return false;
    }

    public override Vector2 MoveOnSight(Vertex playerPos, Vertex enemyPos, Vector2 velocity)
    {
        return activeBehaviour.MoveOnSight(playerPos, enemyPos, velocity);
    }

    public override Vertex NextVertex(Vertex playerPos, Vertex enemyPos, Vector2 velocity)
    {
        return activeBehaviour.NextVertex(playerPos, enemyPos, velocity);
    }

    public override void Update(float deltaTime)
    {
        error = activeBehaviour.error;
    }

    private float Distance(Vertex v1, Vertex v2)
    {
        return Mathf.Sqrt(Mathf.Pow(v1.x - v2.x, 2) + Mathf.Pow(v1.y - v2.y, 2));
    }
}
