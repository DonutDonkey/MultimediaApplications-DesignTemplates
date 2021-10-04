using UnityEngine;
using UnityEngine.Events;

public abstract class Listener<T> : MonoBehaviour {
    public abstract Event<T> game_event { get; }
    public abstract UnityEvent<T> event_response { get; }

    private void OnEnable() => game_event.Register_Listener(this);
    private void OnDisable() => game_event.Unregister_Listener(this);
    public void OnEventRaised(T value) => event_response.Invoke(value);
}
