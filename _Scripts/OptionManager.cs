using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour {
    public readonly string fileName = "options.cfg";

    public static OptionManager options;

    // global settings
    public GameObject defaultHealthBar;
    public Color castleHealthColor = new Color(0, 141, 250);
    public Color towerHealthColor = new Color(24, 192, 58);
    public Color unitHealthColor = new Color(192, 23, 23);

    // gameplay settings
    public bool allowTowerResetAim = true;

    // level specific settings
    public float maxStalkLength = 2.2f;
    public Canvas background; // or image - not sure yet TODO

    public AudioSource defaultBreakSound;

    // collection of stalks. each tower can choose a different one; this is used for stalk skins
    public List<SpriteRenderer> stalkArray;

    public void Awake() {
        options = this;

        if (defaultBreakSound == null) { 
            defaultBreakSound = GetComponent<AudioSource>();
            if (defaultBreakSound == null) {
                
                Debug.LogWarning("The options manager has no default break sound.");
            }
        }
    }
}
