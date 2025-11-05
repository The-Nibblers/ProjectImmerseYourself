using UnityEngine;
using System.Collections;

public class UIShake : MonoBehaviour
{
    public static UIShake Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Debug.Log("Chatgerpeter say: UIShake ready!");
    }

    /// <summary>
    /// Shake one UI element (or a canvas/panel with children)
    /// </summary>
    /// <param name="uiElement">RectTransform to shake</param>
    /// <param name="duration">Seconds to shake</param>
    /// <param name="intensity">How strong</param>
    public void ShakeUI(RectTransform uiElement, float duration, float intensity)
    {
        StartCoroutine(ShakeUIRoutine(uiElement, duration, intensity));
    }

    private IEnumerator ShakeUIRoutine(RectTransform uiElement, float duration, float intensity)
    {
        if (uiElement == null)
            yield break;

        Vector3 originalPos = uiElement.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Vector3 offset = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                0f
            ) * intensity;

            uiElement.localPosition = originalPos + offset;
            yield return null;
        }

        // Reset position
        uiElement.localPosition = originalPos;
    }
}