using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class EnemySpawnerAlgorithm : EnemySpawner {
    public int currentEnergy = 0;


    // y = ax^b + c
    // waveEnergy = coefficient(waveIndex)^(exponent) + startEnergy 

    public float coefficient = 1f;
    public float exponent = 1.1f;
    public int startEnergy = 50;
    //private bool inAWave = false;

    
    public int waveDelay = 5000;

    [Range(0, 100)] public int maxSpawnPricePercent = 30;
    //[Range(0, 100)] public float minUnitSpawnHeightPercent = 15;
    //[Range(0, 100)] public float maxUnitSpawnHeightPercent = 85;


    

    public List<Unit> affordableUnits = null;



    // Todo: make difficulty settings by using preconfigured values stored into profile. use calc to view diff curve

    public override void Start()
    {
        base.Start();
        //currentEnergy = startEnergy;
        //inAWave = false;[

       
    }



    public void Update() {
        //print(!CanSpawn() + ";;;" + pause + ";" + waveEnded);
        if (!CanSpawn() || (pause && waveEnded) || GameControl.control.state != GameState.Combat) {
            return;
        }

        // start of wave
        if (waveEnded && !pause) {
            StartWave(GetWave());
        } else {
            RunWave(GetWave());
        }

        
        
        
    }
    
    public void StartWave(int waveIndex) {
        waveEnded = false;
        //Debug.LogWarning(waveIndex + " " + Time.time);
        // refill energy
        currentEnergy += GetEnergyRefill(waveIndex);
        //print(currentEnergy * maxSpawnPricePercent / 100);
        // get affordable units
        affordableUnits = LevelSettings.level.unitList.FindAll(
            unit => unit.spawnCost <= currentEnergy * maxSpawnPricePercent / 100);
        //foreach (var a in affordableUnits) { print(a.name + " " + a.spawnCost); }
    }

    public void RunWave(int waveIndex) {

        // filter out units that are too expensive
        affordableUnits = affordableUnits.FindAll(
            u => u.spawnCost <= currentEnergy * maxSpawnPricePercent / 100)
            .OrderBy(u => u.spawnCost).ToList<Unit>();

        //foreach(var a in affordableUnits) { print(a.name + " " + a.spawnCost); }
        int e = 0;
        // decide which units to buy and spawn them
        if (affordableUnits.Count > 0 && currentEnergy > 0 && e < 10 ) {
            //foreach (var a in affordableUnits) { print(a.name + " " + a.spawnCost); }
            // spawn randomly from list
            // todo: add intelligence to this if it isn't fun truly random

            // this is the number of units I can safely spawn before recalculating affordable units
            // safeSpawns = Mathf.FloorToInt(currentEnergy / (maxSpawnPricePercent / 100));
            int safeSpawns = Mathf.FloorToInt(100 / maxSpawnPricePercent);
            //int i = 0;
            //if (i < safeSpawns) {

                // calculate unit and spawn height
                Unit unit = affordableUnits[UnityEngine.Random.Range(0, affordableUnits.Count)];

                //float spawnHeight = unit.GetSpawnHeight();
                int delay = UnityEngine.Random.Range(unit.minSpawnDelay, unit.maxSpawnDelay);

                
                // subtract spawnCost from energy
                currentEnergy -= unit.spawnCost;

                // increment delay
                nextSpawn += delay;

                //print(unit.name + " " + spawnHeight);
                Spawn(unit, unit.GetSpawnPosition());
            //print("spawning " + unit.name + " " + unit.GetSpawnPosition());
                //i++;
                //}
                //else {
                // filter out units that are too expensive
                //affordableUnits = affordableUnits.FindAll(
                    //u => u.spawnCost <= currentEnergy * maxSpawnPricePercent / 100)
                    //.OrderBy(u => u.spawnCost).ToList<Unit>();
                //}

                e++;
        } else {
            // wave ends here

            //todo: add wave delay and start next wave
            waveEnded = true;

            // adjust spawn chance if allowed
            foreach (Unit unit in affordableUnits) {
                if (unit.adjustSpawnChanceEachWave) {
                    unit.AdjustSpawnChances();
                }
            }
            
            

            // add wave delay
            nextSpawn += GetWaveDelay(GetWave());

            // wave ended        
            NextWave(GetWave());
        }

        

        
    
    } 



    private float GetWaveDelay(int waveIndex) {
        // todo: consider a random/function-based wave delay formula
        // maybe shorter delay at higher #s

        // just return 1 second for now
        float delay = 0;

        if ( waveIndex >= 1) { delay = waveDelay; }
        return delay;
    }

    private int GetEnergyRefill(int waveIndex) {
        // visualize function on graph
        // https://www.desmos.com/calculator

        // y = ax^b + c
        // waveEnergyRefill = coefficient(waveIndex)^(exponent) + startEnergy 

        int res = Mathf.RoundToInt(
            Mathf.Pow(coefficient * waveIndex, exponent) + startEnergy
            );
        //print("energy refill: " + res);
        return res;
    }

    public override bool CanSpawn() {

        //Debug.Log(Time.time * 1000+ " " + EnemySpawner.nextSpawn);
        if (Time.time * 1000 > nextSpawn) {
            return true;
        }
        else {
            return false;
        }
    }

    public override void StartWaves() {
        base.StartWaves();
        //throw new System.NotImplementedException();
    }

    public override void PauseWaves() {
        base.PauseWaves();
        //throw new System.NotImplementedException();
    }

    public override void Awake2() {
    }

    public override void OnDestroy2() {
    }

    public override void TestStateChange() {
        if (unitCount > 0 || !pause || GameControl.control.state == GameState.Preparation) { return; }

        unitCount = 0;
        // random is endless - so we never send to victory or defeat here
        // todo: could be added later if we're trying to survive a # of waves or something
        GameControl.control.UpdateGameState(GameState.Preparation);

    }
}
