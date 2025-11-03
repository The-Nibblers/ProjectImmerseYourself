using TMPro;
using UnityEngine;

public class TMPNumberInputLimiter : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private int maxCharacters = 5;

    void Start()
    {
        inputField.characterLimit = maxCharacters;
        inputField.onValueChanged.AddListener(OnValueChanged);
    }

    void OnValueChanged(string text)
    {
        string filtered = "";
        foreach (char c in text)
        {
            if (char.IsDigit(c))
                filtered += c;
        }

        if (filtered != text)
        {
            inputField.text = filtered;
        }
    }
}