using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using System;

public class InputManager : MonoBehaviour
{
    public float clickRadius = 1.2f;
    // https://answers.unity.com/questions/8715/how-do-i-use-layermasks.html
    public LayerMask hitLayers;

    public float minZ = 0;
    public float maxZ = 100;
    // initial position of object relative to touch
    public Vector2 offset = new Vector2(0, 1.25f);

    public Tower heldTower = null;

    List<Vector2> touchPositions;

    public Rigidbody2D castleRb;

    public float targetFlightHeight;
    private float lastTargetFlightHeight;
    public float idleFlightHeight;
    [Range(0, 100)]
    public float maxFlightHeight;
    private float _maxFlightHeight;
    [Range(0, 100)]
    public float minFlightHeight;
    private float _minFlightHeight;
    public float deadZone = .1f;
    private float deadZoneTop, deadZoneBottom;
    public float slowZone = 2;
    private float slowZoneTop, slowZoneBottom;
    
    [Range(0, 100)]
    public float maxTouchHeight = .85f;
    private float maxTouchHeightPos;
    [Range(0, 100)]
    public float minTouchHeight = .15f;
    private float minTouchHeightPos;

    public float targetFlightAngle = 0;

    

    //private int flightFrameCount = 0;
    //private int rotationFrameCount = 0;
    public int flightDirection = 0;
    public int flightDirectionCap = 100;
    public float flightVelocity = 0;

    public void Start() {
        // set default height to center of screen
        idleFlightHeight = Camera.main.ViewportToWorldPoint(new Vector2(.5f, .5f)).y;
        targetFlightHeight = idleFlightHeight;
        lastTargetFlightHeight = targetFlightHeight;

        flightVelocity = 0;
        targetFlightAngle = 0; // flight straight

        castleRb = Castle.castle.GetComponent<Rigidbody2D>();

        // get flight bounds
        _maxFlightHeight = Camera.main.ViewportToWorldPoint(new Vector2(0, maxFlightHeight / 100)).y;
        _minFlightHeight = Camera.main.ViewportToWorldPoint(new Vector2(0, minFlightHeight / 100)).y;
        slowZoneTop = _maxFlightHeight - slowZone;
        slowZoneBottom = _minFlightHeight + slowZone;
        deadZoneTop = _maxFlightHeight - deadZone;
        deadZoneBottom = _minFlightHeight + deadZone;

        // get touch bounds
        maxTouchHeightPos = Camera.main.ViewportToWorldPoint(new Vector2(0, Mathf.Clamp01(maxTouchHeight / 100))).y;
        minTouchHeightPos = Camera.main.ViewportToWorldPoint(new Vector2(0, Mathf.Clamp01(minTouchHeight / 100))).y;
    
        

    }

    public void Update()
    {
        if (GameControl.control.state == GameState.Defeat || GameControl.control.state == GameState.Victory) { return; }

        // reset touches
        touchPositions = new List<Vector2>();

        //// capture touches + clicks
        for(int i = 0; i < 2 && i < Input.touchCount; i++) {
            Touch touch = Input.GetTouch(i);
            
            //if (touch != null) {
            touchPositions.Add(touch.position);
            //}
        }
        if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1)) {
            touchPositions.Add(Input.mousePosition);
        }

        // flight controls and hover when idle
        // todo: float  up and down in a slow wavy pattern to make it feel alive
        //HandleFlightWithInput();

        //  place towers during prep phase
        if (CanPlaceTowers()) {

            // initial
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1)) {
                HandleTouch(Input.mousePosition);
            }
        
            // holding 
            if (heldTower != null) {
                // convert pixel to world coords
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                // move tower
                heldTower.transform.position = worldPos + offset;
                // draw anchor each time
                heldTower.PlaceAnchor();
            }


            // release // handles an instance where the update doesn't catch release also
            if ( !((Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1))) ) { 
                HandleTowerRelease(); 
           
            }
        }
               
    }

    private bool CanPlaceTowers() {
        return 
            GameControl.control.state == GameState.Preparation ||
            //GameControl.control.state == GameState.Menu ||
            GameControl.control.state == GameState.Combat;
    }

    private void HandleFlightWithInput() {
        // top speed
        float topSpeed = Castle.castle.flightSpeed * flightDirection;
        // standard acceleration
        float targetVelocity = flightVelocity + Castle.castle.flightAcceleration * Time.deltaTime * flightDirection;


        if (GameControl.control.state == GameState.Combat) {
            bool validTouch = false;
            foreach (var touch in touchPositions) {

                // validate touch
                var point = Camera.main.ScreenToWorldPoint(touch);
                if (point.y > minTouchHeightPos && point.y < maxTouchHeightPos) {

                    validTouch = true;
                }
            }

            // increase or decrease speed
            if (touchPositions.Count > 0 && validTouch) {
                flightDirection = 1;
            }
            else {
                flightDirection = -1;
            }

            
            
            // limit flight range // != 0 solves a bug that occurs when velocity is 0 at the top deadzone
            if (castleRb.velocity.y != 0) {
                if (castleRb.velocity.y > 0) {
                    if (castleRb.position.y >= slowZoneTop) {
                        // get % of distance to edge
                        targetVelocity *= Mathf.Clamp01(Mathf.Abs(_maxFlightHeight - castleRb.position.y) /
                            slowZone);

                        // remove jittery movement by having a deadZone
                        //if (castleRb.position.y >= deadZoneTop) {
                        //    // 0 velocity; set pos manually
                        //    //flightVelocity = 0;

                        //    castleRb.position = new Vector3(
                        //        castleRb.position.x,
                        //        _maxFlightHeight,
                        //        0);
                        //}
                    }
                }
                else {
                    if (castleRb.position.y <= slowZoneBottom) {
                        // get % of distance to edge
                        targetVelocity *= Mathf.Clamp01(Mathf.Abs(_minFlightHeight - castleRb.position.y) /
                            slowZone);

                        //// remove jittery movement by having a deadZone
                        //if (castleRb.position.y <= deadZoneBottom) {
                        //    // 0 velocity; set pos manually
                        //    //flightVelocity = 0;

                        //    castleRb.position = new Vector3(
                        //        castleRb.position.x,
                        //        _minFlightHeight,
                        //        0);
                        //}
                    }
                }
            }
        }
        // hover outside of combat
        else {
            // move toward idleFlightHeight
             targetVelocity = idleFlightHeight - castleRb.position.y;
            
        }
        //Debug.LogError("Ship sinks below bottom border. consider slingshotting ship if it goes beyond border somehow");

        // return the # closer to zero either velocity or top speed. The abs value makes it
        // simpler than having if statements everywhere
        flightVelocity = (Mathf.Abs(targetVelocity) <= Mathf.Abs(topSpeed)) ?
            targetVelocity :
            topSpeed;

        // set velocity
        castleRb.velocity = new Vector2(
            0,
            flightVelocity
        );

        // calculate % of topSpeed
        float percentSpeed = castleRb.velocity.y / Castle.castle.flightSpeed;

        // angle ship proportionally to % of topSpeed
        castleRb.transform.rotation = Quaternion.Euler(0, 0,
                Castle.castle.flightAngle * percentSpeed
            );

    }

    private void HandleTouch(Vector3 pos) {
        // check collider
        Collider2D hit = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(pos), clickRadius, hitLayers, minZ, maxZ);

        if (hit == null) { return; }

        // if collider is tower...
        var tower = hit.GetComponentInParent<Tower>();
        if (tower != null) {
            HandleTowerClick(tower);
        }
    }

    private void HandleTowerClick(Tower tower) {
        if (tower.moveable) {
            // TODO: perhaps offset should be a constant distance above touch/mouse
            //offset = tower.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            heldTower = tower;

        }
    }

    private void HandleTowerRelease() {
        if (heldTower == null) return; // can't release nothing

        // try to place tower; if false (too far) destroy it
        heldTower.ConfirmAnchor();
        heldTower = null;
    }

}
