using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Merger
{
    /// <summary>
    /// 合并两个字典。若合并时有条目Key冲突，则保留overriding（后者）的值。
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="origin"></param>
    /// <param name="overridding"></param>
    /// <returns></returns>
    public static Dictionary<K,V> Merge<K,V>(Dictionary<K,V> origin, Dictionary<K,V> overridding)
    {
        Dictionary<K, V> another = Copyer.Copy(origin);
        foreach(K s in overridding.Keys)
        {
            if (another.ContainsKey(s))
            {
                another[s] = overridding[s];
            }
            else
            {
                another.Add(s, overridding[s]);
            }
        }
        return another;
    }

    public static List<T> Merge<T>(List<T> origin, List<T> extra)
    {
        List<T> another = Copyer.Copy(origin);
        foreach(T val in extra)
        {
            if (!another.Contains(val))
            {
                another.Add(val);
            }
        }
        return another;
    }
}