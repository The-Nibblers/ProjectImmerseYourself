using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlinkingText : MonoBehaviour
{
    [Header("Blink Settings")]
    [SerializeField] private float blinkInterval = 0.5f; // seconds
    [SerializeField] private bool startVisible = true;

    private TMP_Text tmpText;
    private Text uiText;
    private bool isVisible;
    private float timer;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        uiText = GetComponent<Text>();
        isVisible = startVisible;
        SetVisibility(isVisible);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= blinkInterval)
        {
            timer = 0f;
            isVisible = !isVisible;
            SetVisibility(isVisible);
        }
    }

    private void SetVisibility(bool visible)
    {
        if (tmpText != null)
            tmpText.enabled = visible;
        else if (uiText != null)
            uiText.enabled = visible;
    }
}