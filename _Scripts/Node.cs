using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public class Node : MonoBehaviour
{
    // collection of unit and the delay before the next spawns
    public Unit unitPrefab;
    public int msDelay = 500;
    [Range(0, 100)]
    public float spawnHeight;

    // true = complete; false = incomplete
    public bool Run() {
        // make sure enough time has passed
        if (!EnemySpawner.spawner.CanSpawn()) { return false; }
        
        if (unitPrefab != null) {
            // add self to spawner's queue (Unity can't instantiate from side thread;
            // see enemyspawner.lateupdate for more info)
            EnemySpawner.spawner.Spawn(this);
            EnemySpawner.spawner.nextSpawn += msDelay;
            //EnemySpawner.spawner.readyNodes.Enqueue(this);
            //Thread.Sleep(msDelay);
            
        }

        return true;
    }
}
