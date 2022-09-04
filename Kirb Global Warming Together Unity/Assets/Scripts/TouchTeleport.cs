using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchTeleport : MonoBehaviour
{
    public Transform teleportDestination;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Kirb"))
        {
            collision.gameObject.transform.position = teleportDestination.position;
        }
    }
}
