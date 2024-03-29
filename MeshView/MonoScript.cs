﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface MonoScript<T> where T : MonoBehaviour
{
    public T m_monoObject { get; }
}
