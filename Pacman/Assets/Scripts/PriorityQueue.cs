using System.Collections;
using System.Collections.Generic;
using System;

public abstract class PriorityQueue<T> where T:IComparable {
    
    // PRE: -
    // POS: Inserta el elemento e con prioridad p
    public abstract void InsertarConPrioridad( T  e,  int  p);

	// PRE: La cola de prioridad no está vacía
	// POS: Retorna el elemento de mayor prioridad en la cola eliminándolo
	public abstract T EliminarElementoMayorPrioridad();

    // PRE: La cola no está vacía
    // POS: Retorna el elemento de mayor prioridad en la cola sin eliminarlo
    public abstract  T  ObtenerElementoMayorPrioridad();
	
	// PRE: El elemento e pertenece a la cola.
	// POS: Retorna una tupla que contiene al elemento e y su prioridad.
	public abstract ObjectAndPriority<T> ObtenerElementoYPrioridad( T  e);

	// PRE: -
	// POS: Retorna el largo de la cola de prioridad
	public abstract int Largo();
	
	// PRE: -
	// POS: Retorna true si y solo si el elemento e pertenece a la cola.
	public abstract bool Pertenece( T  e);

	// PRE: El elemento e pertenece a la cola.
	// POS: El elemento e tiene una nueva prioridad p.
	public abstract void CambiarPrioridad( T  e,  int  p);
	
	// PRE: El elemento e pertenece a la cola.
	// POS: El elemento e no pertenece a la cola.
	public abstract void EliminarElemento(T  e);

	// PRE: -
	// POS: Retorna true si y solo si la cola esta vacia
	public abstract bool EstaVacia();

	// PRE: -
	// POS: Retorna true si y solo si la cola esta llena
	public abstract bool EstaLlena();

	// PRE: -
	// POS: La cola esta vacía
	public abstract void Vaciar();

    public class ElementAndPriority<T>
    {

        private T element;
        private int priority;

        ElementAndPriority(T elem, int prio)
        {
            element = elem;
            priority = prio;
        }

        public T GetElement()
        {
            return element;
        }

        public int GetPriority()
        {
            return priority;
        }

    }
}
