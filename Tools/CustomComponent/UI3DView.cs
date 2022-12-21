using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI3DView : MonoBehaviour
{
    public static int speed = 1;

    private Vector3 originPos;
    public int distance = 0;
    public int standard_distance = 10;
    // Start is called before the first frame update
    void Start()
    {
        originPos = transform.position;
        float scale = standard_distance / (float)distance;
        transform.localScale = new Vector3(scale, scale, scale);
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.mousePosition.x / Screen.width - 0.5f;
        float y = Input.mousePosition.y / Screen.height - 0.5f;
        transform.position = originPos + new Vector3(x, y, 0) / distance * standard_distance * speed;
    }
}
