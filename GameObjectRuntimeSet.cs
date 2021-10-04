using System.Linq;

using UnityEngine;

[CreateAssetMenu(fileName = "GAME_OBJECT_RUNTIMESET", menuName = "Scriptable Objects/Runtime Sets/Game Object", order = 0)]
public class GameObjectRuntimeSet : RuntimeSet<GameObject> {
    public GameObject GetObjectWithTag(string tag) => items.Find(i => i.CompareTag(tag));

    public GameObject GetObjectWithName(string in_name) => items.Find(i => i.name.Equals(in_name));

    public T GeObjectComponent<T>() where T : Component => 
        items.Select(i => i.GetComponent<T>()).FirstOrDefault();
}
