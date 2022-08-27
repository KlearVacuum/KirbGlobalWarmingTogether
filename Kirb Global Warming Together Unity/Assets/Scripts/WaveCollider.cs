using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Kirb"))
        {
            var kirb = collision.gameObject.GetComponent<AIEntity>();
            kirb.waterOverlap++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Kirb"))
        {
            var kirb = collision.gameObject.GetComponent<AIEntity>();
            kirb.waterOverlap--;
        }
    }
}
