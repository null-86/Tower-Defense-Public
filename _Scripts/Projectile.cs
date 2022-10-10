using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour, IPooledObject
{
    public IPoolManager poolManager;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRend;

    public float travelSpeed;
    public float collisionDamage;


    public virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();

        if (spriteRend == null) { Debug.LogError("Projectile spriteRend == null"); return; }
    }

    public abstract void Launch(Transform target);

    public void OnCollisionEnter2D(Collision2D other) {

        // collision checking is handled by physics layers


        // check if unit collided and apply damage
        if (other.gameObject.GetComponent<BreakableEntity>() != null) {
            //Debug.Log(gameObject.name + " collided with castle");

            // damage castle
            other.gameObject.GetComponent<BreakableEntity>().ModifyHealth(-collisionDamage);

            poolManager.OnPooledObjectDestroyed(this);
            //gameObject.SetActive(false);

        }
    }

    public virtual void OnSpawn() {
        rb.simulated = true;
    }
    public virtual void OnDespawn() {
        rb.velocity = Vector3.zero;
        rb.simulated = false;
    }
}
