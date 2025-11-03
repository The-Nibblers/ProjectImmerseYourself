using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MapSelector : MonoBehaviour
{
    [Header("Points and cursor")]
    public List<Transform> points = new List<Transform>();
    public Transform cursor;

    [Header("Visual Feedback")]
    public Color hoverColor = Color.yellow;
    public Color normalColor = Color.white;

    [Header("Audio Feedback")]
    public AudioSource audioSource;     // Assign in Inspector
    public AudioClip hoverSound;        // Assign in Inspector

    [Header("Navigation")]
    public float snapDuration = 0.25f;
    public AnimationCurve snapCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool wrapAround = true;

    [Header("Input")]
    public InputActionAsset actionsAsset;
    public string actionMapName = "UI";
    public string navigateActionName = "Navigate";
    public string submitActionName = "Submit";

    private InputAction navigateAction;
    private InputAction submitAction;

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

        navigateAction.performed += OnNavigate;
        submitAction.performed += OnSubmit;

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
    }

    private void PlayHoverSound()
    {
        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }
}
