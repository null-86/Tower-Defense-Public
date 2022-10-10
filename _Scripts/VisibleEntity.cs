using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VisibleEntity : ActiveEntity { 
    public SpriteRenderer spriteRend;
    public Animator anim;

    public override void Awake() {
        base.Awake();
        // check if I manually assigned an anim (for use in complex sprites where I can't easilly find it automatically)
        if (anim == null) {
            // check self
            anim = GetComponent<Animator>();
            // check children
            if (anim == null) {
                anim = GetComponentInChildren<Animator>();
            }
        }

        // prevent animations from playing on start
        // can be overriden in subclasses, since their awakes run after this
        if (anim != null) {
            anim.speed = 0;
        }
    }

    public virtual void Start() {
        if (spriteRend == null) { Debug.LogError("Visible Entity " + name + " has no sprite"); }
    }

    //public abstract void RunGraphics(bool on);
}
