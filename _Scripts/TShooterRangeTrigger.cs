using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class TShooterRangeTrigger : MonoBehaviour
{
    // ref to tower 
    TShooter shooter;

    public void Awake()
    {
        shooter = transform.parent.GetComponent<TShooter>();

        CircleCollider2D col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;

        col.radius = shooter.range;

    }

    /// add targets
    public void OnTriggerEnter2D(Collider2D other) {
        //Debug.Log(other.name);
        //GetComponentInParent<Collider2D>();
        shooter.OnEnter(other);
    }

    // keep taking actions
    public void OnTriggerStay2D(Collider2D other) {
        shooter.OnStay(other);
    }

    /// remove targets
    public void OnTriggerExit2D(Collider2D other) {
        shooter.OnExit(other);
    }
}
