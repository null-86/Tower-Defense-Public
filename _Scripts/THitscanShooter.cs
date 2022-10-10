using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public abstract class THitscanShooter : TShooter {


    //public Gradient colors = new Gradient();
    //public AnimationCurve width = AnimationCurve.Constant(0, 1, 1);
    
    protected LineRenderer line;
    public List<Vector3> lineArc;
    

    public override void Start () {
        base.Start();
        line = GetComponent<LineRenderer>();
    }
    public override void Shoot() {
        // damage
        target.ModifyHealth(-damage);
    }

    //public List<Vector3> positions = new List<Vector3>();
    
}
