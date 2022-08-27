using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepoScript : MonoBehaviour
{
    void Start()
    {
        GlobalGameData.AddDepo(gameObject);
    }

    public void RemoveDepo()
    {
        Debug.Log("depo is gone");
        GlobalGameData.RemoveDepo(gameObject);
        Destroy(this);
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
}