//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class BehaviourEscape : Behaviour
//{
//    private Vertex destination;

//    public BehaviourEscape()
//    {
//        velocity = 0.9f;
//    }

//    public override bool InteractOnSight()
//    {
//        return false;
//    }

//    public override Vector2 MoveOnSight(Vertex playerPos, Vertex enemyPos, Vector2 velocity)
//    {
//        throw new System.Exception("Shouldn't interact on sight when in Escape Behaviour");
//    }

//    public override Vertex NextVertex(Vertex playerPos, Vertex enemyPos, Vector2 velocity)
//    {
//        if (destination == null || new VertexEqualityComparer().Equals(enemyPos, destination))
//        {
//            int shortestDistance = 100;
//            int aux = Random.Range(0, Graph.CountVertexes());
//            foreach (Vertex v in Graph.GetEveryVertex())
//            {
//                if(Distance(v,playerPos)>13&& Distance(v, enemyPos) < shortestDistance)
//                {
//                    destination = v;
//                    shortestDistance = Distance(v, enemyPos);
//                }
//            }
//        }
//        return nav.AStarStep(enemyPos, destination, heuristic);
//    }


//    private int Distance(Vertex v1, Vertex v2)
//    {
//        return (int)Mathf.Sqrt(Mathf.Pow(v1.x - v2.x, 2) + Mathf.Pow(v1.y - v2.y, 2));
//    }

//    //a partir de este podemos hacer el hide sacandole el equals y metiendole 
//}