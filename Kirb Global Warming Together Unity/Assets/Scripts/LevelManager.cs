using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public UnityEvent onWaveWarning;
    public UnityEvent onWaveStart;
    public UnityEvent onLevelAdvance;
    public UnityEvent onLose;

    [SerializeField] private GameManager mGameManager = null;
    [SerializeField] private SeaWaveScript mSeaWave = null;
    [SerializeField] private float mLevelDurationSec = 30f;
    [SerializeField] private float mLevelDurationDecreasePerWave = 0.5f;
    [SerializeField] private float mMinLevelDurationSec = 15f;
    [SerializeField] private float mDurationOffset = 10f;
    [SerializeField] private int mNextTrashToSpawn = 15;
    [SerializeField] private int mTrashCountIncrement = 3;
    [SerializeField] private float mCurrentLevelTimer = 0f;
    [SerializeField] private float mWaveWarning = 5.0f;
    [SerializeField] private GameState mGameState = GameState.Playing;

    [Header("Wave Control")]
    [SerializeField] private Transform mStartPt = null;
    [SerializeField] private Transform mEndPt = null;
    [SerializeField] private Transform mMinWave = null;
    [SerializeField] private Transform mMaxWave = null;
    [SerializeField] private SeaWaveScript[] mWavePrefabs = null;

    [Header("Debug")]
    [SerializeField] private bool mIsDebug = false;

    private int mLevelCount = 0;
    private bool mIsWaitingWaveEnd = false;
    private bool mIsWarningTriggered = false;
    private float mNextLevelDuration;

    public void NotifyWin()
    {
        mGameState = GameState.Win;
    }

    private void Awake() 
    {
        Debug.Assert(mGameManager != null, "mGameManager is not assigned!"); 
        Debug.Assert(mEndPt != null, "mEndPt is not assigned!");
    }

    private void Start() 
    {
        mNextLevelDuration = mLevelDurationSec;
    }

    // Update is called once per frame
    void Update()
    {
        if (mGameState == GameState.Playing) 
        {
            bool areAllKirbsDead = AreAllKirbsDead();

            if (areAllKirbsDead)
            {
                OnLose();
                return;
            }

            UpdateLevelTimer();
        }
    }

    private void OnLose()
    {
        if (mIsDebug) { Debug.Log("On lose"); }
        mGameState = GameState.Lose;

        onLose.Invoke();
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

        if (!mIsWarningTriggered && mCurrentLevelTimer >= mNextLevelDuration - mWaveWarning) 
        {
            mIsWarningTriggered = true;
            Debug.Log("Wave incoming!");
            onWaveWarning.Invoke();
        }

        if (mCurrentLevelTimer > mNextLevelDuration && !mIsWaitingWaveEnd)
        {
            StartLevelEndSequence();
        }
    }

    private void StartLevelEndSequence()
    {
        mIsWaitingWaveEnd = true;

        mSeaWave = SpawnWave();

        float y = Random.Range(mMinWave.position.y, mMaxWave.position.y);
        Vector3 newEndPos = mEndPt.transform.position;
        newEndPos.y = y;
        mEndPt.transform.position = newEndPos;
        mSeaWave.endTransform = mEndPt;

        mSeaWave.StartWave(mNextTrashToSpawn);

        onWaveStart.Invoke();
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
        mIsWarningTriggered = false;
        mNextTrashToSpawn += mTrashCountIncrement;
        onLevelAdvance.Invoke();
        mLevelCount++;

        mLevelDurationSec -= mLevelDurationDecreasePerWave;
        mLevelDurationSec = Mathf.Max(mLevelDurationSec, mMinLevelDurationSec);
        mNextLevelDuration = mLevelDurationSec + Random.Range(-mDurationOffset, mDurationOffset);

        mSeaWave.onWaveEnd.RemoveListener(OnWaveEnd);
        Destroy(mSeaWave.gameObject);
    }

    private void OnGUI() 
    {
        if (!mIsDebug) { return; }
    }
}

public enum GameState { Playing, Lose, Win }
