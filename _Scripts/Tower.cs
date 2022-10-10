using System.Linq;
using UnityEngine;

public abstract class Tower : BreakableEntity {

    public Castle castle;
    public EPriority priority = EPriority.CLOSE;
    public SpriteRenderer stalk;
    public bool moveable = false; // determines whether a tower can be dragged
    public bool canAct = true;
    public Vector3 startPos;
    public int uiPanelIndex; // used to track placement in ui panel for replacement
    public int healthCost = 0;
    public int moneyCost = 0;

    // choice of stalk skin from array
    public int stalkIndex = 0;

    public bool passDamageToCastle = true;


    public override void Awake() {
        base.Awake();
        // layer 10 is for towers
        gameObject.layer = 10;


        // get refs 
        castle = FindObjectOfType<Castle>();

        if (castle == null) {
            throw new System.Exception("Tower {" + name + "} cannot find its castle ref");
        }

        // set stalk ref
        if (stalk == null) {
            // get first child with tag "stalk"
            //Debug.LogWarning("Finding stalk for " + name + ".");
            stalk = GetComponentsInChildren<SpriteRenderer>().Where(res => res.tag.ToLower() == "stalk").FirstOrDefault();
            
        }

        // hide stalk
        stalk.transform.localScale = new Vector3(
            stalk.transform.localScale.x,
            0,
            stalk.transform.localScale.z);

    }

    public override void Start() {
        base.Start();
        GetComponent<Rigidbody2D>().isKinematic = true;

        // set stalk sprite (don't make it an else of the above conditional)
        if (stalk != null) {
            stalk.sprite = OptionManager.options.stalkArray[stalkIndex].sprite;
        } else {
            stalk.sprite = OptionManager.options.stalkArray[0].sprite; // 0 is the default stalk
        }

        // if a tower is pre-placed it should be non-moveable
        if (!moveable) {
            PlaceAnchor();
        }

        startPos = transform.position;
    }

    public override void TakeAction() {
        if (!canAct) { return; }
    }
    public override void CreateHealthBar() {
        base.CreateHealthBar();

        // set color
        // set health bar color
        // note that this has the potential to break something if I stop using sprites
        healthBar.GetComponent<SpriteRenderer>().color = OptionManager.options.towerHealthColor;
    }


    // TODO if you're feeling brave, try to fix this up
    // show possible anchor
    // true if valid; false otherwise
    
    public bool PlaceAnchor() {
        if (stalk == null) { Debug.LogError("stalk is null"); return false; }
        if (castle == null) { Debug.LogError("castle is null"); return false; }
        if (castle.coll == null) { Debug.LogError("castle.coll is null"); return false; }
        if (coll == null) { Debug.LogError("coll is null"); return false; }
        if (spriteRend == null) { Debug.LogError("spriteRend is null"); return false; }

        // find closest point on castle (spriteRend.transform.position is the pos of the pivot)
        Vector2 castlePoint = castle.coll.ClosestPoint(spriteRend.transform.position);
        

        // note: the check below requires that a composite collider use geometry type Polygon
        // outlines will optimize the collider is such a way as to ignore it
        float dist = Vector2.Distance(spriteRend.transform.position, castlePoint);

        // check if close to or inside tower
        // verify that castle has resources needed
        if (dist <= OptionManager.options.maxStalkLength && 
            Castle.castle.GetHealth() >= healthCost &&
            Castle.castle.GetMoney() >= moneyCost 
            ) {

            Vector2 spritePos = new Vector2(spriteRend.transform.position.x,
                spriteRend.transform.position.y);

            //// unparent rotating sprite from stalk
            //spriteRend.transform.parent = transform;
            //// unparent stalk from tower
            //stalk.transform.parent = null;

            // position stalk (stalk's pivot is top center, so that I can hide the tail of the stalk behind the castle
            //Vector2 center = (castlePoint + spritePos) / 2;
            stalk.transform.position = spriteRend.transform.position;

            // determine rotation
            Vector3 rotation = spritePos - castlePoint;
            stalk.transform.up = rotation;

            /*
                note to future self: 
                i spent days trying to figure this issue out. 
                sprites don't scale properly (maybe due to ppu)
                I use 120 by default; unity uses 100.
                I tried using spriteRenderer.size and renderer.bounds and looked at rectTransform and rect
                I didn't find anything that worked, so i just put the stalk next to a 1x1 square and 
                brute-forced .39 for this particular sprite makes it scale properly
            */

            //Debug.Log(dist);
            /////stalk.sprite.bounds.Encapsulate(castlePoint);
            //Debug.Log(stalk.sprite.rect.height/100 * stalk.transform.localScale.y);
            //Debug.Log(dist * (stalk.bounds.extents / 2).y);
            //Debug.Log(dist / (spriteRend.size / 2).y);
            //Debug.Log(spriteRend.bounds);
            // set length of stalk
            stalk.transform.localScale = new Vector3(
                stalk.transform.localScale.x,
                dist /** .39f*/, // I've no idea why .39 works, but i put the stalk next to a 1x1 square and .39 makes it match
                //stalk.sprite.rect.height / stalk.sprite.pixelsPerUnit * stalk.transform.localScale.y,
                stalk.transform.localScale.z); //Debug.Log(dist /stalk.bounds.size.y);

            // set position
            // re-parent stalk to tower
            stalk.transform.parent = transform;

            //// re-parent barrel/spriteRend
            //spriteRend.transform.parent = stalk.transform;

            return true;
        }
        else {
            // hide stalk if out of range
            stalk.transform.localScale = new Vector3(
                stalk.transform.localScale.x,
                0,
                stalk.transform.localScale.z);
            return false;
        } 
    }

    public void ConfirmAnchor() {
        if (PlaceAnchor()) {

            // deduct resource costs
            Castle.castle.ModifyMoney(-moneyCost);
            //Castle.castle.ModifyEnergy(-energyCost);
            


            transform.SetParent(castle.transform);
            moveable = false;
            canAct = true;
            TowerPanel.tower.Replace(uiPanelIndex);
        }
        else {
            Debug.Log("failed to place tower");
            canAct = false; // just in case it takes a sec to delete
            moveable = true;
            transform.position = startPos;
            //Destroy(gameObject);
        }
    }

    public override void ModifyHealth(float val) {
        if (passDamageToCastle) {
            Castle.castle.ModifyHealth(val);
        }
        else {
            base.ModifyHealth(val);
        }

    }

}
