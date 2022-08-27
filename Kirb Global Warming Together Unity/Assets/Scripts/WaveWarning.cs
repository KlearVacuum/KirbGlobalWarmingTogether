using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveWarning : MonoBehaviour
{
    [SerializeField] private LevelManager _levelManager = null;
    [SerializeField] private Image _warningImage = null;

    private void Awake() 
    {
        Debug.Assert(_levelManager != null, "_levelManager is not assigned!");     
    }

    private void Start() 
    {
        _levelManager.onWaveWarning.AddListener(OnWaveWarning);
        _levelManager.onWaveStart.AddListener(OnWaveStart);

        _warningImage.gameObject.SetActive(false);
    }

    private void OnWaveWarning()
    {
        _warningImage.gameObject.SetActive(true);
    }

    private void OnWaveStart()
    {
        _warningImage.gameObject.SetActive(false);
    }
}
