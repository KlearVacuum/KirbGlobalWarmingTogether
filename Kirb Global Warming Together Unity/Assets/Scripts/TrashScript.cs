using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashScript : MonoBehaviour
{
    public eTrashType trashType;
    public int trashCash;

    public bool _isYumisTrash;

    private Vector3 _current, _target, velocity;
    [SerializeField] private float _dampRatio = 1.0f, _angular = 0.3f, _timeStep = 0.5f;

    void Start()
    {
        GlobalGameData.AddTrash(gameObject);
    }

    void Update()
    {
        if (_isYumisTrash)
        {
            _current = transform.position;
            _target = GameObject.FindObjectOfType<AIEntity>().transform.position;
            SpringMath.Lerp(ref _current, ref velocity, _target, _dampRatio, _angular, _timeStep);
            transform.position = _current;
        }
    }

    public void RemoveTrash()
    {
        // Debug.Log("trash is gone");
        GlobalGameData.RemoveTrash(gameObject);
        Destroy(gameObject);
    }
}

public enum eTrashType
{
    General,
    Plastic,
    Metal,
    Glass,

    None
}