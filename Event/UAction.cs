using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit)]
public struct UAction
{
    [FieldOffset(0)] private byte paramCount;
    [FieldOffset(8)] public Action action0;
    [FieldOffset(8)] public Action<int> action1;
    [FieldOffset(8)] public Action<int, int> action2;
    [FieldOffset(8)] public Action<int, int, int> action3;

    public void Invoke(int p1 = 0, int p2 = 0, int p3 = 0)
    {
        switch (paramCount)
        {
            case 1:
                action0.Invoke();
                break;
            case 2:
                action1.Invoke(p1);
                break;
            case 3:
                action2.Invoke(p1,p2);
                break;
            case 4:
                action3.Invoke(p1,p2,p3);
                break;
            default:
                Log.Message("Warning: Empty UAction.");
                break;
        }
    }
    public UAction(Action act)
    {
        paramCount = 1;

        action1 = null;
        action2 = null;
        action3 = null;
        action0 = act;
    }
    public UAction(Action<int> act)
    {
        paramCount = 2;
        action0 = null;

        action2 = null;
        action3 = null;
        action1 = act;
    }
    public UAction(Action<int, int> act)
    {
        paramCount = 3;
        action0 = null;
        action1 = null;

        action3 = null;
        action2 = act;
    }
    public UAction(Action<int, int, int> act)
    {
        paramCount = 4;
        action0 = null;
        action1 = null;
        action2 = null;
        action3 = act;
    }
}
