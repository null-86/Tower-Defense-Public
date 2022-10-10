using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


// collections of enemies, duration of wave, rewards, new objectives?
public class Wave : MonoBehaviour {
    
    public List<NodeSequence> sequences = new List<NodeSequence>();
    public int msDelay;
    int i = 0;

    //public static event Action<int> OnWaveEnd;

    public bool Run() {
        while (i < sequences.Count) {
            if (!EnemySpawner.spawner.CanSpawn()) {
                return false;
            }

            if (sequences[i] != null) {
                // GameControl.control.SetwaveText(i + 1); // +1 because i is 0 indexed, and waves are 1 indexed
                // if it fails, return false to indicate that branch is not complete
                if (!sequences[i].Run()) { return false; }
                //Thread.Sleep((int)(msDelay));
                // add delay 
                EnemySpawner.spawner.nextSpawn += msDelay;
                // increment wave #
                
            }

            i++;
        }
        ////Debug.LogWarning("wave " + i + " ended");
        //OnWaveEnd?.Invoke(i);
        EnemySpawner.spawner.NextWave(EnemySpawner.spawner.GetWave());
        return true;
    }

    public void AddObjective() {
        throw new System.NotImplementedException();
    }

    public void PopulateList() {
        sequences = new List<NodeSequence>(GetComponentsInChildren<NodeSequence>());
        foreach (var seq in sequences) {
            seq.PopulateList();
        }
    }
}
