using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
    [SerializeField, HideInInspector] private List<TKey> keys = new List<TKey>();
    [SerializeField, HideInInspector] private List<TValue> values = new List<TValue>();

    void ISerializationCallbackReceiver.OnBeforeSerialize() {
        this.keys.Clear();
        this.values.Clear();
            
        foreach(var pair in this) {
            this.keys.Add(pair.Key);
            this.values.Add(pair.Value);
        }
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize() {
        this.Clear();
        for (var i = 0; i < this.keys.Count && i < this.values.Count; i++)
            this[this.keys[i]] = this.values[i];
    }
}

[Serializable]
public class SerializableDictionary_string_bool : SerializableDictionary<string, bool> { }
[Serializable]
public class SerializableDictionary_string_int : SerializableDictionary<string, int> { }
[Serializable]
public class SerializableDictionary_string_float : SerializableDictionary<string, float> { }
