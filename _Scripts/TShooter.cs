using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class TShooter : Tower, ITargeter {

    public float damage = 1;
    public float range = 5;
    public int burst = 1;
    private int index = 0; // used to track bursts
    public float fireDelayMS = 1000;
    public float burstDelayMS = 0;
    protected DateTime nextFire = new DateTime();
    protected DateTime now = new DateTime();

    // rotation
    public bool allowAimRotation = true, allowResetRotation = true;
    public float rotationOffset;
    public float rotationSpeed = 1.9f;

    public ParticleSystem muzzleFlash;

    // filled by scripts on child gameobjects
    //[SerializeField]
    public List<Unit> targets = new List<Unit>();
    public Unit target; // make a global ref so runGraphics can reset end position

    


    public override void Awake() {
        base.Awake();
        if (muzzleFlash == null && spriteRend != null) {
            // try to get muzzleflash attached to child of barrel. 
            muzzleFlash = spriteRend.GetComponentInChildren<ParticleSystem>();
        }
    }

    public virtual Unit GetTarget() {


        // remove null or missing targets - targets may not be removed if they're destroyed but are
        // removed when exiting; do this before checking targets.count as this will change it
        targets.RemoveAll((unit) => unit == null);

        // Return if target list empty
        if (targets.Count <= 0) {
            return null;
        }
        // avoid null ref exception when castle is broken
        if (castle == null) {
            return null;
        }
        
        
        
        // no point sorting a single item
        if (targets.Count > 1) {
            // sort based on priority setting
            switch (priority) {
                case EPriority.CLOSE:
                    targets.Sort((x, y) =>
                        Mathf.Abs(x.GetXDistance(castle.transform))
                        .CompareTo(y.GetXDistance(castle.transform)));
                    break;

                case EPriority.FAR:
                    targets.Sort((x, y) =>
                        Mathf.Abs(y.GetXDistance(castle.transform))
                        .CompareTo(x.GetXDistance(castle.transform)));
                    break;

                case EPriority.STRONGEST:
                    targets.Sort((x, y) =>
                        Mathf.Abs(y.GetHealth())
                        .CompareTo(x.GetHealth()));
                    break;

                case EPriority.WEAKEST:
                    targets.Sort((x, y) =>
                        Mathf.Abs(x.GetHealth())
                        .CompareTo(y.GetHealth()));
                    break;

                case EPriority.EFFECTIVE:
                    Debug.LogWarning("Not implemented");
                    break;

                case EPriority.MOST:
                    Debug.LogWarning("Not implemented");
                    break;
            }
        }

        // return 1st in sorted list
        return targets[0];
    }

    public void Update() {
        TakeAction();
    }

    public override void TakeAction() {
        
        if (!canAct) {
            // stop animation 
            if (anim != null) {
                anim.speed = 0;
            }
            return; 
        }

        if (targets.Count > 0) {
            OnHasTarget();
        }
        else {
            OnNoTarget();
        }    
    }
    public virtual void OnHasTarget() {
        // update target (don't do this before time is ready or else scripts may draw animation
        // to target before fire rate allows)
        // edit: restructure to allow continuous rotation (TakeAim()); the draw animations can
        // be controlled by Shoot()
        target = GetTarget();

        // update time
        now = DateTime.Now;

        // rotate sprite (polymorphism can be used for unique sprites)
        // call TakeAim regardless of whether target is null since it can rotate back to default
        if (allowAimRotation) TakeAim();

        if (target != null && now >= nextFire) {
            
            // Shoot at target
            Shoot();

            // play sound
            if (activationSound != null) {
                activationSound.Play();
            }


            // Play shoot animation
            if (anim != null) {
                anim.speed = 1;
            }
            /*
                * TODO: make sure everything has a shoot anim
                *  this is way too spammy to use
            else {
                Debug.LogWarning(name + " does not have a shoot animation");
            }*/


            // fire rate cooldown
            index++;

            // set timer - short circuit the divide by 0 exception
            if (burst > 1 && index % burst == 0) {
                nextFire = now.AddMilliseconds(burstDelayMS);

            }
            else {
                nextFire = now.AddMilliseconds(fireDelayMS);
            }
        }
    }

    public virtual void OnNoTarget() {

        if (allowResetRotation) ResetAim();

        // stop animation 
        if (anim != null) {
            anim.speed = 0;
        }
    }


    public virtual void Shoot() {
        // muzzle flash
        if (muzzleFlash != null) {
            muzzleFlash.Play();
        }
    }

    // TODO: I could prevent the shooter from firing until the tower is rotated into place, but I think that will hurt the overall experience more than the realism would help.
    // reconsider this idea at a later time
    public virtual void TakeAim() {
        if (spriteRend != null && target != null) {

            // get dist from target
            float x = target.transform.position.x - spriteRend.transform.position.x;
            float y = target.transform.position.y - spriteRend.transform.position.y;

            // 
            float theta = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, theta + rotationOffset ));
            
            spriteRend.transform.rotation = 
                Quaternion.RotateTowards(spriteRend.transform.rotation, 
                targetRotation, rotationSpeed * Time.deltaTime) ;
        }   
    }

    public void ResetAim() {

        if (!OptionManager.options.allowTowerResetAim) { return; }
        if (spriteRend != null) {
            // reset rotation
            // TODO: fix rotation so that turrets go overtop to reset.
            Quaternion target = Quaternion.Euler(new Vector3(0, 0, rotationOffset));

            Quaternion rotation = Quaternion.RotateTowards(spriteRend.transform.rotation, 
               target, rotationSpeed * Time.deltaTime);
            
            spriteRend.transform.rotation = Quaternion.Euler(new Vector3(
                0,
                0,
                rotation.eulerAngles.z));
        }
    }

    public virtual void OnEnter(Collider2D other) {
        Unit unit = GetUnit(other);
        
        if (unit != null) {
            //Debug.Log(unit.GetInstanceID() + " " + unit.name + " entered");
            targets.Add(unit);

            // Don't call TakeAction or OnHasTarget since OnStay does this already
        }

    }

    public virtual void OnStay(Collider2D other) {
        //Unit unit = other.gameObject.GetComponent<Unit>();
        //if (unit != null) {

        //    // trigger TShooter action
        //    //OnHasTarget();
        //}
    }



    public virtual void OnExit(Collider2D other) {
        
        Unit unit = GetUnit(other);

        if (unit != null) {
            //Debug.Log(unit.GetInstanceID() + " " + unit.name + " exited");
            int i = targets.IndexOf(unit);
            // null object may already be removed by a null check found in GetTarget()
            if (i >= 0) {
                targets.RemoveAt(i);
            }
            
        }
        target = null;
    }  

    private Unit GetUnit(Collider2D coll) {
        //Debug.Log(other + " entered");
        Unit unit = coll.GetComponent<Unit>();

        // some units have a parent object with the unit data
        if (unit == null) {
            unit = coll.GetComponentInParent<Unit>();
        }

        return unit;
    }

}
