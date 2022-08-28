using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveLooper : MonoBehaviour
{
    [SerializeField] private SeaWaveScript mSeaWave = null;
    [SerializeField] private float mWaveInterval = 1.0f;

    private float mTimer = 0;
    private bool mIsLooping = true;

    public void Loop(bool isLooping)
    {
        mIsLooping = isLooping;
    }

    private void Awake() 
    {
        Debug.Assert(mSeaWave != null, "mSeaWave is not assigned!");     
    }

    private void Update()
    {
        if (mIsLooping && !mSeaWave.IsWaveMoving && !mSeaWave.startWave)
        {
            mSeaWave.StartWave(0);
        }
    }
}
