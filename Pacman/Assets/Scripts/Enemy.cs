using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MovingObject
{
    public String error = "";

    private bool calculatingPath = false;
    private int infinity = -1;
    private Vertex lastPlayerPosition = null;
    private Vertex lastPosition = null;
    private VertexEqualityComparer vertexComp = new VertexEqualityComparer();
    private Stack<Vertex> wayToPlayer = new Stack<Vertex>();

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

    // Update is called once per frame
    void FixedUpdate()
    {
        Vertex playerPosition = CalcuarPosicion(GameManager.instance.player.transform.position);
        Vertex myPos = CalcuarPosicion(transform.position);
        if (lastPlayerPosition == null || !vertexComp.Equals(lastPlayerPosition, playerPosition))
        {

            lastPlayerPosition = playerPosition;
            if (!calculatingPath && (lastPosition == null || !vertexComp.Equals(lastPlayerPosition, lastPosition)))
            {
                if (lastPosition == null)
                {
                    lastPosition = myPos;
                }
                DijkstraWithQueue();
            }
        }

        if (wayToPlayer.Count > 1)
        {
            Vector2 movementDirection = GetMovementDirection(myPos);
            Move(movementDirection);
        }
    }

    Vector2 GetMovementDirection(Vertex p1)
    {
        if (lastPosition == null || !vertexComp.Equals(lastPosition, p1))
        {
            lastPosition = p1;
            if (wayToPlayer.Count > 1)
                wayToPlayer.Pop();
        }
        Vertex p2 = wayToPlayer.Peek();
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

    private void DijkstraWithQueue()
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
            if (v.x == lastPlayerPosition.x && v.y == lastPlayerPosition.y)
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
                                        int nuevoCosto = cV.cost - 1;

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
        error = "dijkstra falla";
        calculatingPath = false;
        return;
    }

    private void SiguienteMovimiento(Hashtable camino, Vertex fin)
    {
        wayToPlayer.Clear();
        wayToPlayer.Push(fin);
        Cell c = (Cell)camino[fin];
        while (camino.Contains(c.previous))
        {
            wayToPlayer.Push(c.previous);
            c = (Cell)camino[c.previous];
        }
        //wayToPlayer.Pop();
    }
}
