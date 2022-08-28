using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertSpeakerObject : MonoBehaviour
{
    public bool _testTheWeeWoo;

    void Start()
    {
        transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
    }

    void Update()
    {
        if (_testTheWeeWoo)
        {
            _testTheWeeWoo = false;
            DoTheWeeWoo();
        }
    }

    public void DoTheWeeWoo()
    {
        LeanTween.cancel(gameObject);

        LeanTween
            .rotateX(gameObject, 0.0f, 0.5f)
            .setEaseOutBack();
        LeanTween
            .delayedCall(0.5f, () => {
                LeanTween
                    .scaleY(gameObject, 0.225f, 0.25f)
                    .setLoopPingPong(5)
                    .setOnComplete(() => {
                        LeanTween
                            .rotateX(gameObject, 90.0f, 0.5f)
                            .setEaseInBack();
                    }); });
    }
}
