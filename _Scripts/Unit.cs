using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Unit : BreakableEntity {

    public int breakReward = 5; // money given to player/castle after destroying the unit
    public int spawnCost = 5;


    // spawn control vars
    public int minSpawnDelay = 200;
    public int maxSpawnDelay = 1000;

    [Range(-5, 105)] public float minUnitSpawnHeightPercent = 15;
    [Range(-5, 105)] public float maxUnitSpawnHeightPercent = 85;

    [Range(-5, 105)] public float minUnitSpawnWidthPercent = 105;
    [Range(-5, 105)] public float maxUnitSpawnWidthPercent = 105;

    public bool adjustSpawnChanceEachWave = true;
    public int maxDeviation = 2;


    public List<int> regionSpawnChances = new List<int>();
    [UnityEngine.SerializeField] private List<float> regionSpawnHeights;

    public override void Awake() {
        base.Awake();
        // layer 8 is for units
        gameObject.layer = 8;

        // this should fix collision issues, but it may hurt performance.
        // watch carefully

        GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        CalculateRegionSpawnHeights();
    }

    private void CalculateRegionSpawnHeights() {
        regionSpawnHeights = new List<float>();

        float currentBottom = minUnitSpawnHeightPercent;

        // convert zone to a percentage of allowed height (also as a percentage)
        // ie. proportional height that works reliably with changing max/min heights
        float totalHeightAsPercent = maxUnitSpawnHeightPercent - minUnitSpawnHeightPercent;
        float stepValue = totalHeightAsPercent / regionSpawnChances.Count;

        for (int i = 0; i < regionSpawnChances.Count; i++) {
            float currentTop = stepValue + currentBottom;
            regionSpawnHeights.Add((currentTop + currentBottom) / 2);
            currentBottom = currentTop;
        }

        if (regionSpawnChances.Count <= 0) {
            regionSpawnHeights.Add((totalHeightAsPercent / 2) + minUnitSpawnHeightPercent);
        }


    }


    public void AdjustSpawnChances() {
        for (int i = 0; i < regionSpawnChances.Count; i++) {
            regionSpawnChances[i] += UnityEngine.Random.Range(0, maxDeviation + 1);
        }
    }

    public Vector2 GetSpawnPosition() {
        return Camera.main.ViewportToWorldPoint(new Vector2(
            GetSpawnWidth(),
            GetSpawnHeight()
            ));
    }

    private float GetSpawnWidth() {
        float res;

        //if (regionSpawnChances.Count >= 1) {
        //    Debug.LogError("The region spawn heights no longer works; default to random height");

        //    int totalRange = regionSpawnChances.Sum();
        //    float rand = UnityEngine.Random.Range(0, totalRange + 1); // +1 to make it inclusive
        //    int spawnZone = regionSpawnChances.Count - 1;

        //    // find what zone rand lands in
        //    int sum = 0;
        //    for (int i = 0; i < regionSpawnChances.Count; i++) {
        //        sum += regionSpawnChances[i];

        //        // random # falls under the sum of the ranges. On the first occasion this happens, we know that's the
        //        // range the # is in
        //        if (sum > rand) {
        //            spawnZone = i;
        //            //print(spawnZone + " -- " + rand);
        //            break;
        //        }
        //    }


        //    // convert zone to a percentage of allowed height (also as a percentage)
        //    // ie. proportional height that works reliably with changing max/min heights
        //    //float totalHeightAsPercent = maxUnitSpawnHeightPercent - minUnitSpawnHeightPercent;

        //    //res = ((spawnZone / regionSpawnChances.Count) * totalHeightAsPercent) + minUnitSpawnHeightPercent;
        //    res = regionSpawnHeights[spawnZone];
        //}
        //else {
            //print("random");
            // just pick a random spot 
            res = UnityEngine.Random.Range(minUnitSpawnWidthPercent, maxUnitSpawnWidthPercent);
        //}

        //make it a percent
        return res * .01f; 
    }


    private float GetSpawnHeight() {
        float res;
        
        if (regionSpawnChances.Count >= 1) {
            Debug.LogError("The region spawn heights no longer works; default to random height");

            int totalRange = regionSpawnChances.Sum();
            float rand = UnityEngine.Random.Range(0, totalRange + 1); // +1 to make it inclusive
            int spawnZone = regionSpawnChances.Count - 1;

            // find what zone rand lands in
            int sum = 0;
            for (int i = 0; i < regionSpawnChances.Count; i++) {
                sum += regionSpawnChances[i];

                // random # falls under the sum of the ranges. On the first occasion this happens, we know that's the
                // range the # is in
                if (sum > rand) {
                    spawnZone = i;
                    //print(spawnZone + " -- " + rand);
                    break;
                }
            }


            // convert zone to a percentage of allowed height (also as a percentage)
            // ie. proportional height that works reliably with changing max/min heights
            //float totalHeightAsPercent = maxUnitSpawnHeightPercent - minUnitSpawnHeightPercent;

            //res = ((spawnZone / regionSpawnChances.Count) * totalHeightAsPercent) + minUnitSpawnHeightPercent;
            res = regionSpawnHeights[spawnZone];
        }
        else {
            //print("random");
            // just pick a random spot 
            res = UnityEngine.Random.Range(minUnitSpawnHeightPercent, maxUnitSpawnHeightPercent);
        }

        // make it a percent
        return res * .01f;
    }

    public override void CreateHealthBar() {
        base.CreateHealthBar();

        // set color
        // set health bar color
        // note that this has the potential to break something if I stop using sprites
        healthBar.GetComponent<SpriteRenderer>().color = OptionManager.options.unitHealthColor;
    }

    public virtual float GetXDistance(Transform target) {
        return Mathf.Abs(transform.position.x - target.position.x);
    }

    public override void OnBreak(bool grantReward) {
        if(broken) { return; }
        base.OnBreak(grantReward);

        // give reward money to castle
        if (grantReward) {
            Castle.castle.ModifyMoney(breakReward);
        }
        

        // change gamestate if no units left
        EnemySpawner.spawner.OnUnitBreak();
    }
}
