using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UStraightRusher : URusher
{
    // movement direction as a vector
    public Vector3 minDirection = new Vector3(-1, -1, 0);
    public Vector3 maxDirection = new Vector3(-1, 1, 0);
    public Vector3 direction;

    public Vector3 minRotation = new Vector3(0, 0, -90);
    public Vector3 maxRotation = new Vector3(0, 0, 90);
    public Vector3 rotation;
    
    public Transform objectTransform; // this is the transform that will move and rotate

    // Start is called before the first frame update
    public override void Awake() {
        base.Awake();

        direction = new Vector3(
           Random.Range(minDirection.x, maxDirection.x),
           Random.Range(minDirection.y, maxDirection.y),
           Random.Range(minDirection.z, maxDirection.z)
           );

        rb.velocity = moveSpeed * direction;
        
        rotation = new Vector3(
           Random.Range(minRotation.x, maxRotation.x),
           Random.Range(minRotation.y, maxRotation.y),
           Random.Range(minRotation.z, maxRotation.z)
           );

        if (objectTransform == null) {
            objectTransform = transform;
        }

    }

    public void Update() {
        //Debug.Log("fix this rotation so that I don't do any jank stuff. Just add a parent obj and rotate the child separately")
        if (objectTransform == null) { return; }
        
        objectTransform.Rotate(
            (rotation.x * Time.deltaTime),
            (rotation.y * Time.deltaTime),
            (rotation.z * Time.deltaTime)
        );
           
            
    }

    public override void OnBreak(bool grantReward) {
        base.OnBreak(grantReward);
        //throw new System.NotImplementedException();
    }

    public override void TakeAction() {
        
        //throw new System.NotImplementedException();
    }
}
