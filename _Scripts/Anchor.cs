using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchor : MonoBehaviour
{
    public Tower tower;
    // if this is 1, the anchor and tower must cross on their center points
    // higher numbers allow for a "close enough" interpretation
    public float rangeModifier = 1f; 
    public int unlockCost = 0;

    public void Init() {
        FindTower();
        AlignTower();
    }
    public void Awake() {
        Debug.LogWarning("don't use anchor; allow player to place tower wherever; use sprite shape or use spriteAlignment");
        Debug.Log("consider making a circle hitbox to follow the player when they drag the tower. When it hits a castle sprite, get the dist and hit spot (or the average of 2 points) and use that to connect");
    }

    public void FindTower() {
        // 10 is the layer that towers exist in
        Collider2D[] cols = Physics2D.OverlapBoxAll(transform.position, transform.lossyScale * rangeModifier, 0, 1 << 10);

        if (cols.Length <= 0) { return; }

        // find the closest tower and claim that one
        float min = Vector2.Distance(cols[0].ClosestPoint(transform.position), transform.position);
        int index = 0;
        for (int i = 1; i < cols.Length; i++) {
            float dist = Vector2.Distance(cols[0].ClosestPoint(transform.position), transform.position);
            if (dist < min) {
                min = dist;
                index = i;
            }
        }
        
        // it's impossible for 2 towers to claim the same tower, since this is synchronous.
        // whichever anchor goes first will claim and move it
        if (cols[index] != null) {
            tower = cols[index].GetComponent<Tower>();
        }
        
    }

    public void AlignTower() {
        if (tower != null) {
            // set tower on position of anchor
            tower.transform.position = transform.position;
        }
    }
}
