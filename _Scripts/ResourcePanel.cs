using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePanel : MonoBehaviour
{
    public static ResourcePanel resource;

    // convenience
    private Vector3 anchor;
    public Vector3 inset = new Vector3(.2f, -.2f);
    //public int maxHealth;
    //public float currentHealth; 
    public TextMesh healthText;
    //public GameObject healthSprite;
    //List<GameObject> healthSprites;

    //public int maxEnergy;
    //public float currentEnergy;
    public TextMesh energyText;

    //public int maxMoney;
    //public float currentMoney;
    public TextMesh moneyText;


    void Awake() {
        resource = this;

        anchor = Camera.main.ViewportToWorldPoint(new Vector3(0, 1)) + inset;

        // find text refs
        if (healthText == null) {
            healthText = GameObject.FindGameObjectWithTag("Health Text").GetComponent<TextMesh>();
        }
        //if (energyText == null) {
        //    energyText = GameObject.FindGameObjectWithTag("Energy Text").GetComponent<TextMesh>();
        //}
        if (moneyText == null) {
            moneyText = GameObject.FindGameObjectWithTag("Money Text").GetComponent<TextMesh>();
        }

        // todo: make cool resource panel
        //if (healthSprite == null) { Debug.LogError("no healthsprite"); }

        //healthSprites = new List<GameObject>(maxHealth);
        //for (int i = 0; i < maxHealth; i++) {
        //    healthSprites[i] = Instantiate(
        //        healthSprite,
        //        anchor + new Vector3(
        //            transform.localScale.x / 2 + healthSprite.transform.localScale.x * i + 1,
        //            transform.localScale.y / 2, 
        //            0), 
        //        Quaternion.identity,
        //        transform); ;
        //}

        // position panel 
        transform.position = 
            new Vector3(transform.localScale.x * .5f + anchor.x,
            -transform.localScale.y * .5f + anchor.y,
            0);
    }

    public void SetCounter(string key, int value) {
        switch(key.ToLower()) {
            case "health":
                healthText.text = value.ToString();
                break;
            //case "energy":
            //    energyText.text = value.ToString();
            //    break;
            case "money":
                moneyText.text = value.ToString();
                break;
        }
    }
}
