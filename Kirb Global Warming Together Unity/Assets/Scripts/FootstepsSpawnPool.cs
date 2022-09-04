using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepsSpawnPool : MonoBehaviour
{
    public GameObject footstepPrefab;
    public int maxFootsteps;
    private Queue<GameObject> footstepsQueue = new Queue<GameObject>();
    private List<FootstepScript> footstepScripts = new List<FootstepScript>();

    private float currentSpawnTimer;

    private void Start()
    {
        currentSpawnTimer = 0;
        for (int i = 0; i < maxFootsteps; ++i)
        {
            GameObject footstep = Instantiate(footstepPrefab);
            footstep.transform.position = transform.position;
            footstepScripts.Add(footstep.GetComponent<FootstepScript>());
            footstep.SetActive(false);
            footstepsQueue.Enqueue(footstep);
        }
    }

    public void NextStep(float xOffset, Vector3 velocity)
    {
        GameObject nextStep = footstepsQueue.Dequeue();
        nextStep.SetActive(true);

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));

        nextStep.transform.rotation = targetRotation;
        nextStep.transform.position = transform.position + new Vector3(xOffset,0,0);
        footstepsQueue.Enqueue(nextStep);
    }

    // tell footsteps to destroy when they fade
    public void ClearPool()
    {
        foreach (var footstep in footstepScripts)
        {
            footstep.destroyOnFade = true;
        }
    }
}
