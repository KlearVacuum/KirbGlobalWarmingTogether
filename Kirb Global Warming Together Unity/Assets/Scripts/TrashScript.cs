using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashScript : MonoBehaviour
{
    public eTrashType trashType;
    public int trashCash;

    public List<Sprite> sprites = new List<Sprite>();

    public bool _isYumisTrash, _isHeld;

    private Transform _targetTransform;
    public Vector3 _target;
    private Vector3 _current, velocity;
    [SerializeField] private float _dampRatio = 0.5f, _angular = 0.5f;

    void Start()
    {
        GlobalGameData.AddTrash(gameObject);
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, 1000) % sprites.Count];
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-179f, 179f)));
    }

    void Update()
    {
        if (_isYumisTrash)
        {
            if (_isHeld && _targetTransform != null)
            {
                _target = _targetTransform.position;
            }
            // _current = transform.position;
            // _target = GameObject.FindObjectOfType<AIEntity>().transform.position;
            SpringMath.Lerp(ref _current, ref velocity, _target, _dampRatio, _angular, 0.1f);
            transform.position = _current;
        }
    }

    public void RemoveTrash(Transform holder)
    {
        Debug.Log("trash is picked up");
        GlobalGameData.RemoveTrash(gameObject);

        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().freezeRotation = true;
        // _target = newTarget;
        _isYumisTrash = true;
        _isHeld = true;
        _targetTransform = holder;
        _current = transform.position;
        // Destroy(gameObject);
    }

    public void DoBeDeposited(Vector3 newTarget)
    {
        _isHeld = false;
        _targetTransform = null;
        _target = newTarget;
        _current = transform.position;
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