using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private int MaxTimeInSeconds;
    [SerializeField] private TMP_Text TimerText;
    [SerializeField] private Slider TimerSlider;

    private bool TimerIsActive = false;
    private float CurrentTime;

    void Start()
    {
        CurrentTime = MaxTimeInSeconds;
        TimerSlider.maxValue = MaxTimeInSeconds;
        TimerSlider.value = MaxTimeInSeconds;
        DisplayTime();
    }

    void Update()
    {
        if (TimerIsActive)
        {
            TimerUpdate();
            TimerSlider.value = CurrentTime;
        } 
    }

    public void AddTime(float addedTime)
    {
        CurrentTime += addedTime;
    }

    public void RemoveTime(float removedTime)
    {
        CurrentTime -= removedTime;
    }

    public void StartTimer()
    {
        TimerIsActive = true;
    }

    public void PauseTimer()
    {
        TimerIsActive = false;
    }

    private void TimerUpdate()
    {
        if (CurrentTime > 0)
        {
            CurrentTime -= Time.deltaTime;
            if (CurrentTime < 0) CurrentTime = 0;
            DisplayTime();
        }
        else
        {
            TimerEnd();
        }
    }

    private void DisplayTime()
    {
        int minutes = Mathf.FloorToInt(CurrentTime / 60);
        int seconds = Mathf.FloorToInt(CurrentTime % 60);
        TimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void TimerEnd()
    {
        //handle game over
    }
}