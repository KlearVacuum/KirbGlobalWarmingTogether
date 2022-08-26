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
    public float mVisionAvoidanceStrength;

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


    // final 2 vision lines will be this angle away from forward
    [SerializeField]
    float maxSteerAngle = 90f;
    // number of vision lines beside the main middle one
    [SerializeField]
    int visionLines = 2;
    float visionRange = 1f;
    private List<Vector3> visionCheckList = new List<Vector3>();

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
    private Vector3 mVisionAvoidanceDir;
    private float visionAngleStep;

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
        visionAngleStep = maxSteerAngle / (float)visionLines * Mathf.Deg2Rad;
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
        rb.AddForce(mCurrentMoveDir * mCurrentMoveForce);
        mCurrentMoveForce = 0;
    }

    private void InitializeVision()
    {
        visionCheckList.Clear();
        visionCheckList.Add(transform.right);
        //Debug.DrawLine(transform.position, transform.position + transform.right, Color.magenta, Time.deltaTime);

        float relativeAngleToRight = Vector2.Angle(transform.right, Vector2.right) * Mathf.Deg2Rad;
        if (Vector2.Dot(transform.right, Vector2.up) < 0) relativeAngleToRight *= -1;

        for (int i = 1; i <= visionLines; ++i)
        {
            float leftVisionX = Mathf.Cos(visionAngleStep * i + relativeAngleToRight);
            float leftVisionY = Mathf.Sin(visionAngleStep * i + relativeAngleToRight);

            float rightVisionX = Mathf.Cos(-visionAngleStep * i + relativeAngleToRight);
            float rightVisionY = Mathf.Sin(-visionAngleStep * i + relativeAngleToRight);

            Vector3 leftVision = new Vector3(leftVisionX, leftVisionY, 0).normalized;
            Vector3 rightVision = new Vector3(rightVisionX, rightVisionY, 0).normalized;

            visionCheckList.Add(leftVision);
            visionCheckList.Add(rightVision);

            // Debug.DrawLine(transform.position, transform.position + leftVision, Color.cyan, Time.deltaTime);
            // Debug.DrawLine(transform.position, transform.position + rightVision, Color.red, Time.deltaTime);
        }
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
        RotateTowardTarget((Vector2)transform.position + rb.velocity);

        Avoidance();
        VisionAvoidanceByTag(gameObject.tag);

        mCurrentMoveDir = (pos - transform.position).normalized;
        // mCurrentMoveDir = transform.right;
        mCurrentMoveForce += mCurrentMoveSpeed * Time.deltaTime;

        // Avoidance effector: go away from other kirbs!
        mCurrentMoveDir += mAvoidanceDir * mAvoidanceStrength;
        mCurrentMoveDir += mVisionAvoidanceDir * mVisionAvoidanceStrength;
        mCurrentMoveDir.Normalize();
    }

    public void MoveTowardTarget()
    {
        RotateTowardTarget((Vector2)transform.position + rb.velocity);
        MoveTowardPos(moveToTarget.position);
    }

    private void Avoidance()
    {
        const float avoidanceDist = 2f;
        const float avoidanceAngleDeg = 135f;
        // Basic avoidance so kirbs dont queue up like poops
        // NOTE: THIS SCUFFED THINGY IS NOT SCALABLE, COME BACK AND RE-WRITE IF KIRBS ARE NO LONGER 1:1 SCALE
        List<AIEntity> nearbyKirbs = GlobalGameData.NearbyAiEntities(transform.position, avoidanceDist);
        if (nearbyKirbs.Count > 0)
        {
            foreach (var kirb in nearbyKirbs)
            {
                float dist = Vector2.Distance(kirb.transform.position, transform.position);
                Vector3 dir = (transform.position - kirb.transform.position).normalized;
                if (Vector2.Angle(transform.forward, dir) * Mathf.Rad2Deg <= avoidanceAngleDeg)
                {
                    // Debug.DrawLine(transform.position, kirb.transform.position, Color.red);
                    mAvoidanceDir += dir * (1 - dist);
                }
            }
            mAvoidanceDir.Normalize();
        }
    }

    private void VisionAvoidanceByTag(string tag)
    {
        InitializeVision();

        float longestDist = 0;
        Vector3 bestDir = Vector3.zero;

        for (int i = 0; i < visionCheckList.Count; ++i)
        {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.3f, visionCheckList[i], visionRange);

            if (hit.collider != null)
            {
                // I see it!
                if (hit.transform.gameObject.CompareTag(tag) || hit.transform.gameObject.CompareTag("Wall"))
                {
                    float dist = Vector2.Distance(transform.position, hit.point);

                    Debug.DrawLine(transform.position, transform.position + (visionCheckList[i] * dist), Color.red);
                    if (dist > longestDist)
                    {
                        // longest dist = best dist
                        longestDist = dist;
                        bestDir = visionCheckList[i];
                    }
                }
                else
                {
                    if (i > 0) bestDir = visionCheckList[i];
                    break;
                }
            }
            // I see nothing here, means nothing is blocking my way for this direction
            // affects nothing if my forward path is clear
            else
            {
                if (i > 0) bestDir = visionCheckList[i];
                break;
            }
        }

        Debug.DrawLine(transform.position, transform.position + bestDir, Color.green);
        mVisionAvoidanceDir = bestDir.normalized;
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