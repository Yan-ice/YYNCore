using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Copyer
{
    public static Dictionary<K,V> Copy<K,V>(Dictionary<K,V> origin)
    {
        Dictionary<K, V> another = new Dictionary<K, V>();
        foreach(K s in origin.Keys)
        {
            another.Add(s, origin[s]);
        }
        return another;
    }

    public static List<T> Copy<T>(List<T> origin)
    {
        List<T> another = new List<T>();
        foreach(T val in origin)
        {
            another.Add(val);
        }
        return another;
    }
}