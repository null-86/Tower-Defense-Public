using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class THitscanBeamShooter : THitscanShooter {
    
    public int frequency = 0;

    public override void Shoot() {
        base.Shoot();

        // hide bullet line when no targets are available
        if (targets.Count <= 0) {
            line.enabled = false;
        }

    }

    //
    public void DrawBullet() {
            // draw bullets
        int i = 0;

        line.positionCount = 2; // will need atleast a start and end
        // start at tower itself
        line.SetPosition(i++, transform.position);

        /*
        // TODO allow curved lines
        // get dot product to smooth out line
        //Vector3 lineIntersection = new Vector3()

        while (i < line.positionCount -1) {
            line.SetPosition(i++, )
        }

        // add inspector arc
        foreach(Vector3 v in lineArc) {
            line.SetPosition(i++, v);
        }*/

        // end line on target
        // count + 1 = index of end-point
        line.SetPosition(/*positions.Count + 1*/ i, target.transform.position);
        
        // smooth line?
        
        // line renderer component already has a tolerance. Maybe it simplifies automatically 
        //line.Simplify(line.G);
        
    }

    //private Vector2 Curve(List<Vector2> vectors, float t) {
    //    // https://www.youtube.com/watch?v=RF04Fi9OCPc
    //    return new Vector2();
    //}



    // I may do this, but I'm going to use LateUpdate for now. I don't want to optimize something that doesnt need it

    public override void LateUpdate() {
        base.LateUpdate();

        // turn off line if no target
        if (target == null) {
            line.enabled = false;
            return;
        }

        if (target != null) {
            // turn on line
            DrawBullet();
            line.enabled = true;
        } else {
            line.enabled = false;
        }
        
    }
    /*
    public override void RunGraphics(bool on) {
        
        // turn off line
        if (!on) {
            line.enabled = false;
            return;
        }
        
        line.SetPosition(line.positionCount - 1, target)
    }
    */

}
