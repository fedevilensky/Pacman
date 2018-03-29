using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StrategySelector {
    protected float velocity;
    protected Behaviour activeBehaviour;
    public string error;

    public float Velocity
    {
        get
        {
            return velocity;
        }
    }

    public abstract bool InteractOnSight();

    public abstract void Update(float deltaTime);

    public abstract Vector2 MoveOnSight(Vertex playerPos, Vertex enemyPos, Vector2 velocity);

    public abstract Vertex NextVertex(Vertex playerPos, Vertex enemyPos, Vector2 velocity);

    protected void ChangeBehaviour(Behaviour b)
    {
        activeBehaviour = b;
        velocity = b.Velocity;
    }
    
}
