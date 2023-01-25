using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[JsonConverter(typeof(SpecConverter))]
public class SpecTypeJsonBox : JsonSerializable
{
    public object m_object;
    public SpecTypeJsonBox(object obj)
    {
        this.m_object = obj;
    }
}

class SpecConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(SpecTypeJsonBox);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);

        JToken typ;
        jsonObject.TryGetValue("target_type", out typ);
        Type type = Type.GetType(typ.ToString());

        jsonObject.TryGetValue("object", out typ);

        object o = JsonConvert.DeserializeObject(typ.ToString(),type);
        SpecTypeJsonBox nobj = new SpecTypeJsonBox(o);
        return nobj;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        //new一个JObject对象,JObject可以像操作对象来操作json
        var jobj = new JObject();
        //value参数实际上是你要序列化的Model对象，所以此处直接强转
        SpecTypeJsonBox model = value as SpecTypeJsonBox;
        jobj.Add("target_type", model.m_object.GetType().FullName);
        jobj.Add("object", JObject.Parse(JsonConvert.SerializeObject(model.m_object)));


        //调用该方法，把json放进去，最终序列化Model对象的json就是jsonstr，由此，我们就能自定义的序列化对象了
        serializer.Serialize(writer, jobj);
    }
}