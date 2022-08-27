using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hit something");
        if (collision.gameObject.CompareTag("Kirb"))
        {
            Debug.Log("Hit the kirb");
            var kirb = collision.gameObject.GetComponent<AIEntity>();
            kirb.waterOverlap++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Exit something");
        if (collision.gameObject.CompareTag("Kirb"))
        {
            var kirb = collision.gameObject.GetComponent<AIEntity>();
            kirb.waterOverlap--;
        }
    }
}
