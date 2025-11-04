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
}
public class OrderManager : MonoBehaviour
{
    [SerializeField] private TimerManager timerManager;
    [SerializeField] private TMP_Text OrderPlaceHolder;
    [SerializeField] private GameObject SpaceToConfirm;
    [SerializeField] private Image OrderImage;
    [SerializeField] private List<Orders> orderHolder;
    private int currentOrderIndex;
    private Orders currentOrder;
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
            OrderPlaceHolder.GameObject().SetActive(false);
            RandomizeOrder();
            OrderImage.gameObject.SetActive(true);
            orderConformations++;
            return;
        }
        else if(orderConformations < orderHolder.Count)
        {
            orderHolder.RemoveAt(currentOrderIndex);
            RandomizeOrder();
            orderConformations++;
            return;
        }
        else
        {
            orderHolder.RemoveAt(currentOrderIndex);
            OrderImage.gameObject.SetActive(false);
            OrderPlaceHolder.text = "all orders completed, dont forget to clock out";
            SpaceToConfirm.SetActive(false);
            OrderPlaceHolder.GameObject().SetActive(true);
            timerManager.PauseTimer();
        }
    }

    private void RandomizeOrder()
    {
        currentOrderIndex = Random.Range(0, orderHolder.Count);
        currentOrder = orderHolder[currentOrderIndex];
        OrderImage.sprite = currentOrder.orderImage;
    }
}
