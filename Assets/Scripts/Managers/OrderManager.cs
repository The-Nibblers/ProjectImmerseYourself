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
    public string AudioStartID;
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

    private void HandleOrderConfirmed()
    {
        if(IsOrderShown == false)
        {
            // First time setup
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

            // Remove current order AFTER confirming it
            orderHolder.RemoveAt(currentOrderIndex);

            // If no orders left
            if (orderHolder.Count == 0)
            {
                OrderImage.gameObject.SetActive(false);
                OrderPlaceHolder.text = "All orders completed, don't forget to clock out!";
                SpaceToConfirm.SetActive(false);
                OrderPlaceHolder.gameObject.SetActive(true);
                timerManager.PauseTimer();
                return;
            }

            // If still orders left
            RandomizeOrder();
            OrderImage.gameObject.SetActive(true);
            audioManager.PlayByID("Error");
            orderConformations++;
            IsOrderShown = true;
        }
        else
        {
            IsOrderShown = false;
            audioManager.PlayByID("TaskComplete");
            audioManager.StopAudioByID("Error");
            OrderImage.gameObject.SetActive(false);
            return;
        }

    }

    private void RandomizeOrder()
    {
        currentOrderIndex = Random.Range(0, orderHolder.Count);
        currentOrder = orderHolder[currentOrderIndex];
        OrderImage.sprite = currentOrder.orderImage;
    }
}
