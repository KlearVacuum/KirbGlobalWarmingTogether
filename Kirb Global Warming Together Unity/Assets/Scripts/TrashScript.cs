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
    public float fadeTimeWhenDeposited = 10f;
    private float currentFadeTimeWhenDeposited;

    private Transform _targetTransform;
    public Vector3 _target;
    private Vector3 _current, velocity;
    [SerializeField] private float _dampRatio = 0.5f, _angular = 0.5f;

    private bool shrink;
    private bool deposited;
    private Vector3 startingScale;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        deposited = false;
        if (fadeTimeWhenDeposited <= 0) fadeTimeWhenDeposited = 0.01f;
        currentFadeTimeWhenDeposited = fadeTimeWhenDeposited;
        GlobalGameData.AddTrash(gameObject);
        spriteRenderer.sprite = sprites[Random.Range(0, 1000) % sprites.Count];
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
            SpringMath.Lerp(ref _current, ref velocity, _target, _dampRatio, _angular, Time.deltaTime * 80f);
            transform.position = _current;
        }

        if (shrink && transform.localScale.magnitude > 0)
        {
            Vector3 newScale = transform.localScale - new Vector3(Time.deltaTime, Time.deltaTime, 0);
            
            if (newScale.x <= 0 || newScale.y <= 0)
            {
                transform.localScale = Vector3.zero;
                shrink = false;
            }
            else
            {
                transform.localScale = newScale;
            }
        }
        else if (deposited)
        {
            currentFadeTimeWhenDeposited -= Time.deltaTime;
            if (currentFadeTimeWhenDeposited < 0.001f) currentFadeTimeWhenDeposited = 0.001f;

            float ratio = currentFadeTimeWhenDeposited / fadeTimeWhenDeposited;
            // spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, ratio);
            transform.localScale = startingScale * ratio;

            if (currentFadeTimeWhenDeposited <= 0.001f)
            {
                Destroy(gameObject);
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
        _current = transform.position - new Vector3(0,0.1f,0);
        transform.localScale = startingScale;
        deposited = true;
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