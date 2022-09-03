using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AutoFitText : MonoBehaviour
{
    private Text t;
    private string txt;

    public string text {
        get {
            return txt;
        }
        set
        {
            setText(value);
        }
    }

    // Use this for initialization
    int min, max;
    void Awake()
    {
        t = GetComponent<Text>();
        min = t.resizeTextMinSize;
        max = t.resizeTextMaxSize;
        t.resizeTextForBestFit = false;
        t.verticalOverflow = VerticalWrapMode.Truncate;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        setText(t.text);
    }
    
    private void Update()
    {
        if (t.text != txt)
        {
            setText(t.text);
        }
    }

    void setText(string s)
    {
        txt = s;
        t.text = s;
        int fontSize = max;
        t.fontSize = fontSize;

        while (fontSize>min)
        {
            var generator = new TextGenerator();
            var rectTransform = GetComponent<RectTransform>();
            var settings = t.GetGenerationSettings(rectTransform.rect.size);
            generator.Populate(s, settings);

            // trncate visible value and add ellipsis
            var characterCountVisible = generator.characterCountVisible;
            if (s.Length <= characterCountVisible)
            {
                break;
            }
            else
            {
                fontSize -= 3;
                t.fontSize = fontSize;
            }
        }
        

    }

}