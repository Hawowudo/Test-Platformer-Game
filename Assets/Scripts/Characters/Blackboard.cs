using System;
using System.Collections.Generic;
using UnityEngine;

//Handles runtime variables for both player and ai logic systems.
public class Blackboard : MonoBehaviour
{
    public enum ValueType
    {
        Int,
        Float,
        Bool,
        String,
        Vector3,
        Vector2,
        GameObject,
        Transform
    }

    [Serializable]
    public class Entry
    {
        public string key;
        public ValueType type;

        public int intValue;
        public float floatValue;
        public bool boolValue;
        public string stringValue;
        public Vector2 vector2Value;
        public Vector3 vector3Value;
        public GameObject gameObjectValue;
        public Transform transformValue;

        public object GetValue()
        {
            return type switch
            {
                ValueType.Int => intValue,
                ValueType.Float => floatValue,
                ValueType.Bool => boolValue,
                ValueType.String => stringValue,
                ValueType.Vector2 => vector2Value,
                ValueType.Vector3 => vector3Value,
                ValueType.GameObject => gameObjectValue,
                ValueType.Transform => transformValue,
                _ => null
            };
        }

        public void SetValue(object value)
        {
            switch (value)
            {
                case int v: intValue = v; type = ValueType.Int; break;
                case float v: floatValue = v; type = ValueType.Float; break;
                case bool v: boolValue = v; type = ValueType.Bool; break;
                case string v: stringValue = v; type = ValueType.String; break;
                case Vector2 v: vector2Value = v; type = ValueType.Vector2; break;
                case Vector3 v: vector3Value = v; type = ValueType.Vector3; break;
                case GameObject v: gameObjectValue = v; type = ValueType.GameObject; break;
                case Transform v: transformValue = v; type = ValueType.Transform; break;
            }
        }
    }

    [SerializeField]
    private List<Entry> variables = new();

    private readonly Dictionary<string, Entry> _lookup = new();

    private void Awake()
    {
        RebuildLookup();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        RebuildLookup();
    }
#endif

    private void RebuildLookup()
    {
        _lookup.Clear();

        foreach (var entry in variables)
        {
            if (string.IsNullOrWhiteSpace(entry.key))
                continue;

            _lookup[entry.key] = entry;
        }
    }

    public T Get<T>(string key)
    {
        key = key.ToLower();
        if (_lookup.TryGetValue(key, out var entry))
            return (T)entry.GetValue();

        return default;
    }

    public bool TryGet<T>(string key, out T value)
    {
        key = key.ToLower();
        if (_lookup.TryGetValue(key, out var entry))
        {
            object obj = entry.GetValue();

            if (obj is T typed)
            {
                value = typed;
                return true;
            }
        }

        value = default;
        return false;
    }

    public void Set<T>(string key, T value)
    {
        key = key.ToLower();

        if (_lookup.TryGetValue( key, out var entry))
        {
            entry.SetValue(value);
            return;
        }

        var newEntry = new Entry
        {
            key = key
        };

        newEntry.SetValue(value);

        variables.Add(newEntry);
        _lookup[key] = newEntry;
    }

    public bool Remove(string key)
    {
        key = key.ToLower();

        if (!_lookup.TryGetValue(key, out var entry))
            return false;

        variables.Remove(entry);
        _lookup.Remove(key);

        return true;
    }

    public bool HasKey(string key)
    {
        return _lookup.ContainsKey(key);
    }
}
