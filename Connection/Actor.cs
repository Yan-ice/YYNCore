using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Actor : Singleton<Actor>, MessageListener
{
    ClientSocket client_socket;
    public Actor()
    {
        client_socket = ClientSocket.Instance;
        client_socket.registerMessageListener(this);
        
    }
 
    public void UseCard(int data, LocInFightArea l1, LocInFightArea l2)
    {
        Debug.Log("Use card invoked: "+data);
        CardData cd = FightController.Instance.m_actingEntity.Index2Card(data);
        if (cd == null)
        {
            Debug.Log("未找到卡牌ID: "+data);
        }
        FightController.Instance.UseCard(cd, l1, l2);
    }
    public void EndTurn(int team)
    {
        if(FightController.Instance.m_actingEntity!=null && 
            team == FightController.Instance.m_actingEntity.team)
        {
            FightController.Instance.EndTurn();
        }
        
    }


    //下面这个是通信管道
    public string Identity()
    {
        return "p1";
    }

    public void onMessageReceived(Socket from, Message message)// message支持key乱序
    {
        Debug.Log("onMessageReceived Function");
        List<string> keys = message.getKeys();
        object[] params_object = new object [(keys.Count-1)/2];
        Type[] type_array = new Type[(keys.Count - 1) / 2];
        Type type = typeof(Actor);
        MethodInfo info = type.GetMethod(message.get("function_name"));
  

        for(int i = 0; i < keys.Count; i++)
        {
            
            if (keys[i].Length >= 4 && keys[i].Substring(0,4) == "Type")
            {
                int cur_index = int.Parse(keys[i].Substring(4, keys[i].Length - 4));
                type_array[cur_index] = message.getObj<Type>(keys[i]);
                
            }
            else if (keys[i].Length >= 3 && keys[i].Substring(0, 3) == "Int")
            {
                int cur_index = int.Parse(keys[i].Substring(3, keys[i].Length - 3));
                int t = 0;
                type_array[cur_index] = t.GetType();
            }
        }
        for (int i = 0; i < keys.Count; i++)
        {
            if (keys[i].Length >= 5 && keys[i].Substring(0, 5) == "Value")
            {
                int cur_index = int.Parse(keys[i].Substring(5, keys[i].Length - 5));
                //Type cur_type = getTypeByName(type_array[cur_index]);//下面可能有隐含类型转换
                params_object[cur_index] = message.getObject(type_array[cur_index],message.get(keys[i]));
            }
            else if (keys[i].Length >= 7 && keys[i].Substring(0, 7) == "NoEncryInt")
            {
                int cur_index = int.Parse(keys[i].Substring(10, keys[i].Length - 10));
                params_object[cur_index] = (object)int.Parse(message.get(keys[i]));
            }
        }
        Debug.Log("Suscessfully make params_object");//成功运行
        Loom.QueueOnMainThread(() =>
        {
            info.Invoke(Actor.Instance, params_object);
        });
    }

    private Type getTypeByName(string name)
    {
        foreach(Type i in typeof(Receiver).Assembly.GetTypes())
        {
            Debug.Log(i);
            if (i.Name.Equals(name))
            {
                return i;
            }
        }
        Debug.Log("Wrong in getTypeByName");
        return null;
    }
}