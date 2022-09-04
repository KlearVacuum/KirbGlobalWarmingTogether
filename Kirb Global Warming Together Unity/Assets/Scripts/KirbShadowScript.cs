using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KirbShadowScript : MonoBehaviour
{
    public Transform followTransform;
    public Vector3 offset;

    private void Update()
    {
        if (followTransform != null)
        {
            transform.position = followTransform.position + offset;
        }
    }
}
