using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoFitBoarder : MonoBehaviour
{
    private Text cp_text;
    private Text or_text;

    // Start is called before the first frame update
    void Start()
    {
        or_text = GetComponent<Text>();
        cp_text = transform.Find("m_copyText").GetComponent<Text>();
        cp_text.text = or_text.text;
    }

    // Update is called once per frame
    void Update()
    {
        cp_text.text = or_text.text;
    }

}
