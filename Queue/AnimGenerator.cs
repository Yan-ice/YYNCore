using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AnimGenerator
{
    public static IEnumerable<int> simpleAnimation(Func<int, bool> action_everyFrame, int totalTime)
    {
        for (int a = 1; a <= totalTime; a++)
        {
            if (!action_everyFrame.Invoke(a))
                yield break;
            yield return 0;
        }
    }

    public static IEnumerable<int> valueChangeAnimation<T>(LinkBox<T> value, T delta, Func<bool> stop_condition = null)
    {
        if (typeof(T).IsPrimitive)
        {
            bool stop = false;
            while (!stop && !value.isDesroyed())
            {
                value.value = NumUtil.NumAdd<T>(value.value, delta);

                if (stop_condition != null)
                {
                    stop = stop_condition.Invoke();
                }
                yield return 0;
            }
        }
        else
        {
            MethodInfo oper = typeof(T).GetMethod("op_Addition");

            if (oper == null)
            {
                Debug.LogError("类型" + typeof(T).Name + "不支持加法运算！");
            }

            bool stop = false;
            while (!stop && !value.isDesroyed())
            {
                value.value = (T)oper.Invoke(value.value, new object[] { value.value, delta });

                if (stop_condition != null)
                {
                    stop = stop_condition.Invoke();
                }
                yield return 0;
            }
        }


    }

    public static IEnumerable<int> waitUntilAnimation(BoxInterface<bool> value)
    {
        while (!value.value)
        {
            yield return 0;
        }
    }
}
