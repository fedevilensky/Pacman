using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourWander : Behaviour
{
    private Vertex destination;

    public BehaviourWander()
    {
        velocity = 0.5f;
    }

    public override bool InteractOnSight()
    {
        return true;
    }

    public override Vector2 MoveOnSight(Vertex playerPos, Vertex enemyPos, Vector2 velocity)
    {
        throw new System.Exception("Should change behavior when on sight while wandering");
    }

    public override Vertex NextVertex(Vertex playerPos, Vertex enemyPos, Vector2 velocity)
    {
        if (destination == null || new VertexEqualityComparer().Equals(enemyPos, destination))
        {
            int aux = Random.Range(0, Graph.CountVertexes());
            foreach (Vertex v in Graph.GetEveryVertex())
            {
                if (aux == 0)
                {
                    destination = v;
                    break;
                }
                aux--;
            }
        }
        return nav.GetNextStep(enemyPos, destination);
    }

    //El sight del wander es verlo literalmente a una distancia corta aprox 3, cuando lo ve tenemos que cambiar a chase, si en chase la distancia absoluta 
    //pasa a ser mayor a un valor arbitrario o paso cierto tiempo, vuelve a wonder
}
