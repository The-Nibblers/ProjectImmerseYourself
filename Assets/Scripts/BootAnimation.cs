using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FakeBootScreen : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image logoImage;          
    [SerializeField] private Image flashImage;         
    [SerializeField] private List<GameObject> objectsToEnable; 

    [Header("Settings")]
    [SerializeField] private Color startColor = Color.green;   // starting CRT color
    [SerializeField] private Color defaultColor = Color.white; // color to end on
    [SerializeField] private float logoAppearDelay = 2f;       // wait before logo
    [SerializeField] private float logoVisibleTime = 3f;       // logo stays for this long
    [SerializeField] private float fadeDuration = 1f;          // how long the fade-out takes

    private void Start()
    {
        // disable all scene objects first
        foreach (GameObject obj in objectsToEnable)
            obj.SetActive(false);

        // set screen color + hide logo
        flashImage.color = startColor;
        logoImage.color = new Color(1, 1, 1, 0); // transparent
        logoImage.gameObject.SetActive(true);
        flashImage.gameObject.SetActive(true);

        // start animation
        StartCoroutine(BootSequence());
    }

    private IEnumerator BootSequence()
    {
        // wait before showing logo
        yield return new WaitForSeconds(logoAppearDelay);

        // fade logo in
        yield return StartCoroutine(FadeImage(logoImage, 0f, 1f, 1f));

        // wait while logo visible
        yield return new WaitForSeconds(logoVisibleTime);

        // fade logo out
        yield return StartCoroutine(FadeImage(logoImage, 1f, 0f, fadeDuration));

        // fade background color back to default
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            flashImage.color = Color.Lerp(startColor, defaultColor, t / fadeDuration);
            yield return null;
        }

        // logo invisible now
        logoImage.gameObject.SetActive(false);

        // enable the rest of the game
        foreach (GameObject obj in objectsToEnable)
            obj.SetActive(true);
    }

    private IEnumerator FadeImage(Image img, float from, float to, float duration)
    {
        float time = 0f;
        Color baseColor = img.color;
        while (time < duration)
        {
            time += Time.deltaTime;
            float a = Mathf.Lerp(from, to, time / duration);
            img.color = new Color(baseColor.r, baseColor.g, baseColor.b, a);
            yield return null;
        }
    }
}
