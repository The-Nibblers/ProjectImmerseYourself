using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovieController : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField] private GameObject videoPlayer;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            videoPlayer.SetActive(true);
        }
    }
}
