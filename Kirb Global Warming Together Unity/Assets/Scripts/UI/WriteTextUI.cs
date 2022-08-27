using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WriteTextUI : MonoBehaviour
{
    TextMeshProUGUI mText;

    private void Awake()
    {
        mText = GetComponent<TextMeshProUGUI>();
    }

    public void WriteText(string text)
    {
        mText.text = text;
    }

    public void AddText(string text)
    {
        mText.text += text;
    }
}
