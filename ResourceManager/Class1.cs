using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
internal class Entry<TKey, TValue>
{
    TKey key;
    TValue value;

    public Entry(TKey k, TValue v)
    {
        key = k;
        value = v;
    }
}
[Serializable]
public class SerializedDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField]
    List<TKey> keys;
    [SerializeField]
    List<TValue> values;

    Dictionary<TKey, TValue> target;
    public Dictionary<TKey, TValue> ToDictionary() { return target; }

    public SerializedDictionary(Dictionary<TKey, TValue> target)
    {
        this.target = target;
    }
    public SerializedDictionary() { }

    public bool ContainsKey(TKey k)
    {
        return target.ContainsKey(k);
    }
    public void Replace(TKey k, TValue v)
    {
        target[k] = v;
    }
    public TValue Get(TKey k)
    {
        return target[k];
    }
    public void Add(TKey k, TValue v)
    {
        target.Add(k, v);
    }
    public bool Remove(TKey key)
    {
        return target.Remove(key);
    }

    public void OnBeforeSerialize()
    {
        keys = new List<TKey>(target.Keys);
        values = new List<TValue>(target.Values);
    }

    public void OnAfterDeserialize()
    {
        var count = Math.Min(keys.Count, values.Count);
        target = new Dictionary<TKey, TValue>(count);
        for (var i = 0; i < count; ++i)
        {
            target.Add(keys[i], values[i]);
        }
    }
}
