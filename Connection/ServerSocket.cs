#define UnityEngine

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ServerSocket : Singleton<ServerSocket>
{

    Socket m_serverSocket;
    Thread m_listener;
    List<SocketListener> clients = new List<SocketListener>();

    public static int port = 25566;

 //   public static void Main(string[] args)
 //   {
 //       Console.WriteLine("Running Server");
 //
 //       ServerSocket.Init();
 //       ServerSocket.Instance.OpenServer();
 //       while (true)
 //       {
 //           Console.Read();
 //       }
 //   }

    public ServerSocket()
    {
        
        foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
            netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            {
                foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses)
                {
                    if (addrInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        var ipAddress = addrInfo.Address;
                        Log.Message("本机IP: " + ipAddress.ToString());
                        return;
                    }
                }
            }
        }
    }

    List<MessageListener> listenerList = new List<MessageListener>();
    public void registerMessageListener(MessageListener listener)
    {
        lock (listenerList)
        {
            listenerList.Add(listener);
        }

    }
    public void unregisterMessageListener(MessageListener listener)
    {
        lock (listenerList)
        {
            listenerList.Remove(listener);
        }
    }


    public void onRecieve(Socket socket, Message message)
    {
        foreach (MessageListener listener in listenerList)
        {
            if (listener.Identity().Equals(message.getIdentity()))
            {
                listener.onMessageReceived(socket, message);
            }
        }
    }

    bool closed = false;
    public void close()
    {
        foreach(SocketListener l in clients)
        {
            l.close();
        }
        closed = true;
        if (m_serverSocket != null)
        {
            m_serverSocket.Close();
            m_serverSocket = null;
            m_listener = null;
        }
       
    }

    public void OpenServer()
    {
        if (m_listener != null)
        {
            Log.Message("open server twice.");
        }
        closed = false;
        m_listener = new Thread(ListenConnectSocket);
        m_listener.IsBackground = true; //关闭后天线程
        m_listener.Start();
    }


    private List<Socket> players = new List<Socket>();

    void ListenConnectSocket()
    {
        m_serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //第一个参数为寻找地址的方式,此时选定为IPV4的地址; 第二个参数为数据传输的方式，此时选择的是Stream传输(能够准确无误的将数据传输到)；第三个参数为执行的协议，此时选择的是TCP协议；

        IPAddress address = IPAddress.Any;
        IPEndPoint endPoint = new IPEndPoint(address, port);
        try
        {
            m_serverSocket.Bind(endPoint);
            Log.Message("Server Bind Success");
            port++;

        }
        catch (Exception)
        {
            Log.Message("ServerConnection Failed");
            return;
        }
        while (true)
        {
            try
            {

                m_serverSocket.Listen(1);
                Socket s = m_serverSocket.Accept();
                Log.Message(System.DateTime.Now.ToLongTimeString()+" Identifying connection: " +s.RemoteEndPoint.ToString()+" (local "+s.LocalEndPoint.ToString()+")");
                new SocketListener(s);
            }
            catch (Exception)
            {
                if (closed)
                {
                    return;
                }
                Thread.Sleep(1000);
            }
        }
    }
}

class SocketListener
{
    Socket socket;
    string buffer;

    bool closed = false;
    public SocketListener(Socket socket)
    {
        this.socket = socket;
        new Thread(listen).Start();
    }
    public void send(Message mes)
    {
        Encryptor.SendEncryptMessage(socket, mes.ToString()+"$");
    }
    
    public void close()
    {
        closed = true;
    }

    private void listen()
    {
        while (!closed)
        {
            byte[] bytes = new byte[100000];
            //identifying client version
            socket.Receive(bytes);//接收TCP信息
            string p = Encoding.UTF8.GetString(bytes);
            buffer = buffer.TrimEnd('\0') + p;//接收处

            if (!buffer.Contains("$"))
            {
                continue;
            }

            string mesStr = "";
            try
            {
                string[] msgs = buffer.Split('$');
                for (int a = 0; a < msgs.Length - 1; a++)
                {
                    mesStr = Encryptor.DESDecrypt(msgs[a]);//解密
                    if (mesStr.Contains(":"))
                    {
                        Message message = new Message(mesStr.Trim());
                        ServerSocket.Instance.onRecieve(socket,message);
                    }
                }
                buffer = msgs[msgs.Length - 1];
            }
            catch (Exception e)
            {
                Log.Error("server caught Error from message " + mesStr + ": " + e.Message + " " + e.StackTrace);
            }

            //Log.Message("versionInfo:" + p);
            Message m = new Message(Encryptor.DESDecrypt(p) + "$");
        }
    }
}
