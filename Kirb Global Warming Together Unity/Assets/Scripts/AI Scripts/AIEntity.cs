using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Contains all data and functions of this AI
public class AIEntity : MonoBehaviour
{
    [HideInInspector]
    public bool forceStateTransition;

    [Tooltip("When kirbs are conjested af, quick fix panic button doesn't hurt anyone... right?")]
    public bool panic;
    [Tooltip("Enables ai to continue looking for trash and bringing them to depo: if disabled, ai will not leave depo. Should be true until we start doing the recall horn thingy.")]
    public bool collectTrash;
    [Tooltip("Tells ai to return to depo. True after collecting a trash, false when out looking for trash.")]
    public bool returnToDepo;
    [Tooltip("DIEDED")]
    public bool dead;

    public Animator _animator;

    public float mMoveSpeed;
    public float mRotateSpeed;
    public float mPanicMoveSpeed;
    public float mPanicRotateSpeed;
    public float mPanicDuration;

    Vector3 desiredVelocity;
    [HideInInspector]
    public Vector3 velocityVariance;
    public Vector2 velocityVarStrength;
    public Vector2 velocityVarTimeRange;
    private bool enableVelocityVar;
    IEnumerator velVar;
    [HideInInspector]
    public int depoOverlap;

    [HideInInspector]
    public float mAvoidanceStrength;
    public float mVisionAvoidanceStrength;

    private float mCurrentMoveSpeed;
    private float mCurrentRotateSpeed;
    private float mCurrentPanicDuration;
    private bool mPanicTriggered;
    private float panicMoveTime;
    private Vector2 panicDestination;

    public Transform forwardDir;
    public string trashTag;
    public string depoTag;

    public GameObject deadKirb;

    private TrashScript _heldTrash;

    public float maxTravelTime;
    private float currentTravelTime;

    [HideInInspector]
    public Transform moveToTarget;
    [HideInInspector]
    public int trashCash;

    [HideInInspector]
    public List<GameObject> foundTrashList;
    [HideInInspector]
    public List<GameObject> foundDepoList;

    float avoidanceDist = 2f;
    float avoidanceAngleDeg = 135f;

    // final 2 vision lines will be this angle away from forward
    float maxSteerAngle = 90f;
    // number of vision lines beside the main middle one
    int visionLines = 2;
    float visionRange = 1f;
    float visionCircleRadius;
    private List<Vector3> visionCheckList = new List<Vector3>();

    //protected Animator mAnimator;
    //public Animator animator
    //{
    //    get { return mAnimator; }
    //}
    protected Collider2D mCol;
    public Collider2D col
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
    private float visionWeightStep;

    // death stuff
    [HideInInspector]
    public eDeathType deathType;
    [HideInInspector]
    public int waterOverlap;
    public float timeToDrown;
    private float mCurrentWaterOverlapTime;
    private float currentWaterOverlapTime
    {
        get { return mCurrentWaterOverlapTime; }
        set { mCurrentWaterOverlapTime = Mathf.Clamp(value, 0, mCurrentWaterOverlapTime + 1); }
    }

    public List<eTrashType> trashTypeWeakness;

    protected virtual void Awake()
    {
        // mAnimator = GetComponentInChildren<Animator>();
        mCol = GetComponent<Collider2D>();
        mRB = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        GlobalGameData.AddAiEntity(this);
        mCurrentMoveSpeed = mMoveSpeed;
        mCurrentRotateSpeed = mRotateSpeed;
        visionAngleStep = maxSteerAngle / (float)visionLines * Mathf.Deg2Rad;
        visionWeightStep = 1 / visionLines;

        velVar = NewVelocityVariance(velocityVarTimeRange);
        StartCoroutine(velVar);
        enableVelocityVar = false;

        ResetTravelTime();
        forceStateTransition = false;
    }
    protected virtual void Update()
    {
        currentTravelTime -= Time.deltaTime;

        // TEMP: colors to show feedback on kirb's current state
        if (dead) GetComponent<SpriteRenderer>().color = Color.gray;
        else if (returnToDepo) GetComponent<SpriteRenderer>().color = Color.green;
        else GetComponent<SpriteRenderer>().color = Color.blue;

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

        DrownCheck();
    }

    protected virtual void FixedUpdate()
    {
        rb.AddForce(desiredVelocity * rb.mass);
        desiredVelocity = Vector3.zero;
        // mCurrentMoveForce = 0;
    }

    #region UNUSED FUNCTION: VISION LINES
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
    #endregion

    public GameObject GetNearestVisibleTrash(float searchRadius)
    {
        foundTrashList = GlobalGameData.NearbyTrash(transform.position, searchRadius);
        if (foundTrashList.Count == 0) return null;

        float shortestDist = float.MaxValue;
        GameObject nearestTrash = null;
        foreach (GameObject trash in foundTrashList)
        {
            float dist = Vector2.Distance(transform.position, trash.transform.position);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, trash.transform.position - transform.position, searchRadius);
            if (hit && hit.transform.gameObject == trash)
            {
                if (dist < shortestDist)
                {
                    shortestDist = dist;
                    nearestTrash = trash;
                }
            }
        }
        return nearestTrash;
    }

    public void ResetTravelTime()
    {
        currentTravelTime = maxTravelTime;
    }

    public bool TravelTimeExceeded()
    {
        return currentTravelTime <= 0;
    }

    public GameObject GetNearestVisibleDepo(float searchRadius)
    {
        foundDepoList = GlobalGameData.NearbyDepos(transform.position, searchRadius);
        if (foundDepoList.Count == 0) return null;

        float shortestDist = float.MaxValue;
        GameObject nearestDepo = null;
        foreach (GameObject depo in foundDepoList)
        {
            float dist = Vector2.Distance(transform.position, depo.transform.position);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, depo.transform.position - transform.position, searchRadius);
            if (hit && hit.transform.gameObject == depo)
            {
                if (dist < shortestDist)
                {
                    shortestDist = dist;
                    nearestDepo = depo;
                }
            }
        }
        return nearestDepo;
    }

    // drop the trash into depo
    public void Deposit()
    {
        returnToDepo = false;
        GlobalGameData.cash += trashCash;

        _heldTrash.DoBeDeposited(moveToTarget.position);
        _heldTrash = null;

        _animator.CrossFade("kirb_deposit", 0, 0);
        StartCoroutine(SwitchAnimationAfterDelay("kirb_run", 0.5f));
    }

    public void Die()
    {
        switch (deathType)
        {
            case eDeathType.NASTYFOOD:
                _animator.CrossFade("kirb_kaboom", 0, 0);
                if (_heldTrash != null)
                {
                    GameObject.Destroy(_heldTrash.gameObject);
                    _heldTrash = null;
                }
                break;
            case eDeathType.DROWN:
                _animator.CrossFade("kirb_drowned", 0, 0);
                break;
            default:
                break;
        }
        col.enabled = false;
        GameObject deadGO = Instantiate(deadKirb, transform.position, Quaternion.identity);
        deadGO.GetComponent<TrashScript>().randomRotate = false;
        GlobalGameData.RemoveAiEntity(this);
    }

    // Dampen velocity: must be less than 1
    public void DampenVelocity(float dampenAmount)
    {
        if (dampenAmount >= 1) return;
        rb.velocity = rb.velocity * dampenAmount;
    }

    private void DrownCheck()
    {
        if (waterOverlap > 0) currentWaterOverlapTime += Time.deltaTime;
        else currentWaterOverlapTime -= Time.deltaTime;

        if (currentWaterOverlapTime >= timeToDrown)
        {
            forceStateTransition = true;
            deathType = eDeathType.DROWN;
            dead = true;
        }
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

        Avoidance();
        VisionAvoidanceOtherKirbsAndWalls();

        //mCurrentMoveDir = (pos - transform.position).normalized * mCurrentMoveSpeed;
        // mCurrentMoveDir = transform.right;
        //mCurrentMoveForce += mCurrentMoveSpeed * Time.deltaTime;

        // Avoidance effector: go away from other kirbs!
        //mCurrentMoveDir += mAvoidanceDir * mAvoidanceStrength;
        //mCurrentMoveDir += mVisionAvoidanceDir * mVisionAvoidanceStrength;
        //mCurrentMoveDir.Normalize();

        desiredVelocity = (pos - transform.position).normalized;
        if (enableVelocityVar) desiredVelocity += velocityVariance;
        desiredVelocity *= mCurrentMoveSpeed;
        // avoidance
        desiredVelocity += mVisionAvoidanceDir * mVisionAvoidanceStrength;

        if (desiredVelocity.magnitude > mCurrentMoveSpeed) desiredVelocity = desiredVelocity.normalized * mCurrentMoveSpeed;

        // RotateTowardTarget(transform.position + desiredVelocity);

        // Debug.DrawLine(transform.position, transform.position + desiredVelocity, Color.magenta);
    }

    public void EnableVelVar(bool enable)
    {
        enableVelocityVar = enable;
    }

    IEnumerator NewVelocityVariance(Vector2 timeRange)
    {
        yield return new WaitForSeconds(Random.Range(timeRange.x, timeRange.y));
        velocityVariance = new Vector3(Random.Range(-velocityVarStrength.x, velocityVarStrength.x), 
                                        Random.Range(-velocityVarStrength.y, velocityVarStrength.y), 0);
        velVar = NewVelocityVariance(timeRange);
        StartCoroutine(velVar);
    }

    public void MoveTowardTarget()
    {
        // RotateTowardTarget(desiredVelocity);
        MoveTowardPos(moveToTarget.position);
    }

    private void Avoidance()
    {
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

    // Solving very specific problems made this function abit of a monster opps
    private void VisionAvoidanceOtherKirbsAndWalls()
    {
        Vector3 avoidanceVector = Vector3.zero;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, visionRange);

        for (int i = 0; i < colliders.Length; ++i)
        {
            var collider = colliders[i];

            // I see it!
            if (!returnToDepo && collider.transform.gameObject.CompareTag(gameObject.tag))
            {
                // my current direction's weight
                float weight = (int)(i / 2) * visionWeightStep;
                // get the other kirb's intended direction
                Vector3 otherPos = collider.transform.position;
                Vector3 otherDir = (transform.position - otherPos).normalized;

                weight *= Vector2.Distance(transform.position, otherPos);

                avoidanceVector += weight * otherDir;
                // Debug.DrawLine(transform.position, transform.position + avoidanceVector, Color.red);
            }

            // wall
            else if (collider.transform.gameObject.CompareTag("Wall"))
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position,
                                    (collider.transform.position - transform.position).normalized,
                                    visionRange, gameObject.layer);
                avoidanceVector += (Vector3)hit.normal;
            }
        }

        // Debug.DrawLine(transform.position, transform.position + avoidanceVector, Color.green);
        mVisionAvoidanceDir = avoidanceVector;

        #region AVOIDANCE VERSION 2
        //InitializeVision();

        //Vector3 avoidanceVector = Vector3.zero;

        //for (int i = 0; i < visionCheckList.Count; ++i)
        //{
        //    RaycastHit2D hit = Physics2D.CircleCast(transform.position, visionCircleRadius, visionCheckList[i], visionRange, gameObject.layer);

        //    if (hit.collider != null)
        //    {
        //        // I see it!
        //        if (hit.transform.gameObject.CompareTag(gameObject.tag))
        //        {
        //            // my current direction's weight
        //            float weight = (int)(i / 2) * visionWeightStep;
        //            // get the other kirb's intended direction
        //            Vector3 otherDir = hit.transform.right;
        //            Vector3 otherPos = hit.transform.position;

        //            // if dot product <= 0, affect the weight
        //            float dotProduct = Vector2.Dot(visionCheckList[i], transform.position - otherPos);
        //            if (dotProduct < 0)
        //            {
        //                dotProduct = Mathf.Abs(dotProduct);
        //                weight *= dotProduct;
        //            }

        //            avoidanceVector += weight * otherDir;
        //            Debug.DrawLine(transform.position, transform.position + visionCheckList[i], Color.red);
        //        }
        //        // wall
        //        else if (hit.transform.gameObject.CompareTag("Wall"))
        //        {
        //            avoidanceVector += (Vector3)hit.normal;
        //        }
        //    }
        //}
        //Debug.DrawLine(transform.position, transform.position + avoidanceVector, Color.green);
        //mVisionAvoidanceDir = avoidanceVector;
        #endregion

        #region BEST PATH IS LONGEST PATH: OLD
        //float longestDist = 0;
        //Vector3 bestDir = Vector3.zero;

        //for (int i = 0; i < visionCheckList.Count; ++i)
        //{
        //    RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.3f, visionCheckList[i], visionRange);

        //    if (hit.collider != null)
        //    {
        //        // I see it!
        //        if (hit.transform.gameObject.CompareTag(gameObject.tag))
        //        {
        //            float dist = Vector2.Distance(transform.position, hit.point);

        //            Debug.DrawLine(transform.position, transform.position + (visionCheckList[i] * dist), Color.red);
        //            if (dist > longestDist)
        //            {
        //                // longest dist = best dist
        //                longestDist = dist;
        //                bestDir = visionCheckList[i];
        //            }
        //        }
        //        else if (hit.transform.gameObject.CompareTag("Wall"))
        //        {

        //        }
        //        else
        //        {
        //            if (i > 0) bestDir = visionCheckList[i];
        //            break;
        //        }
        //    }
        //    // I see nothing here, means nothing is blocking my way for this direction
        //    // affects nothing if my forward path is clear
        //    else
        //    {
        //        if (i > 0) bestDir = visionCheckList[i];
        //        break;
        //    }
        //}

        //Debug.DrawLine(transform.position, transform.position + bestDir, Color.green);
        //mVisionAvoidanceDir = bestDir.normalized;
        #endregion
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

            if (trashTypeWeakness != null && trashTypeWeakness.Count != 0)
            {
                foreach (eTrashType trashType in trashTypeWeakness)
                {
                    if (trash.trashType == trashType)
                    {
                        forceStateTransition = true;
                        deathType = eDeathType.NASTYFOOD;
                        dead = true;
                        break;
                    }
                }
            }
            
            _animator.CrossFade("kirb_suck", 0, 0);
            StartCoroutine(SwitchAnimationAfterDelay("kirb_suck_run", 1.0f));

            _heldTrash = trash;
            trash.RemoveTrash(transform);
            returnToDepo = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (moveToTarget != null &&
            collision.transform == moveToTarget &&
            collision.gameObject.CompareTag(trashTag))
        {
            var trash = collision.gameObject.GetComponent<TrashScript>();
            trashCash = trash.trashCash;

            _animator.CrossFade("kirb_suck", 0, 0);
            StartCoroutine(SwitchAnimationAfterDelay("kirb_suck_run", 1.0f));

            _heldTrash = trash;
            trash.RemoveTrash(transform);
            returnToDepo = true;
        }
    }

    IEnumerator SwitchAnimationAfterDelay(string clipName, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!dead)
        {
            _animator.CrossFade(clipName, 0, 0);
        }
    }
}