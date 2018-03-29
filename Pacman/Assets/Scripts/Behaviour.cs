using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Behaviour {
    protected float velocity;

    protected Navigator nav = new Navigator();

    public float Velocity
    {
        get
        {
            return velocity;
        }
    }

    public abstract bool InteractOnSight();

    public abstract Vector2 MoveOnSight(Vertex playerPos, Vertex enemyPos, Vector2 velocity);

    public abstract Vertex NextVertex(Vertex playerPos, Vertex enemyPos, Vector2 velocity);
}
