using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourChase : Behaviour
{
    public BehaviourChase()
    {
        velocity = 1;
    }

    public override bool InteractOnSight()
    {
        return true;
    }

    public override Vector2 MoveOnSight(Vertex playerPos, Vertex enemyPos, Vector2 velocity)
    {
        int xDirection = (playerPos.x - enemyPos.x) > 0 ? 1 : ((playerPos.x - enemyPos.x) < 0 ? -1 : 0);
        int yDirection = (enemyPos.y - playerPos.y) > 0 ? 1 : ((enemyPos.y - playerPos.y) < 0 ? -1 : 0);
        return new Vector2(xDirection, yDirection);
    }

    public override Vertex NextVertex(Vertex playerPos, Vertex enemyPos, Vector2 velocity)
    {
        playerPos = ClosestVertex.Find(playerPos);
        return nav.GetNextStep(enemyPos, playerPos);
    }
}
