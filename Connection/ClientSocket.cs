using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using UnityEngine.Events;

public class ClientSocket : Singleton<ClientSocket>
{
    public string IP;
    public int port;
    Socket m_clientSocket;

    Thread thr;
    public UnityEvent connectSuccess = new UnityEvent();
    public UnityEvent connectFailed = new UnityEvent();

    public ClientSocket()
    {

        thr = new Thread(connectServer);
        thr.IsBackground = true;
        thr.Start();
    }

    public void ConnectTo(string IP,int port)
    {
        this.IP = IP;
        this.port = port;
        //第一个参数为寻找地址的方式,此时选定为IPV4的地址; 第二个参数为数据传输的方式，此时选择的是Stream传输(能够准确无误的将数据传输到)；第三个参数为执行的协议，此时选择的是TCP协议；
        if (connecter != null)
        {
            return;
        }
        try
        {
            connecter = new Thread(socketConnect);
            connecter.Start();
        }
        catch (Exception e)
        {
            Log.Error("error in connecting: " + e.Message + " " + e.StackTrace);
            connecter = null;
            Loom.QueueOnMainThread(connectFailed.Invoke);
        }
    }

    Thread connecter = null;

    private void socketConnect()
    {
        try
        {
            if (m_clientSocket != null)
            {
                close();
            }
            isClosed = false;
            m_clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_clientSocket.ReceiveTimeout = 5000;
            Log.Message("connecting server: "+IP);
            IPAddress address = IPAddress.Parse(IP);
            IPEndPoint endPoint = new IPEndPoint(address, port);
            m_clientSocket.Connect(endPoint);
        }
        catch (Exception e)
        {
            m_clientSocket.Close();
            m_clientSocket = null;
            connecter = null;
            isClosed = true;
            Log.Message("Connection Failed:" +e.Message);
            Loom.QueueOnMainThread(connectFailed.Invoke);
            return;
        }
        isClosed = false;
        connecter = null;
        Loom.QueueOnMainThread(connectSuccess.Invoke);
        Log.Message("connection success.");
    }
    void connectServer()
    {
        //SceneManager.LoadScene("Game", LoadSceneMode.Single);

        string buffer = "";
        while (true)
        {
            
            if (isClosed || m_clientSocket==null)
            {
                Thread.Sleep(1000);
                continue;
            }
           
            byte[] bytes = new byte[50000];
            
            try
            {
                m_clientSocket.ReceiveTimeout = 8000;
                m_clientSocket.Receive(bytes);
                
                buffer = buffer.TrimEnd('\0')+ Encoding.UTF8.GetString(bytes);//接收TCP信息
            }
            catch (SocketException e)
            {
                Thread.Sleep(500);
                
                continue;
                
            }
            catch (ObjectDisposedException)
            {
                Log.Message("unknown host.");
                isClosed = true;
                m_clientSocket.Close();
            }
       
            Debug.Log(buffer);
            if (!buffer.Contains("$"))
            {
                continue;
            }
            string[] msgs = buffer.Split('$');
            for(int a = 0; a < msgs.Length - 1; a++)
            {
                string mesStr = Encryptor.DESDecrypt(msgs[a]);//解密指令
                try
                {
                    if (mesStr.Contains("|"))
                    {
                        Log.Message("[recieve] " + mesStr);
                        Message message = new Message(mesStr.Trim());
                        foreach (MessageListener listener in getListenerList(message.getIdentity()))
                        {
                            listener.onMessageReceived(m_clientSocket,message);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message + " " + e.StackTrace);
                    Log.Error("解析指令" + mesStr + "出错 - " + e.Message + " - " + e.StackTrace);
                }
            }
            buffer = msgs[msgs.Length - 1];
            Thread.Sleep(200);
        }
    }
    struct list_node
    {
        public string identity;
        public List<MessageListener> list;

        public void free()
        {
            list.Clear();
        }
    }
    List<list_node> m_listenerList = new List<list_node>();

    public void registerMessageListener(MessageListener listener)
    {
        foreach(list_node nd in m_listenerList)
        {
            if(nd.identity == listener.Identity())
            {
                nd.list.Add(listener);
                return;
            }
        }
        list_node new_nd = new list_node();
        new_nd.identity = listener.Identity();
        new_nd.list = new List<MessageListener>();
        new_nd.list.Add(listener);

        m_listenerList.Add(new_nd);
    }
    public void unregisterMessageListener(MessageListener listener)
    {
        foreach (list_node nd in m_listenerList)
        {
            if (nd.identity == listener.Identity())
            {
                nd.list.Remove(listener);
                return;
            }
        }
    }

    public List<MessageListener> getListenerList(string id)
    {
        List<MessageListener> l = new List<MessageListener>();
        foreach (list_node nd in m_listenerList)
        {
            if (nd.identity == id)
            {
                foreach (MessageListener ls in nd.list)
                {
                    l.Add(ls);
                }
                break;
            }
        }

        return l;
    }


    public void send(Message mes)
    {
        if (isClosed)
        {
            return;
        }
        //Debug.Log("[send] " + mes.ToString());
        Encryptor.SendEncryptMessage(m_clientSocket,mes.ToString());
    }

    bool isClosed = false;
    public void close()
    {
        if (connecter != null)
        {
            connecter.Interrupt();
            connecter.Abort();
            connecter = null;
        }
        if (m_clientSocket != null)
        {
            Log.Message("client socket closed by user.");
            try
            {
                Message m = new Message("Controller");
                m.add("type", "close");
                send(m);
                isClosed = true;
                m_clientSocket.Close();
            }
            catch (Exception)
            {

            }
            m_clientSocket = null;
        }
    }
    public void close_by_server()
    {
        if (m_clientSocket != null)
        {
            Log.Message("client socket closed by server.");
            try
            {
                isClosed = true;
                m_clientSocket.Close();
            }
            catch (Exception)
            {

            }
            m_clientSocket = null;
        }
    }

}
