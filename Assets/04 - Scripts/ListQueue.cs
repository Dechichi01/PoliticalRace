using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ListQueue<T> : Queue<T>
{
    public T last { get; private set; }
    public T first { get; private set; }

    public List<T> list { get; private set; }

    public ListQueue()
    {
        first = last = default(T);
        list = new List<T>();
    }

    public new void Enqueue(T item)
    {
        if (this.Count == 0) first = item;
        list.Add(item);
        last = item;
        base.Enqueue(item);
    }

    public new T Dequeue()
    {
        this.list.RemoveAt(0);
        if (list.Count > 0)
        {
            first = list[0];
            last = list[list.Count - 1];
        }
        else
        {
            first = last = default(T);
        }

        return base.Dequeue();
    }
}
