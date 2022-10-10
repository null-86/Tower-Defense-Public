using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActiveEntity : Entity, IActive {

    // be very careful using these for specific rates like fire rates and move speed...
    // these are meant as a control on action rate (may not be used, but easier to put now)
    public float actionRate = 1;
    public float intensity = 1;

    public AudioSource activationSound;

    public virtual void Awake() {
        activationSound = GetComponent<AudioSource>();
    }

    public abstract void TakeAction();
}
