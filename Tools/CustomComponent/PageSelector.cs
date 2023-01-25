using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PageSelector : MonoBehaviour
{
    public string[] candidate;
    public UnityEvent<string> onSelectionChange;

    private int index = 0;
    private Text txt;

    // Start is called before the first frame update
    void Start()
    {
        txt = GetComponentInChildren<Text>();
        txt.text = candidate[index];
    }
    void Update()
    {
        if (txt != null)
            txt.text = candidate[index];
    }
    public int getIndex() {
        return index;
    }
    public void setIndex(int i)
    {
        index = (i + candidate.Length) % candidate.Length;
        if(txt!=null)
            txt.text = candidate[index];
    }
    
    
    public void left()
    {
        index--;
        if (index < 0) index += candidate.Length;
        txt.text = candidate[index];
        onSelectionChange.Invoke(candidate[index]);
    }
    public void right()
    {
        index++;
        if (index >= candidate.Length) index -= candidate.Length;
        txt.text = candidate[index];
        onSelectionChange.Invoke(candidate[index]);
    }
}
