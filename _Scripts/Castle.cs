using System.Collections.Generic;
using UnityEngine;

public class Castle : BreakableEntity {

    public static Castle castle;

    [SerializeField]
    private List<Anchor> anchorList = new List<Anchor>();
    public float maxEnergy = 100;
    public float currentEnergy = 10;
    public float energyRegen = 20; // per second
    public int currentMoney;
    public int maxMoney = 1000;

    public float flightAcceleration = 5;
    public float flightSpeed = 5;
    public float flightAngle = 35; // degrees
    public float flightRotationSpeed = 5;
    public int flightAccelerationFrames = 100; // frames to dampen up/down flight
    public int rotationAccelerationFrames = 200; // frames to dampen up/down rotation

    public float safeRadius = 10; // used by units that fly around the ship. This is the safe radius to avoid crashing

    public override void Awake() {
        base.Awake();

        castle = this;

        anchorList.Clear();

        // castle doesn't use this, but it prevents a warning
        spriteRend = GetComponentInChildren<SpriteRenderer>();
    }

    public override void Start() {
        base.Start();
        gameObject.layer = 9; // 9 is for castle
        GetComponent<Rigidbody2D>().isKinematic = true;
        

        ResourcePanel.resource.SetCounter("energy", GetEnergy());
        ResourcePanel.resource.SetCounter("money", currentMoney);
    }

    public override void ModifyHealth(float val) {
        base.ModifyHealth(val);
        ResourcePanel.resource.SetCounter("health", Mathf.CeilToInt(GetHealth()));
    }

    public override void OnBreak(bool grantReward) {
        ResourcePanel.resource.SetCounter("health", 0);// set counter before base() since it allows the counter to hit 0
        base.OnBreak(grantReward); // this breaks the castle object and hides it. I'm ok with this effect, so I'll leave it

        GameControl.control.UpdateGameState(GameState.Defeat);
        Debug.Log("add a cool animation where the ship blows up?");
    }
    
    public override void TakeAction() {
        throw new System.NotImplementedException();
    }

    public override void CreateHealthBar() {
        base.CreateHealthBar();
    
        // set color
        // set health bar color
        // note that this has the potential to break something if I stop using sprites
        healthBar.GetComponent<SpriteRenderer>().color = OptionManager.options.castleHealthColor;
    }

    public int GetEnergy() {
        return Mathf.FloorToInt(currentEnergy);
    }

    public int GetMoney() {
        return currentMoney;
    }

    //public void IncreaseEnergy() {
    //    // set energy to the max or to the increment
    //    currentEnergy = Mathf.Min(
    //        maxEnergy,
    //        currentEnergy + energyRegen * Time.deltaTime
    //        );


    //}

    public void ModifyEnergy(float val) {

        // set energy to the max or to the increment
        currentEnergy = Mathf.Min(
            maxEnergy,
            currentEnergy + val
            );

        ResourcePanel.resource.SetCounter("energy", GetEnergy());
    }

    public void ModifyMoney(int val) {
        currentMoney = Mathf.Min(currentMoney + val, maxMoney);
        //currentMoney = Mathf.Max(0, currentMoney); // no negative money

        ResourcePanel.resource.SetCounter("money", currentMoney);
    }

    public override void LateUpdate() {
        //base.LateUpdate(); // don't update the healthbar for castle
        if (!(EnemySpawner.spawner.waveEnded && EnemySpawner.spawner.pause)) {
            ModifyEnergy(energyRegen * Time.deltaTime);
        }

    }
}
