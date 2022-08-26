using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Contains all data and functions of this AI
public class AIEntity : MonoBehaviour
{
    [Tooltip("When kirbs are conjested af, quick fix panic button doesn't hurt anyone... right?")]
    public bool panic;
    [Tooltip("Enables ai to continue looking for trash and bringing them to depo: if disabled, ai will not leave depo. Should be true until we start doing the recall horn thingy.")]
    public bool collectTrash;
    [Tooltip("Tells ai to return to depo. True after collecting a trash, false when out looking for trash.")]
    public bool returnToDepo;

    public float mMoveSpeed;
    public float mRotateSpeed;
    public float mPanicMoveSpeed;
    public float mPanicRotateSpeed;
    public float mPanicDuration;

    public float mAvoidanceStrength;

    private float mCurrentMoveSpeed;
    private float mCurrentRotateSpeed;
    private float mCurrentPanicDuration;
    private bool mPanicTriggered;
    private float panicMoveTime;
    private Vector2 panicDestination;

    public string trashTag;

    [HideInInspector]
    public Transform moveToTarget;
    [HideInInspector]
    public int trashCash;

    [HideInInspector]
    public List<GameObject> foundTrashList;
    [HideInInspector]
    public List<GameObject> foundDepoList;

    //protected Animator mAnimator;
    //public Animator animator
    //{
    //    get { return mAnimator; }
    //}
    protected Collider mCol;
    public Collider col
    {
        get { return mCol; }
    }

    protected Rigidbody2D mRB;
    public Rigidbody2D rb
    {
        get { return mRB; }
    }

    private float mCurrentMoveForce;
    private Vector3 mCurrentMoveDir;
    private Vector3 mAvoidanceDir;

    protected virtual void Awake()
    {
        // mAnimator = GetComponentInChildren<Animator>();
        mCol = GetComponent<Collider>();
        mRB = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        GlobalGameData.AddAiEntity(this);
        mCurrentMoveSpeed = mMoveSpeed;
        mCurrentRotateSpeed = mRotateSpeed;
    }
    protected virtual void Update()
    {
        if (!mPanicTriggered)
        {
            // panic button pressed!
            if (panic)
            {
                mCurrentPanicDuration = mPanicDuration;
                mCurrentMoveSpeed = mPanicMoveSpeed;
                mCurrentRotateSpeed = mPanicRotateSpeed;

                mPanicTriggered = true;
            }
        }
        // timer for kirbs to panic
        if (panic)
        {
            mCurrentPanicDuration -= Time.deltaTime;
            if (mCurrentPanicDuration <= 0)
            {
                mCurrentMoveSpeed = mMoveSpeed;
                mCurrentRotateSpeed = mRotateSpeed;
                panic = false;
                mPanicTriggered = false;
            }
        }
    }

    protected virtual void FixedUpdate()
    {
        // Avoidance effector: go away from other kirbs!
        mCurrentMoveDir += mAvoidanceDir * mAvoidanceStrength;
        mCurrentMoveDir.Normalize();

        rb.AddForce(mCurrentMoveDir * mCurrentMoveForce);

        mAvoidanceDir = Vector3.zero;
        mCurrentMoveForce = 0;
    }

    public GameObject GetNearestTrash(float searchRadius)
    {
        foundTrashList = GlobalGameData.NearbyTrash(transform.position, searchRadius);
        if (foundTrashList.Count == 0) return null;

        float shortestDist = float.MaxValue;
        GameObject nearestTrash = null;
        foreach (GameObject trash in foundTrashList)
        {
            float dist = Vector2.Distance(transform.position, trash.transform.position);
            if (dist < shortestDist)
            {
                shortestDist = dist;
                nearestTrash = trash;
            }
        }
        return nearestTrash;
    }

    public GameObject GetNearestDepo(float searchRadius)
    {
        foundDepoList = GlobalGameData.NearbyDepos(transform.position, searchRadius);
        if (foundDepoList.Count == 0) return null;

        float shortestDist = float.MaxValue;
        GameObject nearestDepo = null;
        foreach (GameObject depo in foundDepoList)
        {
            float dist = Vector2.Distance(transform.position, depo.transform.position);
            if (dist < shortestDist)
            {
                shortestDist = dist;
                nearestDepo = depo;
            }
        }
        return nearestDepo;
    }

    // drop the trash into depo
    public void Deposit()
    {
        returnToDepo = false;
        GlobalGameData.cash += trashCash;
    }

    public void PanicRandomMoveToTargets(Vector2 minMaxMoveRange, Vector2 minMaxTime)
    {
        panicMoveTime -= Time.deltaTime;
        if (panicMoveTime <= 0)
        {
            // find new random destination
            float rangeDiff = minMaxMoveRange.y - minMaxMoveRange.x;
            float randX = Random.Range(minMaxMoveRange.x, minMaxMoveRange.y) - rangeDiff;
            float randY = Random.Range(minMaxMoveRange.x, minMaxMoveRange.y) - rangeDiff;
            panicDestination = new Vector3(randX, randY) + transform.position;
            panicMoveTime = Random.Range(minMaxTime.x, minMaxTime.y);
        }

        MoveTowardPos(panicDestination);
    }

    public void MoveTowardPos(Vector3 pos)
    {
        RotateTowardTarget(pos);
        mCurrentMoveDir = (pos - transform.position).normalized;
        mCurrentMoveForce += mCurrentMoveSpeed * Time.deltaTime;
        Avoidance();
    }

    public void MoveTowardTarget()
    {
        RotateTowardTarget(moveToTarget.position);
        MoveTowardPos(moveToTarget.position);
    }

    private void Avoidance()
    {
        const float avoidanceDist = 1.5f;
        const float avoidanceAngleDeg = 120;
        // Basic avoidance so kirbs dont queue up like poops
        // NOTE: THIS SCUFFED THINGY IS NOT SCALABLE, COME BACK AND RE-WRITE IF KIRBS ARE NO LONGER 1:1 SCALE
        List<AIEntity> nearbyKirbs = GlobalGameData.NearbyAiEntities(transform.position, avoidanceDist);
        if (nearbyKirbs.Count > 0)
        {
            foreach (var kirb in nearbyKirbs)
            {
                float dist = Vector2.Distance(kirb.transform.position, transform.position);
                Vector3 dir = (transform.position - kirb.transform.position).normalized;
                if (Vector2.Angle(transform.forward, dir)*Mathf.Rad2Deg <= avoidanceAngleDeg)
                {
                    mAvoidanceDir += dir * dist;
                }
            }
            mAvoidanceDir.Normalize();
        }
    }

    public void RotateTowardTarget(Vector3 target)
    {
        float angle = Mathf.Atan2(target.y - transform.position.y, target.x - transform.position.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, mCurrentRotateSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (moveToTarget != null &&
            collision.transform == moveToTarget &&
            collision.gameObject.CompareTag(trashTag))
        {
            var trash = collision.gameObject.GetComponent<TrashScript>();
            trashCash = trash.trashCash;
            trash.RemoveTrash();
            returnToDepo = true;
        }
    }
}