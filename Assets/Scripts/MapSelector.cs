using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MapSelector : MonoBehaviour
{
    [Header("Points and cursor")]
    public List<Transform> points = new List<Transform>();
    [SerializeField] private List<string> pointText = new List<string>();
    public Transform cursor;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Visual Feedback")]
    public Color hoverColor = Color.yellow;
    public Color normalColor = Color.white;
    [SerializeField] private TextMeshProUGUI submitText;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private string submitTextString = "Press [P] to submit flightpath.";
    [SerializeField] private CanvasGroup animationCanvas;
    [SerializeField] private CanvasGroup successCanvas;

    [Header("Audio Feedback")]
    public AudioSource audioSource;     // Assign in Inspector
    public AudioClip hoverSound;        // Assign in Inspector
    [SerializeField] private AudioClip dialupSound;

    [Header("Navigation")]
    public float snapDuration = 0.25f;
    public AnimationCurve snapCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool wrapAround = true;

    [Header("Input")]
    public InputActionAsset actionsAsset;
    public string actionMapName = "UI";
    public string navigateActionName = "Navigate";
    public string submitActionName = "Submit";
    [SerializeField] private string flightActionName = "Flight";

    private InputAction navigateAction;
    private InputAction submitAction;
    private InputAction flightAction;

    private int currentIndex = 0;
    private bool isSnapping = false;
    private Vector2 navigateBuffer = Vector2.zero;

    // Stores selected coordinates
    public List<Vector3> selectedCoordinates = new List<Vector3>();

    private void OnEnable()
    {
        var map = actionsAsset.FindActionMap(actionMapName, true);
        navigateAction = map.FindAction(navigateActionName, true);
        submitAction = map.FindAction(submitActionName, true);
        flightAction = map.FindAction(flightActionName, true);

        navigateAction.performed += OnNavigate;
        submitAction.performed += OnSubmit;
        flightAction.performed += OnFlightSubmit;


        map.Enable();

        if (cursor != null && points.Count > 0)
        {
            cursor.position = points[currentIndex].position;
            UpdatePointVisuals(currentIndex); // highlight first point
        }
    }

    private void OnDisable()
    {
        if (navigateAction != null) navigateAction.performed -= OnNavigate;
        if (submitAction != null) submitAction.performed -= OnSubmit;
        actionsAsset.Disable();
    }

    private void Update()
    {
        if (selectedCoordinates.Count >= 4)
        {
            submitText.text = submitTextString;
        }
        else
        {
            submitText.text = "Press [Enter] to add a star to flightpath.";
        }
    }

    private void OnFlightSubmit(InputAction.CallbackContext ctx)
    {
        if(selectedCoordinates.Count >= 4)
        {
            Debug.Log("Flight path initiated");
            // TODO: Implement flight animation
            StartCoroutine(FlightPathAnimation());
        }
        else
        {
            StartCoroutine(TimedMessage());
        }
    }

    IEnumerator FlightPathAnimation()
    {
        audioSource.PlayOneShot(dialupSound);
        animationCanvas.alpha = 1.0f;
        yield return new WaitForSeconds(dialupSound.length);
        animationCanvas.alpha = 0;
        successCanvas.alpha = 1.0f;
        audioSource.PlayOneShot(hoverSound);
    }

    IEnumerator TimedMessage()
    {
        errorText.text = "ERROR: NOT ENOUGH STARS SELECTED!";
        yield return new WaitForSeconds(2f);
        errorText.text = "";
    }

    private void OnNavigate(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();

        const float threshold = 0.5f;
        bool xTrigger = Mathf.Abs(input.x) > threshold && Mathf.Abs(navigateBuffer.x) <= threshold;
        bool yTrigger = Mathf.Abs(input.y) > threshold && Mathf.Abs(navigateBuffer.y) <= threshold;

        if (xTrigger)
        {
            if (input.x > 0) MoveToNext();
            else MoveToPrevious();
        }
        else if (yTrigger)
        {
            if (input.y > 0) MoveToPrevious();
            else MoveToNext();
        }

        navigateBuffer = input;
    }

    private void OnSubmit(InputAction.CallbackContext ctx)
    {
        if (points.Count == 0) return;
        Vector3 selected = points[currentIndex].position;
        selectedCoordinates.Add(selected);
        Debug.Log($"Selected point {currentIndex} at {selected}");

        string text = (currentIndex < pointText.Count) ? pointText[currentIndex] : $"Point {currentIndex}";
        if (feedbackText != null)
            feedbackText.text = $"Selected: {text}";

        Debug.Log($"Selected point {currentIndex} at {selected} ({text})");
    }

    private void MoveToNext()
    {
        if (points.Count == 0) return;
        int next = currentIndex + 1;
        if (next >= points.Count) next = wrapAround ? 0 : currentIndex;
        SnapToIndex(next);
    }

    private void MoveToPrevious()
    {
        if (points.Count == 0) return;
        int prev = currentIndex - 1;
        if (prev < 0) prev = wrapAround ? points.Count - 1 : currentIndex;
        SnapToIndex(prev);
    }

    private void SnapToIndex(int newIndex)
    {
        if (isSnapping || newIndex == currentIndex || cursor == null) return;
        StopAllCoroutines();
        StartCoroutine(SnapRoutine(points[currentIndex].position, points[newIndex].position, newIndex));
    }

    private IEnumerator SnapRoutine(Vector3 from, Vector3 to, int targetIndex)
    {
        isSnapping = true;
        float t = 0f;
        while (t < snapDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / snapDuration);
            float eased = snapCurve.Evaluate(alpha);
            cursor.position = Vector3.LerpUnclamped(from, to, eased);
            yield return null;
        }
        cursor.position = to;
        currentIndex = targetIndex;
        isSnapping = false;

        // Apply hover feedback
        UpdatePointVisuals(currentIndex);
        PlayHoverSound();
    }

    private void UpdatePointVisuals(int highlightIndex)
    {
        for (int i = 0; i < points.Count; i++)
        {
            var img = points[i].GetComponent<Image>();
            if (img != null)
            {
                img.color = (i == highlightIndex) ? hoverColor : normalColor;
            }
        }
        if (feedbackText != null)
        {
            string text = (highlightIndex < pointText.Count) ? pointText[highlightIndex] : $"Starsystem: {highlightIndex}";
            feedbackText.text = text;
        }
    }

    private void PlayHoverSound()
    {
        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }
}
