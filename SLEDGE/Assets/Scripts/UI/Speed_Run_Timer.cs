using System;
using TMPro;
using UnityEngine;

public class Speed_Run_Timer : MonoBehaviour
{
    private TMP_Text timer_text;

    private float timer_time;

    public bool timer_running;

    // Start is called before the first frame update
    private void Awake()
    {
        timer_text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (!timer_running) return;

        timer_time += Time.deltaTime;
        TimeSpan time = TimeSpan.FromSeconds(timer_time);

        timer_text.text = time.ToString(format:@"mm\:ss\.ff");

        // Updating every second seems bad lol
        //ScoreManager.Instance.CurrentTime = timer_time;
    }

    public string GetTimeString()
    {
        TimeSpan time = TimeSpan.FromSeconds(timer_time);

        return time.ToString(format: @"mm\:ss\.ff");
    }

    public float GetTimeFloat()
    {
        return timer_time;
    }

    public void StartTimer()
    {
        timer_running = true;
    }

    public void ResetTimer()
    {
        timer_time = 0f;
    }

    public void PauseTimer()
    {
        timer_running = false;
    }
}
