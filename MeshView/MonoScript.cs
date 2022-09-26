using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class MonoScript<T> where T : MonoBehaviour
{
    public T m_monoObject { get
        {
            return MeshManager.Instance.GetLinking(this);
        } }

    public void Destroy()
    {
        OnDestroy();
        MeshManager.Instance.Unlink(this);
    }
    protected abstract void OnDestroy();
}