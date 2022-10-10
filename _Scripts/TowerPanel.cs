using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPanel : MonoBehaviour
{
    public static TowerPanel tower;

    // convenience
    private List<Tower> towers;
    private Vector3 anchor;
    public Vector3 inset = new Vector3(.2f, .2f);

    public Transform backgroundTransform;

    public float height = 3;
    public float width = 3;

    public void Awake() {
        tower = this;
    }
    public void Start()
    {
        towers = LevelSettings.level.towerList;
        anchor = Camera.main.ViewportToWorldPoint(Vector3.zero) + inset;


        // set panel width equal to # of towers in this level
        backgroundTransform.localScale = new Vector3(
            width * (towers.Count + .5f),
            height,
            1);
        
        // position panel at bottom left of screen
        backgroundTransform.position = anchor +
            new Vector3(backgroundTransform.localScale.x * .5f,
            backgroundTransform.localScale.y * .5f,
            backgroundTransform.localScale.z);

        // place towers uniformly across the panel
        for (int i = 0; i < towers.Count; i++) {
            Replace(i);
        }


        
        Debug.Log("make level settings and let it store list of towers to be used.");
    }

    public void Replace(int towerIndex) {
        Tower tower = Instantiate(towers[towerIndex],
                // get offset position based on bottom left corner of screen
                anchor + new Vector3(width * (towerIndex + .5f), .5f * height, 1),
                Quaternion.identity,
                backgroundTransform.parent);

        // prepare tower
        tower.moveable = true;
        tower.canAct = false;

        tower.uiPanelIndex = towerIndex;
    }
}
