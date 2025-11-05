using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private int MaxTimeInSeconds;
    [SerializeField] private TMP_Text TimerText;
    [SerializeField] private Slider TimerSlider;

    [Header("Audio")]
    [SerializeField] private AudioMixer MainMixer; // assign your AudioMixer here
    [SerializeField] private AudioSource AudioSource; // for normal beeps
    [SerializeField] private AudioSource FinalAudioSource;
    [SerializeField] private AudioSource FinalAudioSource2;// for final 30s clip
    [SerializeField] private AudioClip NormalBeepClip;
    [SerializeField] private AudioClip FinalCountdownClip;
    [SerializeField] private AudioClip FinalCountdownClip2;

    [Header("Visuals")]
    [SerializeField] private Image BackgroundTerminalImage;
    [SerializeField] private Color BlinkColor = Color.red;
    [SerializeField] private float BlinkDuration = 0.1f;
    [SerializeField] private int BlinkCount = 3;

    private bool TimerIsActive = false;
    private float CurrentTime;
    private Color _originalColor;
    private int lastBeepSecond = -1;
    private bool finalPhaseStarted = false;
    private bool fadeStarted = false;

    void Start()
    {
        CurrentTime = MaxTimeInSeconds;
        TimerSlider.maxValue = MaxTimeInSeconds;
        TimerSlider.value = MaxTimeInSeconds;
        DisplayTime();

        if (BackgroundTerminalImage != null)
            _originalColor = BackgroundTerminalImage.color;

        if (MainMixer != null)
            MainMixer.SetFloat("Volume", 0f); // reset at start
    }

    void Update()
    {
        if (TimerIsActive)
        {
            TimerUpdate();
            TimerSlider.value = CurrentTime;
        }
    }

    public void StartTimer()
    {
        TimerIsActive = true;
        lastBeepSecond = Mathf.FloorToInt(CurrentTime);
    }

    private void TimerUpdate()
    {
        if (CurrentTime > 0f)
        {
            CurrentTime -= Time.deltaTime;
            if (CurrentTime < 0f) CurrentTime = 0f;

            DisplayTime();
            HandleBeepAndBlink();
            HandleFinalCountdown();
            HandleFadeOut();
        }
        else TimerEnd();
    }

    private void HandleBeepAndBlink()
    {
        int currentSec = Mathf.FloorToInt(CurrentTime);

        if (finalPhaseStarted)
        {
            // Below 30s: blink every 5 seconds
            if (currentSec != lastBeepSecond && currentSec % 5 == 0)
            {
                StartCoroutine(BlinkImageMultiple());
                lastBeepSecond = currentSec;
            }
            return;
        }

        // Normal phase: beep every minute
        if (currentSec != lastBeepSecond && currentSec % 60 == 0)
        {
            BeepAndBlink();
            lastBeepSecond = currentSec;
        }
    }

    private void HandleFinalCountdown()
    {
        if (!finalPhaseStarted && CurrentTime <= 30f)
        {
            finalPhaseStarted = true;
            StartCoroutine(StartFinalCountdownAudio());
        }
    }

    private void HandleFadeOut()
    {
        if (!fadeStarted && CurrentTime <= 15f)
        {
            fadeStarted = true;
            StartCoroutine(FadeOutAllAudioExceptFinal(15f));
        }
    }

    private void BeepAndBlink()
    {
        if (AudioSource != null && NormalBeepClip != null)
            AudioSource.PlayOneShot(NormalBeepClip);

        if (BackgroundTerminalImage != null)
            StartCoroutine(BlinkImageMultiple());
    }

    private IEnumerator BlinkImageMultiple()
    {
        for (int i = 0; i < BlinkCount; i++)
        {
            BackgroundTerminalImage.color = BlinkColor;
            yield return new WaitForSeconds(BlinkDuration);
            BackgroundTerminalImage.color = _originalColor;
            yield return new WaitForSeconds(BlinkDuration);
        }
    }

    private IEnumerator StartFinalCountdownAudio()
    {
        if (FinalAudioSource != null && FinalCountdownClip != null)
        {
            FinalAudioSource.clip = FinalCountdownClip;
            FinalAudioSource.volume = 1f;
            FinalAudioSource.Play();
            FinalAudioSource2.clip = FinalCountdownClip2;
            FinalAudioSource2.volume = 1f;
            FinalAudioSource2.Play();
            
        }
        yield return null;
    }

    private IEnumerator FadeOutAllAudioExceptFinal(float duration)
    {
        if (MainMixer == null)
        {
            Debug.LogWarning("No AudioMixer assigned for fading.");
            yield break;
        }

        float startVolume = 0f; // 0dB in Unity mixer
        float endVolume = -80f; // effectively silent
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newVolume = Mathf.Lerp(startVolume, endVolume, elapsed / duration);
            MainMixer.SetFloat("Volume", newVolume);
            yield return null;
        }

        MainMixer.SetFloat("Volume", endVolume);
    }

    private void DisplayTime()
    {
        int minutes = Mathf.FloorToInt(CurrentTime / 60);
        int seconds = Mathf.FloorToInt(CurrentTime % 60);
        TimerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void TimerEnd()
    {
        TimerIsActive = false;
        // handle game over
    }
}
