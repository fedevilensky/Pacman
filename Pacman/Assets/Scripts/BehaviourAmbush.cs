using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourAmbush : Behaviour {

    public BehaviourAmbush() {
        velocity = 3;
    }

    public override bool InteractOnSight() {
        return false;
    }

    public override Vector2 MoveOnSight(Vertex playerPos, Vertex enemyPos, Vector2 velocity) {
        throw new System.Exception("Shouldn't interact on sight when in Ambush Behaviour");
    }

    public override Vertex NextVertex(Vertex playerPos, Vertex enemyPos, Vector2 velocity) {
        if (Distance(playerPos, enemyPos) < 5f) {
            return enemyPos;
        }
        else if (velocity == Vector2.zero)
            return nav.GetNextStep(enemyPos, ClosestVertex.Find(playerPos));
        else {
            bool[,] booleanMap = GameManager.instance.tileManager.booleanMap;
            Vertex nextPos = playerPos;
            int minCost = 4;
            int xDirection = (velocity.x) > 0 ? 1 : ((velocity.x) < 0 ? -1 : 0);
            int yDirection = 0;
            if (xDirection == 0)
                yDirection = (velocity.y) > 0 ? 1 : ((velocity.y) < 0 ? -1 : 0);
            int thisCost = 0;
            Vertex tryPos = new Vertex() { x = playerPos.x, y = playerPos.y };
            while (GameManager.instance.tileManager.IsFloor(tryPos.x, tryPos.y) && minCost < thisCost) {

                tryPos.x += xDirection;
                tryPos.y += yDirection;
                thisCost++;
            }
            if (minCost < thisCost) {
                tryPos.x -= xDirection;
                tryPos.y -= yDirection;
            }
            return nav.AStarStep(enemyPos, ClosestVertex.Find(tryPos), new AmbushHeuristicCostCalculator(playerPos));
        }

    }

    private int Distance(Vertex v1, Vertex v2) {
        return (int)Mathf.Sqrt(Mathf.Pow(v1.x - v2.x, 2) + Mathf.Pow(v1.y - v2.y, 2));
    }
}
