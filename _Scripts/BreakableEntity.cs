using UnityEngine;
using System.Collections.Concurrent;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))] // I recommend Composite Collider since it is flexible
public abstract class BreakableEntity : VisibleEntity, IBreakable {

    /// <summary>
    /// TODO: consider implementing a damage type system (maybe - will be a lot of work)
    /// </summary>
    public bool broken = false;
    
    [SerializeField]
    private float health = 100;
    private float startHealth;

    public DamageIndicator damageIndicator = DamageIndicator.healthBar;
    //public bool showHealthBar = false;
    public Color damageColor = Color.white;
    private Color startColor;
    public float flashDuration = .5f;
    public GameObject healthBarPrefab;
    public Vector3 healthBarOffset = new Vector3(0, -1, 0);
    public Vector3 healthBarScale = new Vector3(.9f, 1, 1);
    protected Transform healthBar; // the ref to the instantiated healthbar
    SpriteRenderer healthBarSprite;

    private Vector2 startScale; // the scale that the health bar was initially
                                //(simplifies design so that bars of all sizes are uniform)


    // fade control variables TODO: consider moving to options
    private float nextFadeTime = 0;
    public float fadeDelay = .8f;
    private static float healthBarFadeRate = .05f; // the length of the delay between fade increments
    private static float healthBarFadeIncrement = .08f; // the raw change in alpha value while fading
    //private static float flashFadeIncrement = .08f;
    private float fadePercent = 0;
    public Collider2D coll;
    public Rigidbody2D rb;

    // controls how many health modifiers are dequeued per frame -allows player to narrowly escape death if being healed
    //public int maxDamageSourcesPerFrame = 20;
    //readonly ConcurrentQueue<float> queue = new ConcurrentQueue<float>();

    // TODO: each entity has its own break sound effect. when 2 objects collide, consider making both effects play
    // this should give it more variety. Or make it random which is played
    public AudioSource breakSound;

    public float collisionDamage = 2;

    public override void Awake() {
        base.Awake();
        
        rb = GetComponent<Rigidbody2D>();

        if (rb != null) {
            rb.gravityScale = 0;
        }

        if (coll == null) {
            coll = GetComponent<Collider2D>();
        }

        if (spriteRend != null) startColor = spriteRend.color;
        startHealth = GetHealth();
        if (flashDuration == 0) flashDuration = .00001f;
    }

    public override void Start() {
        base.Start();
        
        

        // create health bar (get default if not defined)
        if (healthBarPrefab == null) {
            healthBarPrefab = OptionManager.options.defaultHealthBar;
        }

        // create healthBar and move it in front of entity 
        if (damageIndicator == DamageIndicator.healthBar) { 
            CreateHealthBar(); 
            healthBar.position = new Vector3(healthBar.position.x, healthBar.position.y, -1);
        }
        
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        

        // check for sound effect
        if (breakSound == null) {

            // check self
            breakSound = GetComponent<AudioSource>();
            if (breakSound == null) {
                
                // check default
                breakSound = OptionManager.options.defaultBreakSound;

                // if still null, log warning
                if (breakSound == null) { Debug.LogWarning(gameObject.name + " does not have a breakSound"); }
            }
        }

        //initialize values - useful in case a modifier triggers something else in an overridden method
        ModifyHealth(0);
    }

    public virtual void LateUpdate() {

        //if (healthBar == null) { return; }
        //// reposition healthbar if obj rotates
        //healthBar.position = transform.position + new Vector3(
        //    healthBarOffset.x * healthBar.localScale.x,
        //    healthBarOffset.y * healthBar.localScale.y,
        //    healthBarOffset.z * healthBar.localScale.z
        //    );

        //// counteract parent rotation
        //healthBar.rotation = Quaternion.Euler(new Vector3(
        //    -transform.rotation.x,
        //    -transform.rotation.y,
        //    -transform.rotation.z
        //    ));


    }

    // enqueue health modifiers in a thread-safe way
    public virtual void ModifyHealth(float val) {
        health += val;
        
        // check health
        if (health <= 0) {
            OnBreak(true);
        }

        Debug.Log("asteroids are already white color, so they can't flash; look into shaders");
        switch (damageIndicator) {

            case DamageIndicator.healthBar:
                // set healthBar // don't show healthbar if it is max hp (pointless)
                if (healthBar != null && GetHealth() > 0 && GetHealth() < startHealth) {
            
                    // show health bar
                    healthBar.gameObject.SetActive(true);
                    healthBarSprite.color = new Color(
                        healthBarSprite.color.r,
                            healthBarSprite.color.g,
                            healthBarSprite.color.b,
                            1
                        );
                

                    // scale healthBar
                    healthBar.localScale = new Vector2(startScale.x * (health / startHealth), startScale.y);

                    // begin fade coroutine
                    StartCoroutine(FadeHealthBar());
                }
                break;

            case DamageIndicator.flash:
                //Debug.LogError(GetHealth() + " " + startHealth);
                if (spriteRend != null && GetHealth() > 0 && GetHealth() < startHealth) {
                    
                    // flash when taking damage
                    spriteRend.color = damageColor;


                    // begin fade coroutine
                    StartCoroutine(ResetFlash());
                }
                break;
        }
        
    }

    public float GetHealth() {
        return health;
    }



    public virtual void OnBreak(bool grantReward) {
        if (broken) { return; }
        broken = true;
         // set health to some number below 0;
        // 0 should work, but I might as well go lower just to be sure this object dies
        //health = -100000000;

        // play break sound effect
        breakSound.Play();

        // hide and disable object. mark for deletion
        
        gameObject.SetActive(false);
        enabled = false; 
        Destroy(gameObject);
        
    }

    public virtual void CreateHealthBar() {

        healthBar = Instantiate(healthBarPrefab,  transform).transform;

        // hide healthbar if not damaged
        healthBar.gameObject.SetActive(false);

        // set physics layer to match parent (used by several components)
        healthBar.gameObject.layer = gameObject.layer;

        // set initial position and scale
        healthBar.localPosition = healthBarOffset;

        healthBar.localScale = new Vector3(healthBar.localScale.x * healthBarScale.x,
            healthBar.localScale.y * healthBarScale.y,
            healthBar.localScale.z * healthBarScale.z);
        
        // get initial scaling params
        startHealth = health;
        startScale = healthBar.localScale;

        // get ref to spriterenderer
        healthBarSprite = healthBar.GetComponent<SpriteRenderer>();
    }

    IEnumerator FadeHealthBar() {

        // slowly fade until close to 0, then skip to 0
        if (healthBarSprite != null && healthBarSprite.color.a > 0) {

            // get control time value
            nextFadeTime = Time.time + fadeDelay;

            // wait until time to fade
            yield return new WaitForSeconds(fadeDelay);
            StartCoroutine(FadeHealthBarHelper());
        }
    }

    IEnumerator FadeHealthBarHelper() {
        // incrementally fade
        for (float a = healthBarSprite.color.a; Time.time >= nextFadeTime && a >= 0; a -= healthBarFadeIncrement) {

            healthBarSprite.color = new Color(
                    healthBarSprite.color.r,
                    healthBarSprite.color.g,
                    healthBarSprite.color.b,
                    // should fade in {healthBarFadeRate} seconds 
                    Mathf.Clamp01(
                        (a >= .05f) ?
                            a : 0
                        )
                );
            yield return new WaitForSeconds(healthBarFadeRate);
        }
    }

    IEnumerator ResetFlash() {

        //////// This barely works. 
        
        // slowly fade until close to 0, then skip to 0
        if (Time.time >= nextFadeTime) {

            // get control time value
            nextFadeTime = Time.time + fadeDelay;

            // wait until time to fade
            yield return new WaitForSeconds(fadeDelay);
            StartCoroutine(ResetFlashHelper());
        }
    }

    IEnumerator ResetFlashHelper() {
        // incrementally fade

        //float percent = flashDuration / Time.deltaTime;
        Color dist = (damageColor - startColor);

        for (float percent = 0; 
            Time.time >= nextFadeTime && percent < 100; percent += Time.deltaTime / flashDuration) {

            
            spriteRend.color = new Color(
                Mathf.Clamp(damageColor.r - dist.r * percent, startColor.r, damageColor.r),
                Mathf.Clamp(damageColor.g - dist.g * percent, startColor.g, damageColor.g),
                Mathf.Clamp(damageColor.b - dist.b * percent, startColor.b, damageColor.b),
                // should fade in {healthBarFadeRate} seconds 
                Mathf.Clamp(damageColor.a - dist.a * percent, startColor.a, damageColor.a)
            );
            //Debug.LogWarning(damageColor + " - " + spriteRend.color + " - " + startColor + " - " + dist + " - " + percent);
            //yield return new WaitForSeconds(healthBarFadeRate);
        }
        
        spriteRend.color = startColor;
        yield return new WaitForSeconds(fadeDelay);
    }

    private float SumColor(Color c) {
        return c.r + c.g + c.b + c.a;
    }

}

public enum DamageIndicator {
    none,
    healthBar,
    flash
}