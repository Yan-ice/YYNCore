using System.Collections.Generic;

public struct JsonSet<T> : JsonSerializable
{
    public string m_tag;
    public List<T> m_list;
}