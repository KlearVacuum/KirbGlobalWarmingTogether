using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SeaWaveScript : MonoBehaviour
{
    public UnityEvent onWaveEnd;

    public Transform startTransform;
    public Transform endTransform;
    public GameObject waveSprite;

    public MultiTrashManager trashManager;
    [Tooltip("Number of trash spawners to select and spawn trash from")]
    public int trashSpawnNum;

    public bool startWave;
    private bool moveWave;
    // Duration it takes to complete 1 oscillation
    public float travelPeriod;

    private Vector2 travelPath;
    private float currentTravelTime;
    private bool trashSpawned;
    
    public bool IsWaveMoving { get { return moveWave; } }

    private void Start()
    {
        travelPath = endTransform.position - startTransform.position;
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
            moveWave = true;
            startWave = false;
            trashSpawned = false;
        }

        if (moveWave)
        {
            currentTravelTime += Time.deltaTime;
            float t = Mathf.Sin(currentTravelTime * 2 / travelPeriod);

            waveSprite.transform.position = startTransform.position + new Vector3(travelPath.x * t, travelPath.y * t, 0);

            if (currentTravelTime >= travelPeriod * 2)
            {
                currentTravelTime = 0;
                moveWave = false;

                onWaveEnd.Invoke();
            }
            else if (currentTravelTime > travelPeriod)
            {
                if (!trashSpawned)
                {
                    trashManager.SpawnSomeTrash(trashSpawnNum);
                    trashSpawned = true;
                }
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Kirb"))
        {
            var kirb = collision.gameObject.GetComponent<AIEntity>();
            kirb.waterOverlap++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Kirb"))
        {
            var kirb = collision.gameObject.GetComponent<AIEntity>();
            kirb.waterOverlap--;
        }
    }
}
