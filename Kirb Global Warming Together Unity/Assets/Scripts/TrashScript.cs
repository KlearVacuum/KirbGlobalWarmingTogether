using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashScript : MonoBehaviour
{
    public eTrashType trashType;
    public int trashCash;

    public List<Sprite> sprites = new List<Sprite>();

    public bool _isYumisTrash, _isHeld;
    public bool randomRotate = true;

    private Transform _targetTransform;
    public Vector3 _target;
    private Vector3 _current, velocity;
    [SerializeField] private float _dampRatio = 0.5f, _angular = 0.5f;

    private bool shrink;
    private Vector3 startingScale;

    void Start()
    {
        GlobalGameData.AddTrash(gameObject);
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, 1000) % sprites.Count];
        startingScale = transform.localScale;

        if (randomRotate)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-179f, 179f)));
        }
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

        if (shrink && transform.localScale.magnitude > 0)
        {
            transform.localScale -= new Vector3(Time.deltaTime, Time.deltaTime, 0);
            if (transform.localScale.x <= 0 || transform.localScale.y <= 0)
            {
                transform.localScale = Vector3.zero;
                shrink = false;
            }
        }
    }

    public void RemoveTrash(Transform holder)
    {
        // Debug.Log("trash is picked up");
        GlobalGameData.RemoveTrash(gameObject);

        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().freezeRotation = true;
        // _target = newTarget;
        _isYumisTrash = true;
        _isHeld = true;
        _targetTransform = holder;
        _current = transform.position;

        Animator animator = transform.GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }

        // transform.localScale = Vector3.zero;
        shrink = true;
        // Destroy(gameObject);
    }

    public void DoBeDeposited(Vector3 newTarget)
    {
        _isHeld = false;
        _targetTransform = null;
        _target = newTarget;
        shrink = false;
        _current = transform.position - new Vector3(0,0.15f,0);
        transform.localScale = startingScale;
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