using System.Collections.Generic;

public class Message
{
    struct Entry
    {
        public string K;
        public string V;

        public Entry(string k, string v)
        {
            K = k;
            V = v;
        }
        public Entry(string total)
        {
            string[] sl = total.Split(":".ToCharArray());
            K = sl[0];
            V = sl[1];
        }

        public void setValue(string value)
        {
            V = value;
        }

        public override string ToString()
        {
            return K + ":" + V;
        }
        public override bool Equals(object obj)
        {
            Entry e = (Entry)obj;
            return e.K == K && e.V == V;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        
        public Entry clone()
        {
            return new Entry(K, V);
        }
    }


    private string identity = "default";
    private List<Entry> m_entryList = new List<Entry>();
    private bool isP1 = true;
    private bool setted = false;
    /// <summary>
    /// 设置Message的ID。
    /// 
    /// </summary>
    /// <param name="id"></param>
    public void setIdentity(string id)
    {
        identity = id;
    }

    /// <summary>
    /// 为Message添加一对数据(K,V)。
    /// </summary>
    /// <param name="k"></param>
    /// <param name="v"></param>
    public void add(string k, string v)
    {

        foreach (Entry e in m_entryList)
        {
            if (e.K.Equals(k))
            {
                e.setValue(v);
                return;
            }
        }

        if(k.Equals("isRed") || k.Equals("IsRed")){
            if(v.Equals("True") || v.Equals("true"))
            {
                isP1 = true;
            }
            else
            {
                isP1 = false;
            }
        }

        m_entryList.Add(new Entry(k, v));
    }

    /// <summary>
    /// 通过给定的字符串K获取对应字符串V。
    /// 如果不存在对应字符串V，则返回null。
    /// </summary>
    /// <param name="k"></param>
    /// <returns></returns>
    public string get(string k)
    {
        foreach (Entry e in m_entryList)
        {
            if (e.K.Equals(k))
            {
                return e.V;
            }
        }
        return null;
    }



    /// <summary>
    /// 通过序列化的字符串来获得一个Message对象，
    /// 或者初始化一个Identity。
    /// </summary>
    /// <param name="message"></param>
    public Message(string message)
    {
        if (message.EndsWith("$"))
        {
            message = message.Split('$')[0];
        }
        if (message.Contains(":"))
        {
            string[] fir = message.Split("|".ToCharArray());
            identity = fir[0];
            string[] sec = fir[1].Split(",".ToCharArray());

            foreach (string en in sec)
            {
                if (en != null && en.Contains(":"))
                {
                    string[] thi = en.Split(":".ToCharArray());
                    add(thi[0], thi[1]);
                }

            }
        }
        else
        {
            identity = message;
        }
        if (get("isP1") != null)
        {
            setIsP1(get("isP1").Equals("True"));
        }
    }

    /// <summary>
    /// 复制一个新的Message。
    /// </summary>
    public Message clone()
    {

        Message ms = new Message(identity);

        foreach (Entry e in m_entryList)
        {
            ms.m_entryList.Add(e.clone());
        }
        if (whetherP1Setted())
        {
            ms.setIsP1(isP1);
        }
        return ms;
    }


    /// <summary>
    /// 序列化Message。
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        string total = identity + "|";
        bool fir = true;
        foreach (Entry en in m_entryList)
        {
            if (fir)
            {
                fir = false;
                total = total + en.ToString();
            }
            else
            {
                total = total + "," + en.ToString();
            }
        }
        
        return total + "$";
    }

    /// <summary>
    /// 获取Message的id。
    /// </summary>
    /// <returns>id</returns>
    public string getIdentity()
    {
        return identity;
    }

    public bool whetherP1Setted()
    {
        return setted;
    }

    /// <summary>
    /// 设置指令的针对对象为哪个玩家。
    /// </summary>
    /// <param name="isP1">是P1吗</param>
    public void setIsP1(bool isP1)
    {
        this.isP1 = isP1;
        setted = true;
    }

    /// <summary>
    /// 查看指令的针对对象。
    /// </summary>
    /// <returns></returns>
    public bool getIsP1()
    {
        return isP1;
    }

    public bool Contains(Message ms)
    {
        if (ms.identity != identity)
        {
            return false;
        }
        foreach(Entry k in ms.m_entryList)
        {
            if (!this.m_entryList.Contains(k))
            {
                return false;
            }
        }
        return true;
    }
}
