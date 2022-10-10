using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDumbAim : Projectile
{

    // use this instead of start; use awake for rigidbody references etc
 
    public override void Launch(Transform target) {

        // determine direction
        var direction = target.position - transform.position;
        

        // set rb velocity
        rb.velocity = (direction).normalized * travelSpeed;

        print(direction + " " + rb.velocity);
        
        // todo
        // rotate sprite to point toward target
        transform.rotation = Quaternion.Euler(new Vector3(
            0, 0, Vector2.Angle(Vector2.right, direction)
            ));
    }

    
}
