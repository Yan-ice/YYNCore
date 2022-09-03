using UnityEngine;
using UnityEngine.UI;

class SpeedController : MonoBehaviour
{
    int speed = 1;
    public void Start()
    {
        MonoUpdateManager.Instance.setSpeed(1);
        GetComponentInChildren<Text>().text = "1x";
    }
    public void switchSpeed()
    {
        speed++;
        if (speed == 4)
        {
            speed = 1;
        }
        MonoUpdateManager.Instance.setSpeed(speed);
        GetComponentInChildren<Text>().text = $"{speed}x";

    }
}