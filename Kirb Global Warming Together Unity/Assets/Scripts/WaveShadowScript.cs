using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveShadowScript : MonoBehaviour
{
    public Transform followTransform;
    public bool startFade;
    public float fadeTime;
    SpriteRenderer spriteRenderer;
    private float startAlpha;

    private float fadeTimer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        startAlpha = spriteRenderer.color.a;
        fadeTimer = fadeTime;
    }

    private void Update()
    {
        if (startFade)
        {
            fadeTimer -= Time.deltaTime;
            if (fadeTimer <= 0) Destroy(gameObject);
            else
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, 
                                                    spriteRenderer.color.g, 
                                                        spriteRenderer.color.b, 
                                                            fadeTimer / fadeTime * startAlpha);
            }
        }
        else
        {
            transform.position = followTransform.position;
        }
    }
}
