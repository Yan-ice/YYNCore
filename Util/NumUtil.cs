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
        if (current == null)
        {
            current = new Random(DateTime.Now.Millisecond);
        }
        return (float)current.NextDouble() * max;
    }
    public static float RandFloat(float min, float max)
    {
        return min + RandFloat(max-min);
    }

    /// <summary>
    /// 开启一个新的随机数状态
    /// </summary>
    /// <param name="seed"></param>
    public static void NewState(int seed)
    {
        if (current != null)
        {
            rand_states.Push(current);
        }
        current = new Random(seed);
    }

    /// <summary>
    /// 开启一个随机的随机数状态
    /// </summary>
    public static void NewState()
    {
        NewState(DateTime.Now.Millisecond+DateTime.Now.DayOfYear*1000);
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

    //泛型的数字加法。
    public static T NumAdd<T>(T num1, T num2)
    {
        if (typeof(T) == typeof(int))
        {
            int a = (int)Convert.ChangeType(num1, typeof(int));
            int b = (int)Convert.ChangeType(num2, typeof(int));

            int c = a + b;
            return (T)Convert.ChangeType(c, typeof(T));
        }
        else if (typeof(T) == typeof(float))
        {
            float a = (float)Convert.ChangeType(num1, typeof(float));
            float b = (float)Convert.ChangeType(num2, typeof(float));

            float c = a + b;
            return (T)Convert.ChangeType(c, typeof(T));
        }
        else if (typeof(T) == typeof(double))
        {
            double a = (double)Convert.ChangeType(num1, typeof(double));
            double b = (double)Convert.ChangeType(num2, typeof(double));

            double c = a + b;
            return (T)Convert.ChangeType(c, typeof(T));
        }
        else if (typeof(T) == typeof(decimal))
        {
            decimal a = (decimal)Convert.ChangeType(num1, typeof(decimal));
            decimal b = (decimal)Convert.ChangeType(num2, typeof(decimal));

            decimal c = a + b;
            return (T)Convert.ChangeType(c, typeof(T));
        }

        return default(T);
    }

}