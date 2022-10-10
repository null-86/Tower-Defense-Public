using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class SceneOrganizer : MonoBehaviour
{
    [SerializeField]
    private Castle castle = null; // enemies that fly past  the tower should also cause damage
    public Vector2 sizeMultiplier = new Vector2(2.5f, 1.3f);
    public LayerMask hiddenLayers;
    private bool startup = true; // don't damage player for startup cleaning
       
    public void Start() {

        // hiddenLayers currently causes only these layers to be hit
        // switch the bits to hide them
        hiddenLayers = ~hiddenLayers;

        // stretch organizer around the camera
        StretchTo(Camera.main);
        
        // delete anything that happens to be off-screen
        DeleteOffScreen();

        // end startup phase
        castle = GameObject.FindObjectOfType<Castle>();
        startup = false;
    }

    // couldn't think of a good name. It scales the hitbox responsible for removing out-of-bounds objects
    private void StretchTo(Camera mainCam) {
        

        // get screenpoint coords (they ignore pixel ratio)
        Vector3 screenBottomLeft = mainCam.ViewportToScreenPoint(Vector3.zero);
        Vector3 screenTopRight = mainCam.ViewportToScreenPoint(Vector3.one);

        // convert to world coords
        // ignore z since it's used for depth
        Vector3 worldBottomLeft = mainCam.ScreenToWorldPoint(new Vector3(screenBottomLeft.x, screenBottomLeft.y, 0));
        Vector3 worldTopRight = mainCam.ScreenToWorldPoint(new Vector3(screenTopRight.x, screenTopRight.y, 0));

        // if you change this value, make sure that the formulas below work out. They're not scientific, just intuitive
        // TODO improve the width division formula 
        
        float width =  (worldTopRight.x - worldBottomLeft.x);
        float height = (worldTopRight.y - worldBottomLeft.y);

        // stretch to screen
        transform.localScale = new Vector3(
            sizeMultiplier.x * width,
            sizeMultiplier.y * height,
            0
        );

        // position on edge of camera
        transform.position = new Vector3(
            ((transform.localScale.x - mainCam.transform.position.x) / 2) - width / 2, 
            Camera.main.transform.position.y,
            0
        );
    }


    private void DeleteOffScreen() {
        
        /*
            note: using collider2D for the "all" list allows any gameobject without a collider to go undeleted.
            i decided this was desirable since it prevents me from accidentally deleting gamecontrol or some
            other important item. I imagine the performance gain from deleting 1 random object doesn't warrant
            the dev pain caused by random deletions.
        */


        // get all colliders in scene
        Collider2D[] all = FindObjectsOfType<Collider2D>();

        // get colliders in range
        List<Collider2D>  inRange = new List<Collider2D>(
            Physics2D.OverlapBoxAll(transform.position, 
            transform.localScale, transform.rotation.z));



        foreach(Collider2D c in all) {
            
            // find objects not in range
            // for an explanation of the weird function https://answers.unity.com/questions/50279/check-if-layer-is-in-layermask.html
            if (!inRange.Contains(c) && hiddenLayers == (hiddenLayers | (1 << c.gameObject.layer))) {
                c.gameObject.SetActive(false);
                Destroy(c.gameObject);
            }
            
        }
    }




    // TODO on trigger enter to enable rendering 


    // destroy objects off-screen
    public void OnTriggerExit2D(Collider2D other) {
        if (other != null && hiddenLayers == (hiddenLayers | (1 << other.gameObject.layer))) {

            //var parent = other.GetComponentInParent<BreakableEntity>();

            //if (parent != null && parent.coll != null) {
            //    other = parent.coll;
            //}
            
            var bypassDestroy = false;
            
            if (!startup) {
                var breakableEntity = other.GetComponent<BreakableEntity>();
                if (breakableEntity != null) {

                    // no reward when exiting area
                    Unit unit = breakableEntity.GetComponent<Unit>();
                    if (unit != null) {
                        //unit.breakReward = 0;
                    }
                    
                    breakableEntity.OnBreak(false);
                }

                //if (other.gameObject.layer == LayerMask.NameToLayer("Tower Projectile") ||
                //    other.gameObject.layer == LayerMask.NameToLayer("Unit Projeactile"))

                // bypass destroying projectiles that hit units
                if (other.gameObject.layer == 12 || other.gameObject.layer == 13) bypassDestroy = true;

                
                //Debug.Log(other);
                //Debug.Log(projectile);
                // bypass destroying projectiles that hit the border of the map
                Projectile projectile = other.GetComponent<Projectile>();
                if (projectile == null) projectile = other.GetComponentInParent<Projectile>();
                if (projectile != null) {

                    
                    bypassDestroy = true;
                    projectile.poolManager.OnPooledObjectDestroyed(projectile);
                    // don't destroy projectiles since they're in a pool
                }

                //Debug.LogWarning("TODO: unit flew out of range; lower player health or silently delete?");
            }
            //Debug.Log(bypassDestroy);
            if (bypassDestroy) return;
            
            other.gameObject.SetActive(false);
            Destroy(other.gameObject);
        }
    }

}
