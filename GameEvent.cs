using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Generate a new event in editor when you want systems to act when certain actions happens in game.
/// </summary>
[CreateAssetMenu(fileName = "NEW_GAME_EVENT", menuName = "Scriptable Objects/Game Event")]
public class GameEvent : ScriptableObject {
    private List<GameEventListener> listeners = new List<GameEventListener>();

    /// <summary>
    /// Use inside other class or classes, when called all the listeners will response with assigned Unity Event
    /// </summary>
    public void Raise() {
        for (var i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventRaised();
    }

    public void Register_Listener(GameEventListener listener) => listeners.Add(listener);
    public void Unregister_Listener(GameEventListener listener) => listeners.Remove(listener);
}
