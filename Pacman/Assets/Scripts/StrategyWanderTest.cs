using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategyWanderTest : StrategySelector
{
    private float chaseTimer = 0f;
    private Behaviour chase = new BehaviourChase();
    private Behaviour wander = new BehaviourWander();
    private float MaxWanderTime = 13f;
    private float MaxChaseTime = 5f;
    private float distance;
    private float minDistance = 10f;

    public StrategyWanderTest()
    {
        ChangeBehaviour(wander);
    }

    public override bool InteractOnSight()
    {
        return true;
    }

    public override Vector2 MoveOnSight(Vertex playerPos, Vertex enemyPos, Vector2 velocity)
    {

        if (activeBehaviour != chase)
        {
            ChangeBehaviour(chase);
        }
        chaseTimer = 0f;
        return activeBehaviour.MoveOnSight(playerPos, enemyPos, velocity);
    }

    public override Vertex NextVertex(Vertex playerPos, Vertex enemyPos, Vector2 velocity)
    {
        distance = Distance(playerPos, enemyPos);
        return activeBehaviour.NextVertex(playerPos, enemyPos, velocity);
    }

    public override void Update(float deltaTime)
    {
        chaseTimer += deltaTime;
        if (chaseTimer >= MaxWanderTime && activeBehaviour == wander)
        {
            if (distance > minDistance)
            {
                ChangeBehaviour(chase);
            }
            chaseTimer = 0f;
        }
        else if (chaseTimer >= MaxChaseTime && activeBehaviour == chase)
        {
            ChangeBehaviour(wander);
            chaseTimer = 0f;
        }
    }

    private float Distance(Vertex v1, Vertex v2)
    {
        return Mathf.Sqrt(Mathf.Pow(v1.x - v2.x, 2) + Mathf.Pow(v1.y - v2.y, 2));
    }
}
