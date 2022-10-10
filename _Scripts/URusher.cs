using UnityEngine;

public abstract class URusher : Unit {

    public float moveSpeed = 2;
    public float damage = 2;
    
    public void OnCollisionEnter2D(Collision2D other) {
        
        // check if unit collided and apply damage
        if (other.gameObject.GetComponent<BreakableEntity>() != null) {
            //Debug.Log(gameObject.name + " collided with castle");

            // damage castle
            other.gameObject.GetComponent<BreakableEntity>().ModifyHealth(-collisionDamage);

            // destroy self
            OnBreak(true);
            
        }
    }
}
