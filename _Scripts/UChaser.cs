using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UChaser : URusher 
{
    [SerializeField] Transform target;

    // if radius is negative, the offset must be predefined
    public float radius = 5;
    public float radiusVariation = 1f;
    public float minXDist = 2;
    [SerializeField] private float actualRadius;
    public Vector2 additiveOffset; // an offset to the offset - applies regardless of randomness
    [SerializeField] Vector2 offset;
    public Vector2 targetWorld;
    public Vector2 lookDirection = new Vector2(1, 0);
    

    public float travelTimeRandomness = .5f; // +/- with unit's speed to make slightly faster or slower animations

    public float hoverDistance = .5f;
    public float distanceRandomness = .2f;
    public float hoverFrequency = 2;
    public float frequencyRandomness = .5f;

    private float actualDistance, actualFrequency, actualTravelTime, flightSlope;
    
    Tweener startTween;
    Tweener floatTween;
    public Ease ease;


    // Start is called before the first frame update
    public override void Start() {
        base.Start();

        // apply randomness
        actualDistance = hoverDistance + Random.Range(-distanceRandomness, distanceRandomness);
        actualFrequency = hoverFrequency + Random.Range(-frequencyRandomness, frequencyRandomness);
        actualTravelTime = moveSpeed + Random.Range(-travelTimeRandomness, travelTimeRandomness);


        // assume your target is the castle
        if (target == null && GetComponent<Unit>() != null) {
            target = Castle.castle.transform;
        }

        // if radius is negative, the offset must be predefined
        if (radius > 0) {
            
            // define offset
            actualRadius = radius + Castle.castle.safeRadius + Random.Range(-radiusVariation, radiusVariation);
            offset.x = Random.Range(minXDist, actualRadius);
            offset.y = Mathf.Sqrt(
                Mathf.Pow(actualRadius, 2) - Mathf.Pow(offset.x, 2)
                );

            if (offset.y == float.NaN) offset.y = targetWorld.y;
            if (offset.y == 0) offset.y = .0001f;

            /// determine which corner (+/- x or +/- y) is closest to spawn point
            offset.x = (rb.transform.position.x > target.position.x)?
                Mathf.Abs(offset.x) : -Mathf.Abs(offset.x);

            offset.y = (Random.Range(0, 2) == 1) ? /*rb.transform.position.y > target.position.y*/
                Mathf.Abs(offset.y) : -Mathf.Abs(offset.y);
            //Debug.LogWarning(offset);

        } else {
            offset = Vector2.zero;
        }

        offset += additiveOffset;

        //target world pos
        targetWorld = new Vector2(
            target.position.x + offset.x,
            target.position.y + offset.y
            );

        MoveTowardTarget();

        // calculate flightSlope - used to angle the unit
        //var rise = target.position.y - rb.transform.position.y;
        //var run = target.position.x - rb.transform.position.x;

        //flightSlope = (run != 0) ? Mathf.Tan(rise/run) :
        //    (rise >= 0) ? Mathf.Deg2Rad * 90 :
        //    Mathf.Deg2Rad * 270;
    }

    // Update is called once per frame
    void MoveTowardTarget() {

        

        //// move toward target destination
        //var direction = new Vector2(
        //    targetWorld.x - rb.transform.position.x,
        //    targetWorld.y - rb.transform.position.y
        //    );

        //float travelTime = Vector2.Distance(targetWorld, rb.transform.position) / moveSpeed;
        
        //Debug.LogWarning(transform.position);
        //Debug.LogWarning(targetWorld);
        //Debug.LogWarning(rb.velocity);
        //Debug.LogWarning(travelTime);
        startTween = rb.transform.DOMove(targetWorld, actualTravelTime).SetEase(ease).OnComplete(() => {
            floatTween = rb.transform.DOMoveY(targetWorld.y + actualDistance, actualFrequency)
            .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }) ;

       
        

        //rb.velocity = direction;

        //Debug.LogError(direction);

    }
    public override void TakeAction() {
        throw new System.NotImplementedException();
    }

    public override void OnBreak(bool grantReward) {

        if (startTween.IsActive()) {
            startTween.Kill();
        }

        if (floatTween.IsActive()) {
            floatTween.Kill();
        }

        base.OnBreak(grantReward);
    }

    public void Update() {
        //print(rb.velocity.y + " " + rb.velocity.x);

        Vector3 difference = new Vector3(
            targetWorld.x + lookDirection.x - rb.transform.position.x,
            targetWorld.y + lookDirection.y - rb.transform.position.y,
            0
            );
        if (difference.x == 0) return;

        // set rotation to point in the direction of travel
       
        spriteRend.transform.rotation = Quaternion.Euler(0, 0,
                Mathf.Tan(difference.y / difference.x) * Mathf.Rad2Deg
            );

        Debug.Log(spriteRend.transform.rotation);
    }

    public void OnDestroy() {
        transform.DOKill();
    }
}
