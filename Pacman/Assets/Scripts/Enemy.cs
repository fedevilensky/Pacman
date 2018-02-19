using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MovingObject
{

    private int infinity = -1;

    public class Cell
    {
        public Vertex previous;
        public int cost;
    }

    public class Vertex : IComparable
    {
        public int x;
        public int y;

        public int CompareTo(object obj)
        {
            Vertex aux = obj as Vertex;
            if (x == aux.x && y == aux.y)
            {
                return 0;
            }
            return 1;
        }
    }


    private Vertex CalcuarPosicion()
    {
        Vector3 relPos = TilemapManager.instance.wallMap.WorldToCell(transform.position);
        Vertex ret = new Vertex
        {
            x = (int)relPos.x,
            y = (int)relPos.y
        };
        return ret;
    }

    private Vertex DijkstraWithQueue(Vertex fin)
    {

        bool[,] mat = TilemapManager.instance.booleanMap;
        Hashtable camino = new Hashtable();
        HashSet<Vertex> evaluados = new HashSet<Vertex>();
        int[] posiblesMovimientos = { 1, -1, 0 };

        PriorityQueue<Vertex> cola = new ImplementedPriorityQueue<Vertex>();

        cola.InsertarConPrioridad(CalcuarPosicion(), 0);

        while (!cola.EstaVacia())
        {
            Vertex v = cola.EliminarElementoMayorPrioridad();
            evaluados.Add(v);
            if (v.CompareTo(fin) == 0)
            {
                return SiguienteMovimiento(camino, fin);
            }

            for (int i = 0; i < posiblesMovimientos.Length; i++)
            {
                for (int j = 0; j < posiblesMovimientos.Length; j++)
                {
                    int nuevaPosX = v.x + posiblesMovimientos[i];
                    int nuevaPosY = v.y + posiblesMovimientos[j];
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
                                if (evaluados.Contains(nuevoVertice))
                                {
                                    continue;
                                }
                                else
                                {
                                    if (camino.Contains(nuevoVertice))
                                    {
                                        Cell cNuevoVertice = (Cell)camino[nuevoVertice];
                                        Cell cV = (Cell)camino[v];
                                        if (cNuevoVertice.cost <= cV.cost)
                                        {
                                            int nuevoCosto = cV.cost + 1;

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
                                        Cell c = (Cell)camino[v];
                                        int nuevoCosto = c.cost + 1;

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
        }
        return null;
    }

    private Vertex SiguienteMovimiento(Hashtable camino, Vertex fin)
    {
        Cell c = (Cell)camino[fin];
        while (camino.Contains(c.previous))
        {
            c = (Cell)camino[c.previous];
        }
        return c.previous;
    }
}
