using System;
using System.Net.Sockets;

public interface MessageListener
{
    /// <summary>
    /// 给予一个ID，该监听器将只会接受指定ID的消息。
    /// 如果ID为null，该监听器将会接受任何消息。
    /// </summary>
    /// <returns>指定的ID</returns>
    string Identity();

    /// <summary>
    /// 在接收到消息时会调用本方法。
    /// </summary>
    void onMessageReceived(Socket from, Message message);

}

class ASimpleListener : MessageListener
{
    public string Identity()
    {
        return "Controller";
    }

    public void onMessageReceived(Socket from, Message message)
    {
        ClientSocket.Instance.registerMessageListener(this);
    }
}
