using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float miliSecond;
    public float second;
    public float minute;
    public GameObject Pause;
    public TMP_Text text;

    private void FixedUpdate() // вызывается 50 раз в секунду
    {
        if (Pause.activeSelf == false)
        {
            miliSecond += 0.02f;

            if (miliSecond >= 1)
            {
                second++;
                miliSecond = 0;
            }

            if (second >= 60)
            {
                minute++;
                second = 0;
            }

            if (minute < 10)
            {
                if (second < 10)
                    text.text = $"0{minute} : 0{second}";
                else
                    text.text = $"0{minute} : {second}";
            }
            else
            {
                if (second < 10)
                    text.text = $"{minute} : 0{second}";
                else
                    text.text = $"{minute} : {second}";
            }
        }
        

    }
}
