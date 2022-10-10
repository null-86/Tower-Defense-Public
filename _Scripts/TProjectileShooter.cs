using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TProjectileShooter : TShooter, IPoolManager {

    public Projectile projectile;
    public Transform spawnPos;
    public float projectileSpeed = 7;
    // a projectile has a gameobject and a projectile speed. it may also have logic to control special targetting scripts
    public int queueStartSize = 10;
    public Queue<Projectile> projectiles;


    public override void Awake() {
        base.Awake();

        if (spawnPos == null) spawnPos = transform;

        projectiles = new Queue<Projectile>();
        for (int i = 0; i < queueStartSize; i++) {
            CreateProjectile();
        }
    }

    private void CreateProjectile() {
        Projectile proj = Instantiate(projectile, spawnPos.position, Quaternion.identity, spawnPos);
        proj.poolManager = this;
        proj.collisionDamage = damage;
        proj.travelSpeed = projectileSpeed;
        proj.gameObject.SetActive(false);
        projectiles.Enqueue(proj);
    }

    private void EnableProjectile(Projectile proj) {
        proj.transform.position = spawnPos.position;
        proj.gameObject.SetActive(true);
        proj.OnSpawn();
    }
    private void DisableProjectile(Projectile proj) {
        proj.OnDespawn();
        projectiles.Enqueue(proj);
        proj.gameObject.SetActive(false);
    }

    public override void Shoot() {

        if (target == null) return;
        if (projectile == null) { Debug.LogError("No projectile"); return; }

        // add to pool if empty
        if (projectiles.Count < 1) { CreateProjectile();  }

        // set proj position
        Projectile proj = projectiles.Dequeue();
        EnableProjectile(proj);

        // launch projectile according to its specific targetting algorithm
        proj.Launch(target.transform);
    }

    public void OnPooledObjectDestroyed(Projectile proj) {
        DisableProjectile(proj);
    }

    



    //public override void TakeAim() {
    //    if (spriteRend != null && target != null) {

    //        // https://mathemerize.com/point-of-intersection-of-two-lines/

    //        // slope of target = rise / run
    //        float targetSlope = target.rb.velocity.y / target.rb.velocity.x;

    //        // point of target 
    //        // target.transform.position

    //        // slope of projectile
    //        float projectileSlope = 0; // to be determined


    //        // point of projectile
    //        // this.transform.position

    //        print("fix projectile targeting");
    //        // intersection
    //        //Vector2 intersect = new Vector2 (

    //        //    )

    //        // 
    //        //float theta = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

    //        ////Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, theta + rotationOffset));

    //        //spriteRend.transform.rotation =
    //        //    Quaternion.RotateTowards(spriteRend.transform.rotation,
    //        //    targetRotation, rotationSpeed * Time.deltaTime);
    //    }
    //}


    //public override void TakeAction() {
    //    if (!canAct) { return; }
    //    throw new System.NotImplementedException();
    //}



}
