using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class OrderManager : MonoBehaviour
{
    [SerializeField] private TimerManager timerManager;
    [SerializeField] private GameObject OrderPlaceHolder;
    [SerializeField] private Image OrderImage;
    [SerializeField] private Dictionary<int, Image> orderHolder;
    private int currentOrder;
    private int orderConformations;
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            HandleOrderConfirmed();
        }
    }

    private void HandleOrderConfirmed()
    {
        if (orderConformations == 0)
        {
            timerManager.StartTimer();
            OrderPlaceHolder.SetActive(false);
            RandomizeOrder();
            OrderImage.gameObject.SetActive(true);
            orderConformations++;
            return;
        }
        else if(orderConformations < orderHolder.Count)
        {
            
        }
        else
        {
            
        }
    }

    private void RandomizeOrder()
    {
        currentOrder = orderHolder[Random.Range(0, orderHolder.Count)];
    }
}
