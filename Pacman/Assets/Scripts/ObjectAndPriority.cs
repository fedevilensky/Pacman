using System.Collections;
using System.Collections.Generic;
using System;

public class ObjectAndPriority<T> where T:IComparable {
    public T item;
    public int priority;

    public ObjectAndPriority(T t, int p)
    {
        item = t;
        priority = p;
    }
}
