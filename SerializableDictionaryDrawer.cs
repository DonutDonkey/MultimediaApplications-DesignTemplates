using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Editor {
    public class SerializableDictionaryDrawer<TKey, TValue> : PropertyDrawer {
        private SerializableDictionary<TKey, TValue> _dictionary;
        private bool _foldout;
        private const float button_width = 22f;
        private const int margin = 3;
        
        private static readonly GUIContent icon_toolbar_minus = EditorGUIUtility.IconContent("Toolbar Minus", "Remove selection from list");
        private static readonly GUIContent icon_toolbar_plus = EditorGUIUtility.IconContent("Toolbar Plus", "Add to list");
        private static readonly GUIStyle pre_button = "RL FooterButton";
        private static readonly GUIStyle box_background = "RL Background";
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            CheckInitialize(property, label);
            return _foldout ? Mathf.Max((_dictionary.Count + 1) * 17f, 17 + 16) + margin * 2 : 17f + margin * 2;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            CheckInitialize(property, label);

            DrawBackgroundRect(position);

            position.y += margin;
            position.height = 17f;
            DrawFoldout(position, label);

            position.xMin += margin;
            position.xMax -= margin;
            DrawAddAndClearButtons(position);

            if (!_foldout)
                return;
            
            DrawLabel(position);

            foreach (var item in _dictionary) {
                var key = item.Key;
                var value = item.Value;

                position.y += 17f;

                var key_rect = position;

                key_rect.width /= 2;
                key_rect.width -= 4;

                EditorGUI.BeginChangeCheck();

                var new_key = DrawField(key_rect, typeof(TKey), key);

                if (EditorGUI.EndChangeCheck()) {
                    try {
                        _dictionary.Remove(key);
                        _dictionary.Add(new_key, value);
                    }
                    catch (Exception e) {
                        Debug.LogError(e.Message);
                    }
                }

                var value_rect = position;
                value_rect.xMin = key_rect.xMax;
                value_rect.xMax = position.xMax - button_width;


                EditorGUI.BeginChangeCheck();
                value = DrawField(value_rect, typeof(TValue), value);

                if (EditorGUI.EndChangeCheck()) {
                    _dictionary[key] = value;
                    break;
                }

                var remove_rect = value_rect;
                remove_rect.xMin = remove_rect.xMax - button_width;
            
                if (GUI.Button(remove_rect, icon_toolbar_minus, pre_button)) {
                    RemoveItem(key);
                    break;
                }
            }
            // EditorGUI.EndProperty();
        }
        
        private static void DrawBackgroundRect(Rect position) {
            var background_rect = position;
            background_rect.xMin -= 7;
            background_rect.height += margin;

            if (Event.current.type == EventType.Repaint)
                box_background.Draw(background_rect, false, false, false, false);
        }

        private void DrawFoldout(Rect position, GUIContent label) {
            var foldout_rect = position;
            foldout_rect.width -= 2 * button_width;
            
            EditorGUI.BeginChangeCheck();
            _foldout = EditorGUI.Foldout(foldout_rect, _foldout, label, true);
            
            if (EditorGUI.EndChangeCheck())
                EditorPrefs.SetBool(label.text, _foldout);
        }

        private void DrawAddAndClearButtons(Rect position) {
            var button_rect = position;

            button_rect.xMin = position.xMax - button_width;
            if (GUI.Button(button_rect, icon_toolbar_minus, pre_button))
                ClearDictionary();

            button_rect.x -= button_width - 1;
            if (GUI.Button(button_rect, icon_toolbar_plus, pre_button))
                AddNewItem();
        }
        
        private void DrawLabel(Rect position) {
            var label_rect = position;
            label_rect.y += 16;

            if (_dictionary.Count == 0)
                GUI.Label(label_rect, "This dictionary doesn't have any items. Click + to add one!");
        }
        
        private static readonly Dictionary<Type, Func<Rect, object, object>> _fields =
            new Dictionary<Type,Func<Rect,object,object>>() {
                { typeof(int), (rect, value) => EditorGUI.IntField(rect, (int)value) },
                { typeof(float), (rect, value) => EditorGUI.FloatField(rect, (float)value) },
                { typeof(string), (rect, value) => EditorGUI.TextField(rect, (string)value) },
                { typeof(bool), (rect, value) => EditorGUI.Toggle(rect, (bool)value) },
                { typeof(Vector2), (rect, value) => EditorGUI.Vector2Field(rect, GUIContent.none, (Vector2)value) },
                { typeof(Vector3), (rect, value) => EditorGUI.Vector3Field(rect, GUIContent.none, (Vector3)value) },
                { typeof(Bounds), (rect, value) => EditorGUI.BoundsField(rect, (Bounds)value) },
                { typeof(Rect), (rect, value) => EditorGUI.RectField(rect, (Rect)value) },
            };
        
        private static T DrawField<T>(Rect rect, Type type, T value) {
            if (_fields.TryGetValue(type, out var field))
                return (T)field(rect, value);
 
            if (type.IsEnum)
                return (T)(object)EditorGUI.EnumPopup(rect, (Enum)(object)value);
 
            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                return (T)(object)EditorGUI.ObjectField(rect, (UnityEngine.Object)(object)value, type, true);
 
            Debug.Log("Type is not supported: " + type);
            return value;
        }
        
        private void AddNewItem() {
            var key = typeof(TKey) == typeof(string) ? (TKey) ("" as object) : default(TKey);
            var value = default(TValue);

            try {
                _dictionary.Add(key!, value);
            } catch (Exception e) {
                Debug.LogError(e);
            }
        }
        private void RemoveItem(TKey tkey) => _dictionary.Remove(tkey);
        private void ClearDictionary() => _dictionary.Clear();
        
        private void CheckInitialize(SerializedProperty property, GUIContent label) {
            if (_dictionary != null) 
                return;

            _dictionary =
                fieldInfo.GetValue(property.serializedObject.targetObject) as SerializableDictionary<TKey, TValue> ??
                new SerializableDictionary<TKey, TValue>();

            fieldInfo.SetValue(property.serializedObject.targetObject, _dictionary);
            _foldout = EditorPrefs.GetBool(label.text);
        }
}
    
    [CustomPropertyDrawer(typeof(SerializableDictionary_string_bool))]
    public class StringBoolDictionaryDrawer : SerializableDictionaryDrawer<string, bool> { }
    [CustomPropertyDrawer(typeof(SerializableDictionary_string_int))]
    public class StringIntDictionaryDrawer : SerializableDictionaryDrawer<string, int> { }
    [CustomPropertyDrawer(typeof(SerializableDictionary_string_float))]
    public class StringFloatDictionaryDrawer : SerializableDictionaryDrawer<string, float> { }
}
