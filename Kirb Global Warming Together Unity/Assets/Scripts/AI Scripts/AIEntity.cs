using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Contains all data and functions of this AI
public class AIEntity : MonoBehaviour
{
    [HideInInspector]
    public bool forceStateTransition;

    // [Header("Behaviour bools")]
    [Tooltip("When kirbs are conjested af, quick fix panic button doesn't hurt anyone... right?")]
    public bool panic;
    [Tooltip("Enables ai to continue looking for trash and bringing them to depo: if disabled, ai will not leave depo. Should be true until we start doing the recall horn thingy.")]
    public bool collectTrash;
    [Tooltip("Tells ai to return to depo. True after collecting a trash, false when out looking for trash.")]
    public bool returnToDepo;
    [Tooltip("DIEDED")]
    public bool dead;

    public Animator _animator;
    public FootstepsSpawnPool footstepSpawner;
    public GameObject myShadowPrefab;
    public TrashScript trashRef;
    private GameObject myShadow;

    [Header("Movement Info")]
    public float mMoveSpeed;
    public float mRotateSpeed;
    public float mPanicMoveSpeed;
    public float mPanicRotateSpeed;
    public float mPanicDuration;
    public float mPanicStrength;
    public float mStopCollectingTrashDuration;

    [Tooltip("Time kirb waits after executing an action before being able to do the next action")]
    public float depositActionDelayTime;
    public float suckActionDelayTime;

    // When crowned, kirb gives more cash per trash
    [Header("Crown Info")]
    public GameObject crownGO;
    public List<Crown> myCrowns = new List<Crown>();
    public bool crowned;
    public float mCrownThreshold;
    public float mTrashCashMult;
    public float crownByTimeWeightage;
    public float crownByTrashCostWeightage;
    private float mCurrCrownProgress;

    [Header("Velocity Variance Info")]
    Vector3 desiredVelocity;
    [HideInInspector]
    public Vector3 velocityVariance;
    public Vector2 velocityVarStrength;
    public Vector2 velocityVarTimeRange;
    private bool enableVelocityVar;
    IEnumerator velVar;
    [HideInInspector]
    public int depoOverlap;

    [Header("Avoidance Info")]
    [HideInInspector]
    public float mAvoidanceStrength;
    public float mVisionAvoidanceStrength;

    [HideInInspector]
    public float mCurrentMoveSpeed;
    private float mCurrentRotateSpeed;
    private float mCurrentPanicDuration;
    private bool mPanicTriggered;
    private float panicMoveTime;
    private Vector2 panicDestination;
    private bool mStopCollectingTrashTriggered;
    private float mCurrentStopCollectingTrashDuration;
    private float mActionDelayTimer;

    [Header("Dependencies")]
    public Transform forwardDir;
    public GameObject stopCollectTrashIndicator;
    public GameObject panicIndicator;
    public ParticleSystem eatParticles;
    public float eatParticlesDelay;
    public string trashTag;
    public string depoTag;

    public GameObject deadKirb;
    [HideInInspector]
    public TrashScript _heldTrash;

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
    protected SpriteRenderer mSpriteRenderer;
    private float lookOtherWayTime;

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
    
    public AudioSource _asource;
    public AudioClip _aclipEat, _aclipBweh, _aclipDrown, _aclipPop;

    public string description;
    public string crownDescription;

    [HideInInspector]
    public float superTimer;
    // Create adaptive timer system, to test super crown

    protected virtual void Awake()
    {
        // mAnimator = GetComponentInChildren<Animator>();
        mCol = GetComponent<Collider2D>();
        mRB = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        mSpriteRenderer = GetComponent<SpriteRenderer>();
        _asource = GetComponent<AudioSource>();
    }

    protected virtual void Start()
    {
        GlobalGameData.AddAiEntity(this);
        crowned = false;
        crownGO.SetActive(false);

        myShadow = Instantiate(myShadowPrefab, transform.position, Quaternion.identity);
        KirbShadowScript shadowScript = myShadow.GetComponent<KirbShadowScript>();
        shadowScript.followTransform = transform;
        shadowScript.offset = new Vector3(0, -0.75f, 0);

        GetComponent<TrashScript>().CopyAllValues(trashRef);

        mCurrentMoveSpeed = mMoveSpeed;
        mCurrentRotateSpeed = mRotateSpeed;
        visionAngleStep = maxSteerAngle / (float)visionLines * Mathf.Deg2Rad;
        visionWeightStep = 1 / visionLines;

        velVar = NewVelocityVariance(velocityVarTimeRange);
        StartCoroutine(velVar);
        enableVelocityVar = false;

        ResetTravelTime();
        forceStateTransition = false;

        _animator.Play("run");

        stopCollectTrashIndicator.SetActive(false);
        panicIndicator.SetActive(false);
    }
    protected virtual void Update()
    {
        if (!dead)
        {
            currentTravelTime -= Time.deltaTime;

            if (!mPanicTriggered)
            {
                // panic button pressed!
                if (panic)
                {
                    mCurrentPanicDuration = mPanicDuration + Random.Range(-1f, 1f);
                    mCurrentMoveSpeed = mPanicMoveSpeed;
                    mCurrentRotateSpeed = mPanicRotateSpeed;
                    panicIndicator.SetActive(true);
                    mPanicTriggered = true;
                    forceStateTransition = true;
                }
            }
            // timer for kirbs to panic
            if (panic)
            {
                mCurrentPanicDuration -= Time.deltaTime;
                if (mCurrentPanicDuration <= 0)
                {
                    panicIndicator.SetActive(false);
                    mCurrentMoveSpeed = mMoveSpeed;
                    mCurrentRotateSpeed = mRotateSpeed;
                    panic = false;
                    mPanicTriggered = false;
                }
            }

            if (!collectTrash)
            {
                if (!mStopCollectingTrashTriggered)
                {
                    mCurrentStopCollectingTrashDuration = mStopCollectingTrashDuration + Random.Range(-1f, 1f);
                    returnToDepo = true;
                    forceStateTransition = true;
                    mStopCollectingTrashTriggered = true;
                    stopCollectTrashIndicator.SetActive(true);
                }
                mCurrentStopCollectingTrashDuration -= Time.deltaTime;

                if (mCurrentStopCollectingTrashDuration <= 0)
                {
                    stopCollectTrashIndicator.SetActive(false);
                    mStopCollectingTrashTriggered = false;
                    collectTrash = true;
                    if (_heldTrash == null)
                    {
                        returnToDepo = false;
                    }
                }
            }

            if (!crowned)
            {
                if (mCurrCrownProgress < mCrownThreshold) mCurrCrownProgress += Time.deltaTime * crownByTimeWeightage;
                else
                {
                    crownGO.SetActive(true);
                    crowned = true;
                    foreach (Crown crown in myCrowns) crown.OnCrowned(this);
                }
            }
            else
            {
                foreach (Crown crown in myCrowns) { crown.CrownedUpdate(this); }
            }

            mActionDelayTimer -= Time.deltaTime;

            DrownCheck();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (LevelManager.Instance != null && LevelManager.Instance.GameState != GameState.Playing) 
        {
            desiredVelocity = Vector3.zero;
        }

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
        Vector2 depoPos = Vector2.zero;

        if (_heldTrash != null)
        {
            GlobalGameData.cash += (int)((float)trashCash * mTrashCashMult);
            // crowned progress per deposit
            if (!crowned)
            {
                mCurrCrownProgress += (float)trashCash * crownByTrashCostWeightage;
            }

            // dont move to another depo if you're standing on one, or you'll spit it across the world
            List<GameObject> nearbyDepos = GlobalGameData.NearbyDepos(transform.position, 3f);
            if (nearbyDepos.Count > 0)
            {
                depoPos = nearbyDepos[0].transform.position;
                _heldTrash.DoBeDeposited(depoPos);
            }
            else
            {
                depoPos = moveToTarget.position;
                _heldTrash.DoBeDeposited(moveToTarget.position);
            }

            FlipToLookAtTarget(depoPos);
            _heldTrash = null;
            ActionExecuted(depositActionDelayTime);
            _asource.PlayOneShot(_aclipBweh);
            _animator.CrossFade("deposit", 0, 0);
            StartCoroutine(SwitchAnimationAfterDelay("run", depositActionDelayTime));
        }
    }

    private void FlipToLookAtTarget(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        float dot = Vector3.Dot(dir, Vector2.right);

        // flip sprite if target is to the left
        mSpriteRenderer.flipX = dot < 0;
        lookOtherWayTime = 0;
    }

    public void Die()
    {
        panicIndicator.SetActive(false);
        stopCollectTrashIndicator.SetActive(false);
        Destroy(myShadow);
        footstepSpawner.ClearPool();
        crownGO.SetActive(false);

        switch (deathType)
        {
            case eDeathType.NASTYFOOD:
                _animator.CrossFade("kaboom", 0, 0);
                if (_heldTrash != null)
                {
                    Destroy(_heldTrash.gameObject);
                    _heldTrash = null;
                    _asource.PlayOneShot(_aclipPop);
                }
                col.enabled = false;
                break;
            case eDeathType.DROWN:
                _animator.CrossFade("drown", 0, 0);
                _asource.PlayOneShot(_aclipDrown);
                if (_heldTrash != null)
                {
                    Destroy(_heldTrash.gameObject);
                    _heldTrash = null;
                }

                // Special kirb death
                foreach (Crown crown in myCrowns) { crown.CrownedOnDeath(this); }

                GetComponent<TrashScript>().enabled = true;
                mSpriteRenderer.color = mSpriteRenderer.color * new Vector4(0.7f, 0.7f, 0.7f, 1);
                gameObject.tag = "Trash";
                gameObject.transform.rotation = Quaternion.identity;
                GlobalGameData.AddTrash(gameObject);
                mSpriteRenderer.sortingOrder = -1;
                break;
            default:
                col.enabled = false;
                break;
        }
        if (GlobalGameData.allAiEntities.Contains(this)) 
        {
            NotifyLastDead();
        }
        GlobalGameData.RemoveAiEntity(this);
    }

    public void ActionExecuted(float delayTime)
    {
        mActionDelayTimer = delayTime;
    }

    public bool ActionReady()
    {
        return mActionDelayTimer <= 0;
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

            Vector3 randEffector = new Vector3(randX, randY).normalized;
            panicDestination = new Vector3(0, -2) + transform.position + randEffector * mPanicStrength;
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
        // mSpriteRenderer.flipX = desiredVelocity.x < 1;

        // looking left
        if (mSpriteRenderer.flipX)
        {
            if (rb.velocity.x > 0.05f)
            {
                lookOtherWayTime += Time.deltaTime;
            }
            else
            {
                lookOtherWayTime = 0;
            }
            if (lookOtherWayTime > 0.2f)
            {
                lookOtherWayTime = 0;
                mSpriteRenderer.flipX = false;
            }
        }

        // looking right
        if (!mSpriteRenderer.flipX)
        {
            if (rb.velocity.x < -0.05f)
            {
                lookOtherWayTime += Time.deltaTime;
            }
            else
            {
                lookOtherWayTime = 0;
            }
            if (lookOtherWayTime > 0.2f)
            {
                lookOtherWayTime = 0;
                mSpriteRenderer.flipX = true;
            }
        }
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
        if (moveToTarget != null)
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

    private void SpawnDeadBodyTrash()
    {
        GameObject deadGO = Instantiate(deadKirb, transform.position, Quaternion.identity);
        deadGO.GetComponent<TrashScript>().randomRotate = false;
        deadGO.GetComponent<SpriteRenderer>().color = mSpriteRenderer.color;
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
        if (ActionReady() && 
            moveToTarget != null &&
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

            FlipToLookAtTarget(collision.transform.position);
            ActionExecuted(suckActionDelayTime);
            StartCoroutine(EatFeedbackAfterDelay(collision.transform.position, eatParticlesDelay));
            _animator.CrossFade("suck", 0, 0);
            StartCoroutine(SwitchAnimationAfterDelay("suck_run", suckActionDelayTime));

            _heldTrash = trash;
            trash.RemoveTrash(transform);
            StartCoroutine(ReturnToDepoAfterDelay(0.2f));
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (ActionReady() &&
            moveToTarget != null &&
            collision.transform == moveToTarget &&
            collision.gameObject.CompareTag(trashTag))
        {
            var trash = collision.gameObject.GetComponent<TrashScript>();
            trashCash = trash.trashCash;

            FlipToLookAtTarget(collision.transform.position);
            ActionExecuted(suckActionDelayTime);
            _animator.CrossFade("suck", 0, 0);
            StartCoroutine(SwitchAnimationAfterDelay("suck_run", suckActionDelayTime));

            _heldTrash = trash;
            trash.RemoveTrash(transform);
            StartCoroutine(ReturnToDepoAfterDelay(0.2f));
        }
    }

    private void NotifyLastDead() 
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.NotifyLastDead(this.gameObject);
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

    IEnumerator EatFeedbackAfterDelay(Vector3 targetPos, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!dead)
        {
            _asource.PlayOneShot(_aclipEat);
            var particlesMain = eatParticles.main;
            particlesMain.startColor = mSpriteRenderer.color;

            Vector3 trashDir = (targetPos - transform.position).normalized;
            eatParticles.transform.position = gameObject.transform.position + trashDir * 0.75f;

            eatParticles.Play();
        }
    }

    IEnumerator ReturnToDepoAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        moveToTarget = null;
        returnToDepo = true;
    }

    public void StopCollectingTrash()
    {
        if (dead) return;
        if (collectTrash)
        {
            collectTrash = false;
        }
        else
        {
            mCurrentStopCollectingTrashDuration = mStopCollectingTrashDuration + Random.Range(-1f, 1f);
            returnToDepo = true;
            forceStateTransition = true;
            mStopCollectingTrashTriggered = true;
            stopCollectTrashIndicator.SetActive(true);
        }
    }

    public void SpawnNextFootstep(float xOffset)
    {
        if (mSpriteRenderer.flipX) xOffset *= -1;
        footstepSpawner.NextStep(xOffset, rb.velocity);
    }

    private void OnMouseDown()
    {
        StopCollectingTrash();
    }
}