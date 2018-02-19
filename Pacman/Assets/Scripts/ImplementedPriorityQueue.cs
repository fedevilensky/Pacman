using System.Collections;
using System.Collections.Generic;
using System;


public class ImplementedPriorityQueue<T> : PriorityQueue<T> where T : IComparable
{
    private ObjectAndPriority<T>[] heap;
    private int size;
    private Hashtable itemToPosMap = new Hashtable();

    public override void CambiarPrioridad(T e, int p)
    {
        int pos = (int)itemToPosMap[e];

        heap[pos].priority = p;
        Hundir(pos);
        Flotar(pos);

    }

    public override void EliminarElemento(T e)
    {
        int pos =(int) itemToPosMap[e];
        itemToPosMap.Remove(e);
        heap[pos] = heap[size];
        size--;
        if(size >0 && pos <= size)
        {
            itemToPosMap[heap[pos].item] = pos;
            Hundir(pos);
        }
    }

    public override T EliminarElementoMayorPrioridad()
    {
        T ret = heap[1].item;
        heap[1] = heap[size];
        heap[size] = heap[0];
        itemToPosMap.Remove(ret);

        if (size > 0)
        {
            itemToPosMap[heap[1].item] = 1;
            Hundir(1);
        }


        return ret;
    }

    public override bool EstaLlena()
    {
        return false;
    }

    public override bool EstaVacia()
    {
        return size > 0;
    }

    public override void InsertarConPrioridad(T e, int p)
    {
        size++;
        if(size > heap.Length)
        {
            ObjectAndPriority<T>[] aux = new ObjectAndPriority<T>[heap.Length * 2];
            for(int i = 1; i < heap.Length; i++)
            {
                aux[i] = heap[i];
            }
            heap = aux;
        }
        heap[size] = new ObjectAndPriority<T>(e, p);
        itemToPosMap.Add(e, size);

        Flotar(size);

    }

    public override int Largo()
    {
        return size;
    }

    public override T ObtenerElementoMayorPrioridad()
    {
        return heap[1].item;
    }

    public override ObjectAndPriority<T> ObtenerElementoYPrioridad(T e)
    {
        return heap[(int)itemToPosMap[e]];
    }

    public override bool Pertenece(T e)
    {
        return itemToPosMap.Contains(e);
    }

    public override void Vaciar()
    {
        size = 0;
        heap = new ObjectAndPriority<T>[heap.Length];
        itemToPosMap.Clear();
    }

    private void Hundir(int pos)
    {
        if (!NoTieneHijos(pos))
        {
            int hijoMayor = HijoMayor(pos);
            if ((heap[hijoMayor].priority > heap[pos].priority))
            {
                IntercambiarPosicion(pos, hijoMayor);
                Hundir(hijoMayor);
            }
        }
    }

    private void Flotar(int pos)
    {
        if (pos != 1)
        {
            int padre = Padre(pos);
            if (heap[pos].priority > heap[padre].priority)
            {
                IntercambiarPosicion(pos, padre);
                Flotar(padre);
            }
        }
    }

    private int Padre(int i)
    {
        return i / 2;
    }

    private int HijoMenor(int i)
    {
        return 2 * i;
    }

    private int HijoMayor(int i)
    {
        return 2 * i + 1;
    }

    private bool NoTieneHijos(int i)
    {
        return HijoMenor(i) > size;
    }

    private void IntercambiarPosicion(int pos1, int pos2)
    {
        T t1 = heap[pos1].item;
        ObjectAndPriority<T> t2 = heap[pos2];

        heap[pos2] = heap[pos1];
        heap[pos1] = t2;

        itemToPosMap[t1] = pos2;
        itemToPosMap[t2.item] =  pos1;
    }

}
