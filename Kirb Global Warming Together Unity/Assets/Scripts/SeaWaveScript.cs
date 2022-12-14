using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SeaWaveScript : MonoBehaviour
{
    public bool passiveWave;
    public UnityEvent onWaveEnd;

    public Transform startTransform;
    public Transform endTransform;
    public GameObject waveSprite;
    public GameObject waveShadowPrefab;
    private WaveShadowScript waveShadowScript;

    public MultiTrashManager trashManager;
    [Tooltip("Number of trash spawners to select and spawn trash from")]
    public int trashSpawnNum;

    public bool startWave;
    private bool moveWave;
    // Duration it takes to complete 1 oscillation
    public float travelPeriod;
    private float prevT;

    [SerializeField] private bool mIsKiller = true;

    private Vector2 travelPath;
    private float currentTravelTime;
    private bool trashSpawned;

    private AudioSource _aSource;
    public AudioClip _aClipBigWave;
    
    public bool IsWaveMoving { get { return moveWave; } }

    private void Awake()
    {
        _aSource = gameObject.GetComponent<AudioSource>();
    }

    private void Start()
    {
        travelPath = endTransform.position - startTransform.position;

        if (!mIsKiller)
        {
            waveSprite.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    private void Update()
    {
        MoveWave();
    }

    // call this to start the wave
    public void StartWave(int trashToSpawn)
    {
        if (!moveWave)
        {
            startWave = true;
            trashSpawnNum = trashToSpawn;
        }
    }

    public void MoveWave()
    {
        if (startWave)
        {
            if (!passiveWave)
            {
                _aSource.PlayOneShot(_aClipBigWave);
            }

            moveWave = true;
            startWave = false;
            trashSpawned = false;
        }

        if (moveWave)
        {
            currentTravelTime += Time.deltaTime;
            float t = Mathf.Sin(currentTravelTime * 2 / travelPeriod);

            waveSprite.transform.position = startTransform.position + new Vector3(travelPath.x * t, travelPath.y * t, 0);

            if (t < prevT)
            {
                if (!trashSpawned)
                {
                    GameObject waveShadow = Instantiate(waveShadowPrefab, waveSprite.transform.position + new Vector3(0, 0.05f, 0), Quaternion.identity);
                    waveShadowScript = waveShadow.GetComponent<WaveShadowScript>();
                    waveShadowScript.followTransform = waveSprite.transform;
                    waveShadowScript.startFade = true;

                    trashManager.SpawnSomeTrash(trashSpawnNum);
                    trashSpawned = true;
                }
            }

            prevT = t;

            if (currentTravelTime >= travelPeriod * 2)
            {
                currentTravelTime = 0;
                moveWave = false;

                onWaveEnd.Invoke();
            }
            else if (currentTravelTime >= travelPeriod)
            {
            }
        }
    }
}
