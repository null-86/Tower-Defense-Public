using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class NodeSequence : MonoBehaviour
{
    public List<Node> nodes = new List<Node>();
    public int msDelay;
    public int i = 0;

    // true = complete; false = incomplete
    public bool Run() {
        while (i < nodes.Count) {
            if (!EnemySpawner.spawner.CanSpawn()) {
                return false;
            }

            // try to run node but return false if it fails
            if (nodes[i] != null) {
                // if it fails, return false to indicate that branch is not complete
                if (!nodes[i].Run()) { return false; }
                //Thread.Sleep(msDelay);
                EnemySpawner.spawner.nextSpawn += msDelay;
            }

            i++;
        }
        return true;
    }

    public void PopulateList() {
        nodes = new List<Node>(GetComponentsInChildren<Node>());
    }
}
