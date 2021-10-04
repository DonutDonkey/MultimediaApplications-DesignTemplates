using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour {
    [SerializeField] protected GameEvent game_event;
    [Space(10)]
    [SerializeField] private UnityEvent event_response;

    private void OnEnable() => game_event.Register_Listener(this);

    private void OnDisable() => game_event.Unregister_Listener(this);

    public void OnEventRaised() => event_response.Invoke();
}
