using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner spawner;

    public Transform unitHolder;

    private int waveIndex = 0;
    public int unitCount = 0;
    public float nextSpawn = 0;
    public bool waveEnded = true; // assume wave 0 ended so that first wave doesn't start automatically
    public bool pause = true; // if pause is set, delay next wave; else go


    protected float timeDiff; // track time diff b/t current time and nextSpawn when paused
    // use this to offset nextspawn on play


    public abstract bool CanSpawn();

    public void Spawn(Node n) {
        Spawn(n.unitPrefab, n.unitPrefab.GetSpawnPosition());
    }

    public void Spawn(Unit unitPrefab, Vector2 spawnPosition) {
        Unit unit = Instantiate(unitPrefab,
            spawnPosition,
            Quaternion.identity);

        // camera is offset which makes the units appear in a place that is not rendered.
        // put them back into position and assign them to a parent to keep the gameobject hierarchy clean
        unit.transform.position = new Vector3(unit.transform.position.x, unit.transform.position.y);
        unit.transform.SetParent(unitHolder);

        // increment counter
        unitCount++;

    }

    public abstract void Awake2();
    public abstract void OnDestroy2();


    public abstract void TestStateChange();

    public event Action<int> OnWaveChanged;

    public void Awake() {
        this.Awake2();
        spawner = this;

        if (unitHolder == null) {
            unitHolder = transform;
        }

    }

    public virtual void Start() {
        GameControl.control.OnGameStateChanged += OnGameStateChanged;
    }

    public void OnGameStateChanged(GameState state) {
        if (state == GameState.Combat) {
            this.StartWaves();
        }
        else {
            this.PauseWaves();
        }
    }
    public void OnDestroy() {
        GameControl.control.OnGameStateChanged -= OnGameStateChanged;
        OnWaveChanged?.Invoke(waveIndex++);
        this.OnDestroy2();
    }

    public int GetWave() {
        return waveIndex;
    }

    public void NextWave(int endedWave) {
        waveIndex = endedWave + 1;
        waveEnded = true;
        TestStateChange();
    }

    //public static void OnWaveEnd(int en) {
    //    waveEnded = true;

    //    TestStateChange();
    //}

    public virtual void StartWaves() {
        if (!pause) { return; }
        pause = false;
        waveEnded = false;

        // add the time spent paused to nextSpawn
        nextSpawn = Time.time * 1000 + timeDiff;
    }

    public virtual void PauseWaves() {
        if (pause) { return; }
        pause = true;

        // track the time diff b/t nextSpawn and current time when paused
        timeDiff = nextSpawn - Time.time * 1000;
    }

    public virtual void OnUnitBreak() {
        unitCount--;

        TestStateChange();
    }

}
