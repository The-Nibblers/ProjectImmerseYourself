using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

[Serializable]
public struct colorCode
{
    public Color color;
    public int pass;
}
public class ColorCodeManager : MonoBehaviour
{
    [SerializeField] private List<colorCode> colorCodeList;
    [SerializeField] private Image image;
    private colorCode selectedColor;

    public void Start()
    {
        RandomizeColorCode();
    }

    private void RandomizeColorCode()
    {
        selectedColor = colorCodeList[Random.Range(0, colorCodeList.Count)];
        image.color = selectedColor.color;
    }
}
