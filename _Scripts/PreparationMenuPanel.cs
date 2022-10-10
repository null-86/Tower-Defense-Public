using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparationMenuPanel : MonoBehaviour {
    // convenience
    private Vector3 anchor;
    public Vector3 inset = new Vector3(.2f, .2f);

   

    void Start() {
        anchor = Camera.main.ViewportToWorldPoint(new Vector3(1, 1)) + inset;

        // position panel 
        transform.position =
            new Vector3(-transform.localScale.x * .5f + anchor.x,
            -transform.localScale.y * .5f + anchor.y,
            0);
    }

    
}
