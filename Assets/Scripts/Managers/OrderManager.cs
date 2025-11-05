using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public struct Orders
{
    public string orderName;
    public Sprite orderImage;
    public bool HasAudio;
    public string AudioClipName;
    public string LoopAudioClipName;
}

public class OrderManager : MonoBehaviour
{
    [SerializeField] private TimerManager timerManager;
    [SerializeField] private TMP_Text OrderPlaceHolder;
    [SerializeField] private GameObject SpaceToConfirm;
    [SerializeField] private Image OrderImage;
    [SerializeField] private List<Orders> orderHolder;
    [SerializeField] private AudioManager audioManager;
    private int currentOrderIndex;
    private Orders currentOrder;
    private int orderConformations;
    private bool IsOrderShown = false;

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            HandleOrderConfirmed();
        }
    }

    private void RandomizeOrder()
    {
        // Choose new random order
        currentOrderIndex = Random.Range(0, orderHolder.Count);
        currentOrder = orderHolder[currentOrderIndex];
        OrderImage.sprite = currentOrder.orderImage;

        // If order has audio, play its unique sound
        if (currentOrder.HasAudio)
        {
            // Stop loop (in case it’s still running)
            audioManager.StopAudioByID(currentOrder.LoopAudioClipName);
            // Play the order’s main audio
            audioManager.PlayByID(currentOrder.AudioClipName);
        }
    }

    private void HandleOrderConfirmed()
    {
        if (!IsOrderShown)
        {
            audioManager.PlayByID("MusicProblem");
            audioManager.StopAudioByID("MusicCalm");
            // First order setup
            if (orderConformations == 0)
            {
                audioManager.PlayByID("Error");
                timerManager.StartTimer();
                OrderPlaceHolder.gameObject.SetActive(false);
                RandomizeOrder();
                OrderImage.gameObject.SetActive(true);
                orderConformations++;
                IsOrderShown = true;
                return;
            }

            // Remove finished order
            orderHolder.RemoveAt(currentOrderIndex);

            // If no orders left
            if (orderHolder.Count == 0)
            {
                audioManager.StopAudioByID("MusicProblem");
                audioManager.StopAudioByID("MusicCalm");
                // Stop all audio when done
                if (currentOrder.HasAudio)
                {
                    audioManager.StopAudioByID(currentOrder.AudioClipName);
                    audioManager.StopAudioByID(currentOrder.LoopAudioClipName);
                }

                OrderImage.gameObject.SetActive(false);
                OrderPlaceHolder.text = "All orders completed, don't forget to clock out!";
                SpaceToConfirm.SetActive(false);
                OrderPlaceHolder.gameObject.SetActive(true);
                timerManager.PauseTimer();
                
                return;
            }

            // Go to next order
            RandomizeOrder();
            OrderImage.gameObject.SetActive(true);
            audioManager.PlayByID("Error");
            orderConformations++;
            IsOrderShown = true;
        }
        else
        {
            audioManager.PlayByID("MusicCalm");
            audioManager.StopAudioByID("MusicProblem");
            // Task complete for this order
            IsOrderShown = false;
            audioManager.PlayByID("TaskComplete");
            audioManager.StopAudioByID("Error");

            // Stop the main order sound, start the loop
            if (currentOrder.HasAudio)
            {
                audioManager.StopAudioByID(currentOrder.AudioClipName);
                audioManager.PlayByID(currentOrder.LoopAudioClipName);
            }

            OrderImage.gameObject.SetActive(false);
        }
    }
}
