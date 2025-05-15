using UnityEngine;
using UnityEngine.Rendering;

public class StageTime : MonoBehaviour
{
    public float time;
    TimerUI timerUI;
    private void Awake()
    {
        timerUI = FindAnyObjectByType<TimerUI>();
    }
    private void Update()
    {
        time += Time.deltaTime;
        timerUI.UpdateTime(time);
    }
}
