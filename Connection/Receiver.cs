using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Receiver: Singleton<Receiver>
{
    public static bool LOCAL = true;

    ClientSocket client_socket;
    public Receiver()
    {
        client_socket = ClientSocket.Instance;
    }
    private void invoke(string name, params object[] list)
    {
        Message ms = new Message("p1");
        //put...
        ms.add("function_name", name);
        for (int i = 0; i < list.Length; i++)
        {
            if ((i % 2) == 0)
            {
                //ms.add("Type" + (i/2).ToString(), Convert.ToString(list[i]));
                ms.addObj<Type>("Type" + (i / 2).ToString(), (Type)list[i]);
            }
            else
            {
                ms.addObject("Value" + (i / 2).ToString(), (Type)list[i - 1], list[i]); //第二个参数是把type传进去
            }

        }
        client_socket.send(ms);
        //send sth
    }

    public void UseCard(CardData data, LocInFightArea l1, LocInFightArea l2)
    {
        int id_data = data.uid;
        if (LOCAL)
        {
            Actor.Instance.UseCard(id_data, l1, l2);
        }
        else
        {
            invoke((string)System.Reflection.MethodBase.GetCurrentMethod().Name, id_data.GetType(), id_data, l1.GetType(), l1, l2.GetType(), l2);

        }

    }
    public bool GetWhetherConnectSuccess()
    {
        return client_socket.whether_connectting;
    }
}


public class ParamTest
{
    public int i;
    public float b;
    public ParamTest(int i, float b)
    {
        this.i = i;
        this.b = b;
    }   
}

public class ParamTest2
{
    public ParamTest ts;
    public double b;
    public ParamTest2(ParamTest ts, double b)
    {
        this.ts = ts;
        this.b = b;
    }
}