using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Box<T>
{
    public T value { get; set; }

    public Box(T v){
        value = v;
    }
}
