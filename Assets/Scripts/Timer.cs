using System.Collections;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private TMP_Text timer;
    private WaitForSeconds oneSec = new WaitForSeconds(1);
    float currentTime = 0;
    [SerializeField] bool isRunning = false;

    private void Start()
    {
        timer = GetComponent<TMP_Text>();
        StartStopwatch();
    }

    public void StartStopwatch()
    {
        if (!this.isRunning)
        {
            this.isRunning = true;
        }
    }

    public void Update()
    {
        if(isRunning)
        {
            currentTime += Time.deltaTime;
            timer.text = $"{currentTime / 60:00}:{currentTime % 60:00}";
        }
    }

    public void StopStopwatch()
    {
        Debug.Log("stop" + isRunning);
        if (this.isRunning)
        {
            this.isRunning = false;
        }
    }

    public void ResetTime()
    {
        if (timer == null) return;
        currentTime = 0;
        timer.text = $"{(int)currentTime / 60:00}:{(int)currentTime % 60:00}";
    }
}
