using System.Collections.Generic;

using UnityEngine;

public class Event<T> : ScriptableObject {
    private List<Listener<T>> listeners = new List<Listener<T>>();

    public void Raise(T go) {
        for (var i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventRaised(go);
    }

    public void Register_Listener(Listener<T> listener) => listeners.Add(listener);
    public void Unregister_Listener(Listener<T> listener) => listeners.Remove(listener);
}
