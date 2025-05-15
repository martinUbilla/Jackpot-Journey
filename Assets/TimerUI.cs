using UnityEngine;
using TMPro;
public class TimerUI : MonoBehaviour
{

    TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateTime(float time)
    {
        int minutes = (int)(time / 60f);
        int seconds = (int)(time % 60f);

        text.text = minutes.ToString() + ":" + seconds.ToString("00");
    }
}
