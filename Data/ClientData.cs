
using System;

public class ClientData
{
    static int clientID = 0;
    public static int GetClientID()
    {
        if (clientID == 0)
        {
            clientID = GameDatabase.base_data.GetData("clientID");
            if (clientID == 0)
            {
                System.Random ran = new System.Random(DateTime.Now.Second * 1000 + DateTime.Now.Millisecond);
                clientID = ran.Next(100000, 999999);
                GameDatabase.base_data.SetData("clientID", clientID);
            }
        }
        return clientID;
    }
    public static void ResetClientID()
    {
        GameDatabase.base_data.SetData("clientID", 0);
        GetClientID();
    }
}
