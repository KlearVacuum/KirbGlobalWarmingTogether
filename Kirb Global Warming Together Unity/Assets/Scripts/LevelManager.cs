using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : Singleton<LevelManager>
{
    public UnityEvent onWaveWarning;
    public UnityEvent onWaveStart;
    public UnityEvent onLevelAdvance;
    public UnityEvent onLose;
    public UnityEvent onWin;

    [Header("References")]
    [SerializeField] private GameManager mGameManager = null;
    [SerializeField] private SeaWaveScript mSeaWave = null;
    [SerializeField] private MultiTrashManager mStarterSpawner = null;
    [SerializeField] private GameState mGameState = GameState.Playing;

    [Header("Level Timing")]
    [SerializeField] private float mLevelDurationSec = 30f;
    [SerializeField] private float mLevelDurationDecreasePerWave = 0.5f;
    [SerializeField] private float mMinLevelDurationSec = 15f;
    [SerializeField] private float mDurationOffset = 10f;
    [SerializeField] private float mCurrentLevelTimer = 0f;
    [SerializeField] private float mWaveWarning = 5.0f;

    [Header("Wave Control")]
    [SerializeField] private WaveLooper mPassiveWave = null;
    [SerializeField] private Transform mStartPt = null;
    [SerializeField] private Transform mEndPt = null;
    [SerializeField] private Transform mMinWave = null;
    [SerializeField] private Transform mMaxWave = null;
    [SerializeField] private SeaWaveScript[] mWavePrefabs = null;

    [Header("Trash Progression Control")]
    [SerializeField] private List<TrashSpawnInfo> mTrashSpawnInfoList;
    [SerializeField] private int mNextTrashToSpawn = 15;
    [SerializeField] private int mTrashCountIncrement = 3;

    private int mCurrentTrashPhaseCount = 0;
    [SerializeField] private int mCurrentTrashPhaseIndex = 0;

    [Header("Debug")]
    [SerializeField] private bool mIsDebug = false;

    private int mLevelCount = 0;
    private bool mIsWaitingWaveEnd = false;
    private bool mIsWarningTriggered = false;
    private float mNextLevelDuration;
    private GameObject mLastDead;

    public void NotifyLastDead(GameObject lastDead)
    {
        mLastDead = lastDead;
    }

    public void NotifyWin()
    {
        if (mGameState == GameState.Win) { return; }
        mGameState = GameState.Win;
        onWin.Invoke();
    }

    protected override void Awake() 
    {
        base.Awake();

        Debug.Assert(mGameManager != null, "mGameManager is not assigned!"); 
        Debug.Assert(mEndPt != null, "mEndPt is not assigned!");
        Debug.Assert(mTrashSpawnInfoList != null && mTrashSpawnInfoList.Count > 0, "mTrashSpawnInfoList are not assigned!");
        Debug.Assert(mStarterSpawner != null, "mTrashManager is not assigned!");
        Debug.Assert(mPassiveWave != null, "mPassiveWave is not assigned!");
    }

    private void Start() 
    {
        OnGameStart();
    }

    private void OnGameStart()
    {
        mNextLevelDuration = mLevelDurationSec;

        TrashSpawnInfo info = (mTrashSpawnInfoList[mCurrentTrashPhaseIndex]);
        mStarterSpawner.SetTrashToSpawn(info);
        mStarterSpawner.SpawnSomeTrash(info.Quantity);
        IncrementTrashPhase();
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
            mPassiveWave.Loop(false);
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

        TrashSpawnInfo info = (mTrashSpawnInfoList[mCurrentTrashPhaseIndex]);
        wave.trashManager.SetTrashToSpawn(info);
        wave.trashSpawnNum = info.Quantity;

        return wave;
    }

    private void OnWaveEnd()
    {
        if (!mIsWaitingWaveEnd) { return; }
        mPassiveWave.Loop(true);
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

        IncrementTrashPhase();

        mSeaWave.onWaveEnd.RemoveListener(OnWaveEnd);
        Destroy(mSeaWave.gameObject);
    }

    private void IncrementTrashPhase()
    {
        mCurrentTrashPhaseCount++;

        if (mCurrentTrashPhaseCount >= mTrashSpawnInfoList[mCurrentTrashPhaseIndex].NumLevels)
        {
            mCurrentTrashPhaseIndex = Mathf.Min(mTrashSpawnInfoList.Count - 1, mCurrentTrashPhaseIndex + 1);
            mCurrentTrashPhaseCount = 0;
        }
    }

    private void OnGUI() 
    {
        if (!mIsDebug) { return; }
    }
}

public enum GameState { Playing, Lose, Win }
