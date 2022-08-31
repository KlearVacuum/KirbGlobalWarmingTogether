using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FadeInOut : MonoBehaviour
{
    public float fadeInSpeed;
    public float fadeOutSpeed;
    public float showTime;

    private TextMeshProUGUI myText;

    private bool fadeIn;
    private bool showing;
    private bool fadeOut;
    private float timer;

    private void Awake()
    {
        myText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        fadeIn = true;
        showing = false;
        fadeOut = false;
    }

    private void Update()
    {
        if (fadeIn)
        {
            timer += Time.deltaTime;
            float newAlpha = timer * fadeInSpeed;
            if (newAlpha >= 1)
            {
                newAlpha = 1;
                showing = true;
                timer = 0;
                fadeIn = false;
            }
            myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, newAlpha);
        }

        if (showing)
        {
            timer += Time.deltaTime;
            if (timer >= showTime)
            {
                showing = false;
                fadeOut = true;
                timer = 0;
            }
        }

        if (fadeOut)
        {
            timer += Time.deltaTime;
            float newAlpha = 1 - (timer * fadeOutSpeed);
            if (newAlpha <= 0)
            {
                newAlpha = 0;
                fadeOut = false;
                gameObject.SetActive(false);
            }
            myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, newAlpha);
        }
    }
}
