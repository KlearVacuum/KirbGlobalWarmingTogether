using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteTrashOnCollision : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trash"))
        {
            collision.gameObject.GetComponent<TrashScript>().DeleteTrashInstant();
        }
    }
}
