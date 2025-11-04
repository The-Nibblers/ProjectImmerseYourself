using System;
using System.Collections;
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
    [SerializeField] private TMP_Text conformationText;
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
            StartCoroutine(InvalidHandling());
        }
    }

    private void HandleSuccess()
    {
        input.gameObject.SetActive(false);
        conformationText.gameObject.SetActive(true);
        conformationText.text = "correct!";
    }

    IEnumerator  InvalidHandling()
    {
        input.text = "";
        conformationText.text = "Invalid Passkey";
        conformationText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        conformationText.gameObject.SetActive(false);
    }
}
