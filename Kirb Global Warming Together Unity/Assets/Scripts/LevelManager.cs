using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public UnityEvent onLevelAdvance;
    public UnityEvent onLose;

    [SerializeField] private GameManager mGameManager = null;
    [SerializeField] private SeaWaveScript mSeaWave = null;
    [SerializeField] private float mLevelDurationSec = 30f;
    [SerializeField] private int mNextTrashToSpawn = 15;
    [SerializeField] private int mTrashCountIncrement = 3;
    [SerializeField] private float mCurrentLevelTimer = 0f;
    [SerializeField] private GameState mGameState = GameState.Playing;

    [Header("Wave Control")]
    [SerializeField] private Transform mStartPt = null;
    [SerializeField] private Transform mEndPt = null;
    [SerializeField] private SeaWaveScript[] mWavePrefabs = null;

    [Header("Debug")]
    [SerializeField] private bool mIsDebug = false;

    private int mLevelCount = 0;
    private bool mIsWaitingWaveEnd = false;

    private void Awake() 
    {
        Debug.Assert(mGameManager != null, "mGameManager is not assigned!"); 
        Debug.Assert(mEndPt != null, "mEndPt is not assigned!");
    }

    // Update is called once per frame
    void Update()
    {
        if (mGameState == GameState.Playing) 
        {
            bool areAllKirbsDead = AreAllKirbsDead();

            if (areAllKirbsDead) 
            {
                if (mIsDebug) 
                {
                    Debug.Log("On lose");
                }
                onLose.Invoke();
                return;
            }

            UpdateLevelTimer();
        }
    }

    private bool AreAllKirbsDead()
    {
        foreach (var kirb in GlobalGameData.allAiEntities)
        {
            if (!kirb.dead) { return false; }
        }

        return true;
    }

    private void UpdateLevelTimer()
    {
        mCurrentLevelTimer += Time.deltaTime;
        if (mCurrentLevelTimer > mLevelDurationSec && !mIsWaitingWaveEnd)
        {
            StartLevelEndSequence();
        }
    }

    private void StartLevelEndSequence()
    {
        mIsWaitingWaveEnd = true;

        mSeaWave = SpawnWave();

        // TODO: Set end point stuff.
        mSeaWave.endTransform = mEndPt;

        mSeaWave.StartWave(mNextTrashToSpawn);
    }

    private SeaWaveScript SpawnWave()
    {
        int waveIndexToSpawn = mLevelCount;

        waveIndexToSpawn = Mathf.Min(mLevelCount, mWavePrefabs.Length - 1);

        SeaWaveScript wave = Instantiate<SeaWaveScript>(mWavePrefabs[waveIndexToSpawn]);
        wave.startTransform = mStartPt;
        wave.onWaveEnd.AddListener(OnWaveEnd);

        return wave;
    }

    private void OnWaveEnd()
    {
        if (!mIsWaitingWaveEnd) { return; }
        AdvanceLevel();
    }

    public void AdvanceLevel()
    {
        mIsWaitingWaveEnd = false;
        mCurrentLevelTimer = 0;
        mNextTrashToSpawn += mTrashCountIncrement;
        onLevelAdvance.Invoke();
        mLevelCount++;

        mSeaWave.onWaveEnd.RemoveListener(OnWaveEnd);
        Destroy(mSeaWave.gameObject);
    }

    private void OnGUI() 
    {
        if (!mIsDebug) { return; }
    }
}

public enum GameState { Playing, Lose, Win }
