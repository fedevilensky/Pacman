using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Pathfinding
{
    RUNNING_AWAY,
    CHASING
}

public class Enemy : MovingObject
{
    public GameObject killRadius;
    public const float pathfindingDelay = 1f;

    private bool calculatingPath = false;
    private int infinity = -1;
    private Vertex lastPlayerPosition = null;
    private Vertex lastPosition = null;
    private VertexEqualityComparer vertexComp = new VertexEqualityComparer();
    private Stack<Vertex> wayToDestination = new Stack<Vertex>();
    private HeuristicCostCalculator nullCalculator = new NullHeuristicCostCalculator();
    private HeuristicCostCalculator distanceCalculator = new ImplementedHeuristicCostCalculator();
    private Vertex destination;
    public float timeSinceLastPathfind;
    public Pathfinding lastPathfind;



    public class Cell
    {
        public Vertex previous;
        public int cost;
    }

    public class CellEqualityComparer : IEqualityComparer
    {
        private VertexEqualityComparer comparer;
        public CellEqualityComparer(VertexEqualityComparer comparer)
        {
            this.comparer = comparer;
        }

        public bool Equals(object o1, object o2)
        {
            Cell x = (Cell)o1;
            Cell y = (Cell)o2;
            return x.cost == y.cost && comparer.Equals(x.previous, y.previous);
        }

        public int GetHashCode(object o)
        {
            Cell obj = (Cell)o;
            return comparer.GetHashCode(obj.previous) + obj.cost;
        }
    }

    void Awake()
    {
        timeSinceLastPathfind = pathfindingDelay;
        lastPathfind = Pathfinding.RUNNING_AWAY;
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (!GameManager.instance.hasEnded)
        {
            if (coll.gameObject.tag == "Player")
            {
                if (!GameManager.instance.playerHasGun)
                {
                    GameManager.instance.gameOver = true;
                }
                CollisionWithPlayer();
            }
        }
    }

    public void CollisionWithPlayer()
    {
        rb2D.bodyType = RigidbodyType2D.Static;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!GameManager.instance.hasEnded)
        {
            Vertex playerPosition = CalcuarPosicion(GameManager.instance.player.transform.position);
            Vertex myPos = CalcuarPosicion(transform.position);
            timeSinceLastPathfind += Time.deltaTime;
            if (lastPlayerPosition == null || !vertexComp.Equals(lastPlayerPosition, playerPosition))
            {

                lastPlayerPosition = playerPosition;
                if (!calculatingPath && (lastPosition == null || !vertexComp.Equals(lastPlayerPosition, lastPosition)))
                {
                    if (lastPosition == null)
                    {
                        lastPosition = myPos;
                    }

                    if (GameManager.instance.playerHasGun)
                    {
                        killRadius.SetActive(true);
                        if (!(lastPathfind == Pathfinding.RUNNING_AWAY && timeSinceLastPathfind < pathfindingDelay))
                        {
                            if (lastPathfind != Pathfinding.RUNNING_AWAY || wayToDestination.Count < 2 || rb2D.velocity == Vector2.zero)
                            {
                               // destination = FindWaypoint();
                            }
                            lastPathfind = Pathfinding.RUNNING_AWAY;
                            AStarWithQueue(distanceCalculator);
                            timeSinceLastPathfind = 0f;
                        }

                    }
                    else
                    {
                        killRadius.SetActive(false);
                        if (!(lastPathfind == Pathfinding.CHASING && timeSinceLastPathfind < pathfindingDelay))
                        {
                            lastPathfind = Pathfinding.CHASING;
                            destination = lastPlayerPosition;
                            AStarWithQueue(nullCalculator);
                            timeSinceLastPathfind = 0f;
                        }
                    }
                }
            }

            if (wayToDestination.Count > 1)
            {
                Vector2 movementDirection = GetMovementDirection(myPos);
                Move(movementDirection);
            }
            else if (GameManager.instance.playerHasGun)
            {
                lastPlayerPosition = null;
                destination = null;
            }
        }
    }

    Vector2 GetMovementDirection(Vertex p1)
    {
        if (lastPosition == null || !vertexComp.Equals(lastPosition, p1))
        {
            lastPosition = p1;
            if (wayToDestination.Count > 1)
                wayToDestination.Pop();
        }
        Vertex p2 = wayToDestination.Peek();
        int xDirection = (p2.x - p1.x) > 0 ? 1 : ((p2.x - p1.x) < 0 ? -1 : 0);
        int yDirection = (p2.y - p1.y) > 0 ? 1 : ((p2.y - p1.y) < 0 ? -1 : 0);
        return new Vector2(xDirection, yDirection);
    }

    private Vector2 VertexToVector(Vertex position)
    {
        return new Vector2(position.x, position.y);
    }

    private Vertex CalcuarPosicion(Vector2 position)
    {
        Vector3 relPos = GameManager.instance.wallMap.WorldToCell(position);
        Vertex ret = new Vertex
        {
            x = (int)relPos.x + GameManager.instance.tileManager.booleanMap.GetLength(0) / 2 - 2,
            y = (int)relPos.y + GameManager.instance.tileManager.booleanMap.GetLength(1) / 2,
        };
        return ret;
    }

    private void AStarWithQueue(HeuristicCostCalculator costCalculator)
    {
        calculatingPath = true;
        bool[,] mat = GameManager.instance.tileManager.booleanMap;
        Hashtable camino = new Hashtable(new VertexEqualityComparerGenericObject());
        HashSet<Vertex> evaluados = new HashSet<Vertex>(new VertexEqualityComparer());
        int[] posiblesMovimientosX = { 1, -1, 0, 0 };
        int[] posiblesMovimientosY = { 0, 0, 1, -1 };
        PriorityQueue<Vertex> cola = new ImplementedPriorityQueue<Vertex>(new VertexEqualityComparerGenericObject());
        cola.InsertarConPrioridad(lastPosition, 0);
        Vertex v = new Vertex();
        while (!cola.EstaVacia())
        {
            v = cola.EliminarElementoMayorPrioridad();
            evaluados.Add(v);
            if (v.x == destination.x && v.y == destination.y)
            {
                SiguienteMovimiento(camino, v);
                calculatingPath = false;
                return;
            }

            for (int i = 0; i < posiblesMovimientosX.Length; i++)
            {

                int nuevaPosX = v.x + posiblesMovimientosX[i];
                int nuevaPosY = v.y + posiblesMovimientosY[i];
                if (nuevaPosX > -1 && nuevaPosX < mat.GetLength(0))
                {
                    if (nuevaPosY > -1 && nuevaPosY < mat.GetLength(1))
                    {
                        if (mat[nuevaPosX, nuevaPosY])
                        {
                            Vertex nuevoVertice = new Vertex
                            {
                                x = nuevaPosX,
                                y = nuevaPosY
                            };
                            if (!evaluados.Contains(nuevoVertice))
                            {
                                if (camino.Contains(nuevoVertice))
                                {
                                    Cell cNuevoVertice = (Cell)camino[nuevoVertice];
                                    Cell cV = (Cell)camino[v];
                                    if (cNuevoVertice.cost <= cV.cost)
                                    {
                                        int nuevoCosto = cV.cost - 1 + costCalculator.Calculate(nuevoVertice, lastPlayerPosition);

                                        Cell newCell = new Cell
                                        {
                                            cost = nuevoCosto,
                                            previous = v
                                        };

                                        cola.CambiarPrioridad(nuevoVertice, nuevoCosto);
                                        camino[nuevoVertice] = newCell;
                                    }
                                }
                                else
                                {
                                    int nuevoCosto;
                                    if (camino.Contains(v))
                                    {
                                        Cell c = (Cell)camino[v];
                                        nuevoCosto = c.cost - 1;
                                    }
                                    else
                                    {
                                        nuevoCosto = -1;
                                    }

                                    Cell newCell = new Cell
                                    {
                                        cost = nuevoCosto,
                                        previous = v
                                    };

                                    cola.InsertarConPrioridad(nuevoVertice, nuevoCosto);
                                    camino.Add(nuevoVertice, newCell);
                                }
                            }
                        }
                    }
                }
            }
        }
        calculatingPath = false;
        return;
    }

    private void SiguienteMovimiento(Hashtable camino, Vertex fin)
    {
        wayToDestination.Clear();
        wayToDestination.Push(fin);
        Cell c = (Cell)camino[fin];
        while (camino.Contains(c.previous))
        {
            wayToDestination.Push(c.previous);
            c = (Cell)camino[c.previous];
        }
    }
    /*
    private Vector3 FindWaypoint()
    {
        Vertex ret = lastPlayerPosition;
        int max = 0;
        foreach (Vector3 v in GameManager.instance.waypointList)
        {
            if (destination == null || !vertexComp.Equals(v, destination))
            {
                int newDistance = distanceCalculator.Calculate(v, lastPlayerPosition);
                if (max < newDistance)
                {
                    max = newDistance;
                    ret = v;
                }
            }
        }

        return ret;
    }*/
}
