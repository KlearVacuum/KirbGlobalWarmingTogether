using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertSpeakerObject : MonoBehaviour
{
    [SerializeField] private int mNumWeeWoo = 5;
    public bool _testTheWeeWoo;

    private AudioSource _aSource;
    public AudioClip _aClipAlarm;
    private void Awake()
    {
        _aSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
    }

    float debugTime;
    void Update()
    {
        if (_testTheWeeWoo)
        {
            _testTheWeeWoo = false;
            DoTheWeeWoo();
        }
    }

    public void PlayWeeWooSound()
    {
        _aSource.PlayOneShot(_aClipAlarm);
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
                    .setLoopPingPong(mNumWeeWoo)
                    .setOnComplete(() => {
                        LeanTween
                            .rotateX(gameObject, 90.0f, 0.5f)
                            .setEaseInBack();
                    }); });
    }
}
