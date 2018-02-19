using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {

    private int infinity = -1;

    struct Cell
    {
        public int cost, previous;
        public bool known;
    }

    private Cell[] InitializeTable(int vertex)
    {
        Cell[] pathList = new Cell[TilemapManager.instance.booleanMap.Length];
        for (int i = 0; i < pathList.Length; i++)
        {
            pathList[i] = new Cell
            {
                cost = infinity,
                previous = infinity,
                known = false
            };

        }
        return pathList;
    }
/*
    void DijkStraWithQueue(int vertice)
    {
        Array <celda> tabla = InicializarTabla();
        tabla[vertice].costo = 0;
        Puntero <ColaPrioridad> cola = new ...;
        cola -> Encolar(v, 0);
        while (!cola -> EstaVacia()){
            int v = cola -> Desencolar();
            for (int w = 1; w & lt;= cantVertices; w++){
                if (mat[v][w] != INF)
                {
                    //para los adyacentes
                    if (tabla[w].costo >; mat[v][w] + tabla[v].costo){
                        tabla[w].costo = mat[v][w] + tabla[v].costo;
                        //si el costo
                        tabla[w].anterior = v;
                    }
                }
            }
        }
    }*/
}
