using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float miliSecond;
    public int second;
    public int minute;
    public GameObject Pause;
    public TMP_Text text;

    public int Second { get => second; set => second = value; }

    private void FixedUpdate() // вызывается 50 раз в секунду
    {
        if (Pause.activeSelf == false)
        {
            miliSecond += 0.02f;

            if (miliSecond >= 1)
            {
                Second++;
                miliSecond = 0;
            }

            if (Second >= 60)
            {
                minute++;
                Second = 0;
            }

            if (minute < 10)
            {
                if (Second < 10)
                    text.text = $"0{minute} : 0{Second}";
                else
                    text.text = $"0{minute} : {Second}";
            }
            else
            {
                if (Second < 10)
                    text.text = $"{minute} : 0{Second}";
                else
                    text.text = $"{minute} : {Second}";
            }
        }
    }
}
