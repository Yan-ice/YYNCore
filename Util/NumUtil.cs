using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class NumUtil
{

    //以下是随机数生成部分。
    static Random current;
    static Stack<Random> rand_states = new Stack<Random>();

    public static int RandInt(int min, int max)
    {
        if (min == max) { return min; }
        if(current == null)
        {
            current = new Random(DateTime.Now.Millisecond);
        }
        return current.Next() % (max - min) + min;
    }

    public static int RandInt(int max)
    {
        return RandInt(0, max);
    }
    public static float RandFloat(float max)
    {
        return (float)current.NextDouble() * max;
    }
    public static float RandFloat(float min, float max)
    {
        return min + (float)current.NextDouble() * (max-min);
    }

    public static double Random()
    {
        double d = current.NextDouble();
        return d - (int)d;
    }
    
    /// <summary>
    /// 开启一个新的随机数状态
    /// </summary>
    /// <param name="seed"></param>
    public static void NewState(int seed)
    {
        rand_states.Push(current);
        current = new Random(seed);
    }

    /// <summary>
    /// （放弃当前随机数状态并)回到上一个随机数状态。
    /// </summary>
    public static void RestoreState()
    {
        if(rand_states.Count == 0)
        {
            return;
        }
        current = rand_states.Pop();
    }

    //以下是数据处理部分。

    public static float LimitNum(float num, float bottom, float top)
    {
        if (num < bottom) return bottom;
        if (num > top) return top;
        return num;
    }


}