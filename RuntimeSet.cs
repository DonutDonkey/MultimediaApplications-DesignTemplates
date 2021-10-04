using System.Collections.Generic;
using UnityEngine;

public class RuntimeSet<T> : ScriptableObject {
    protected List<T> items = new List<T>();
    public T GetItemAt(int index) => items[index];

    public void Add(T t) {
        if (!items.Contains(t)) 
            items.Add(t);
    }

    public void Remove(T t) {
        if (items.Contains(t)) 
            items.Remove(t);
    }
}
