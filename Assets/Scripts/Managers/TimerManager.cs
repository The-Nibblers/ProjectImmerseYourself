using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;
using Unity.VisualScripting;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private int MaxTimeInSeconds;
    [SerializeField] private TMP_Text TimerText;
    [SerializeField] private Slider TimerSlider;

    [Header("Audio")]
    [SerializeField] private AudioMixer MainMixer;
    [SerializeField] private AudioSource AudioSource;          
    [SerializeField] private AudioSource FinalAudioSource;     
    [SerializeField] private AudioSource FinalAudioSource2;    
    [SerializeField] private AudioClip NormalBeepClip;
    [SerializeField] private AudioClip FinalCountdownClip;
    [SerializeField] private AudioClip FinalCountdownClip2;
    [SerializeField] private AudioClip DeathClip;              

    [Header("Visuals")]
    [SerializeField] private Image BackgroundTerminalImage;
    [SerializeField] private Color BlinkColor = Color.red;
    [SerializeField] private Color Deathcolor = Color.black;
    [SerializeField] private float BlinkDuration = 0.1f;
    [SerializeField] private int BlinkCount = 3;
    [Header("UI Shake")]
    [SerializeField] private RectTransform[] UIPanels; // 4 canvases/panels
    
    [Header("UI Canvases")]
    [SerializeField] private GameObject BlackCanvas;
    [SerializeField] private GameObject TimerCanvas;
    [SerializeField] private GameObject OrderCanvas;
    [SerializeField] private GameObject DarkImage;
    [SerializeField] private GameObject OverlayCanvas;


    private bool TimerIsActive = false;
    private float CurrentTime;
    private Color _originalColor;
    private bool finalPhaseStarted = false;
    private bool fadeStarted = false;
    private bool timerEnded = false;

    private float nextNormalBeepTime;
    private float nextMediumBeepTime;

    void Start()
    {
        CurrentTime = MaxTimeInSeconds;
        TimerSlider.maxValue = MaxTimeInSeconds;
        TimerSlider.value = MaxTimeInSeconds;
        DisplayTime();

        if (BackgroundTerminalImage != null)
            _originalColor = BackgroundTerminalImage.color;

        if (MainMixer != null)
            MainMixer.SetFloat("Volume", 0f);

        // Initialize next beep times
        nextNormalBeepTime = MaxTimeInSeconds;
        nextMediumBeepTime = 30f;
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
    }

    private void TimerUpdate()
    {
        if (CurrentTime > 0f)
        {
            CurrentTime -= Time.deltaTime;
            if (CurrentTime < 0f) CurrentTime = 0f;

            DisplayTime();
            HandleBeepBlinkShake();
            HandleFinalCountdown();
            HandleFadeOut();
        }
        else if (!timerEnded)
        {
            TimerEnd();
        }
    }

    private void HandleBeepBlinkShake()
    {
        if (!finalPhaseStarted && CurrentTime <= nextNormalBeepTime)
        {
            BeepAndBlink();
            ShakeUIPanels(0.5f, 5f); // small shake
            nextNormalBeepTime -= 60f;
        }

        if (finalPhaseStarted)
        {
            if (CurrentTime <= nextMediumBeepTime)
            {
                StartCoroutine(BlinkImageMultiple());
                ShakeUIPanels(0.5f, 12f); // medium shake
                nextMediumBeepTime -= 10f;
            }

            if (CurrentTime <= MaxTimeInSeconds * 0.1f)
            {
                float intensity = Mathf.Lerp(15f, 25f, 1f - (CurrentTime / (MaxTimeInSeconds * 0.1f)));
                ShakeUIPanels(0.5f, intensity);
            }
        }
    }

    private void ShakeUIPanels(float duration, float intensity)
    {
        if (UIShake.Instance == null) return;
        if (UIPanels == null || UIPanels.Length == 0) return;

        foreach (var panel in UIPanels)
        {
            if (panel != null)
                UIShake.Instance.ShakeUI(panel, duration, intensity);
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
        }

        if (FinalAudioSource2 != null && FinalCountdownClip2 != null)
        {
            FinalAudioSource2.clip = FinalCountdownClip2;
            FinalAudioSource2.volume = 1f;
            FinalAudioSource2.Play();
        }

        yield return null;
    }

    private IEnumerator FadeOutAllAudioExceptFinal(float duration)
    {
        if (MainMixer == null) yield break;

        float startVolume = 0f;
        float endVolume = -80f;
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
        timerEnded = true;
        TimerIsActive = false;

        if (FinalAudioSource != null) FinalAudioSource.Stop();
        if (FinalAudioSource2 != null) FinalAudioSource2.Stop();
        if (AudioSource != null && DeathClip != null) AudioSource.PlayOneShot(DeathClip);

        ShakeUIPanels(14f, 25f);

        StopAllCoroutines();
        StartCoroutine(TimerENdRoutine());
    }

    IEnumerator TimerENdRoutine()
    {
        BlackCanvas.gameObject.SetActive(true);
        OrderCanvas.gameObject.SetActive(false);
        TimerCanvas.gameObject.SetActive(false);
        BackgroundTerminalImage.color = Deathcolor;
        
        yield return new WaitForSeconds(12f);
        
        OverlayCanvas.SetActive(false);
        DarkImage.GameObject().SetActive(true);
    }
}
