using System;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private TMP_InputField input;
    private colorCode selectedColor;

    public void Start()
    {
        RandomizeColorCode();
        input.onEndEdit.AddListener(CheckPass);
    }

    private void RandomizeColorCode()
    {
        selectedColor = colorCodeList[Random.Range(0, colorCodeList.Count)];
        image.color = selectedColor.color;
    }

    private void CheckPass(string arg0)
    {
        if (arg0 == selectedColor.pass.ToString())
        {
            HandleSuccess();
        }
        else
        {
            input.text = "";
        }
    }

    private void HandleSuccess()
    {
        
    }
}
