
using UnityEngine;
using UnityEngine.UI;

public class TstCaller : MonoBehaviour
{
    public void onClick()
    {
        UIManager.Instance.ShowWindow<TstConsole>();
    }
}
