using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LoseSequence : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera mDeathFollowCam;

    private void Awake() {
        Debug.Assert(mDeathFollowCam != null, "mDeathFollowCam is not assigned!");
    }

    public void OnLose() 
    {
        mDeathFollowCam.Priority += 2;
        mDeathFollowCam.Follow = LevelManager.Instance.LastDead.transform;
    }
}
