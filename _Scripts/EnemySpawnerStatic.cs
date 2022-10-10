using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawnerStatic : EnemySpawner
{
    public List<Wave> waves;
    public int msDelay = 500;
    public Queue<Node> readyNodes = new Queue<Node>();   

    public override void Awake2()
    {
        // convert gameobjects to waves, seq, nodes ...
        PopulateList();
    }

    public override void Start() {
        base.Start();

        // get current unitCount
        unitCount = FindObjectsOfType<Unit>().Length;
    }




    // checks if enough time has passed for a new enemy to spawn
    public override bool CanSpawn() {
        
        //Debug.Log(Time.time * 1000+ " " + EnemySpawner.nextSpawn);
        if (Time.time * 1000 > nextSpawn) {
            return true;
        }
        else {
            return false;
        }
    }

    // i made it bool to be consistent with the wave, nodesequence, and node class, but i dont use its value here
    private bool Run() {
        while (GetWave() < waves.Count) {
            if (!CanSpawn() || (pause && waveEnded)) {
                return false;
            }

            // if I'm allowed to spawn ie. get past the sentinel above. I should assume wave is going
            waveEnded = false;

            if (waves[GetWave()] != null) {
                if (!waves[GetWave()].Run()) { return false; }
                //Thread.Sleep(waveDelay);
                nextSpawn += msDelay;
            }
        }
        return true;
    }

    // part of threading method: failed since pausing breaks it




    private void PopulateList() {
        waves = new List<Wave>(GetComponentsInChildren<Wave>());
        foreach (var wave in waves) {
            wave.PopulateList();
        }
    }

    public void Update() {
        Run();
    }

    // unity cannot instantiate from another thread, so we check our instantiate queue and 
    // instantiate them from main thread
   





    public override void TestStateChange() {
        // if no more units and no more in queue
        if (unitCount <= 0 && pause && GameControl.control.state != GameState.Preparation) {
            // probably useless, but reset unitCount just in case
            unitCount = 0;

            // counter has equaled or surpassed # of waves
            if (GetWave() >= waves.Count) {
                GameControl.control.UpdateGameState(GameState.Victory);
            } else {
                GameControl.control.UpdateGameState(GameState.Preparation);
            }
            
        }
    }

    public override void OnDestroy2() {
    }
}
