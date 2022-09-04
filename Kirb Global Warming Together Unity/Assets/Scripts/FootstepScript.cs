using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepScript : MonoBehaviour
{
    public float startFadeDelay;
    public float fadeTime;
    public bool destroyOnFade = false;

    private float startAlpha;
    private SpriteRenderer spriteRenderer;
    private float timer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startAlpha = spriteRenderer.color.a;
    }
    private void OnEnable()
    {
        timer = startFadeDelay + fadeTime;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, startAlpha);

    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= fadeTime && timer > 0)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, startAlpha * timer / fadeTime);
        }

        if (timer <= 0)
        {
            if (destroyOnFade)
            {
                Destroy(gameObject);
            }
            else gameObject.SetActive(false);
        }
    }
}
