using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepoScript : MonoBehaviour
{
    public bool locked = false;
    public int unlockCost;
    public Color lockedColor;
    public Color unlockedColor;

    SpriteRenderer spriteRenderer;

    public GameObject glowObject;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        if (!locked)
        {
            GlobalGameData.AddDepo(gameObject);
            spriteRenderer.color = unlockedColor;
            gameObject.tag = "Depo";
        }
        else
        {
            spriteRenderer.color = lockedColor;
            gameObject.tag = "Untagged";
        }
        glowObject.SetActive(false);
    }

    public void RemoveDepo()
    {
        Debug.Log("depo is gone");
        GlobalGameData.RemoveDepo(gameObject);
        Destroy(this);
    }

    public void UnlockDepo()
    {
        if (!locked) return;
        if (GlobalGameData.cash >= unlockCost)
        {
            GlobalGameData.AddDepo(gameObject);
            GlobalGameData.cash -= unlockCost;
            spriteRenderer.color = unlockedColor;
            gameObject.tag = "Depo";
            locked = false;
        }
        else
        {
            GameManager._instance.NoMoneyFeedback();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Kirb"))
        {
            collision.gameObject.GetComponent<AIEntity>().depoOverlap++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Kirb"))
        {
            collision.gameObject.GetComponent<AIEntity>().depoOverlap--;
        }
    }

    private void OnMouseDown()
    {
        if (!locked)
        {
            GameManager._instance.SpawnKirb();
        }
        else
        {
            UnlockDepo();
        }
    }

    private void OnMouseEnter()
    {
        if (locked)
        {
            if (GlobalGameData.cash >= unlockCost)
            {
                // glow to show unlock
                glowObject.SetActive(true);
            }
        }
        else
        {
            // glow to show can spawn
            glowObject.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        // un-glow
        glowObject.SetActive(false);
    }
}