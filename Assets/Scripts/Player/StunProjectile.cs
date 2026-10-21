using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class StunProjectile : MonoBehaviour
{
    [SerializeField]
    private int lifetime = 5; // how long the projectile lasts
    private Rigidbody rb;
    private int stunDuration; // how long the enemy is stunned for 

    void Awake(){
        rb = GetComponent<Rigidbody>(); // get the rigidbody component
    }

    public void Fire(Vector3 velocity, int duration){
        stunDuration = duration; // set the stun duration
        rb.linearVelocity = velocity; // sends the bullet flying
        Destroy(gameObject, lifetime); // destroy the bullet after it's lifetime
    }

    void OnCollisionEnter(Collision collision){
         

        EnemyStun enemy = collision.collider.GetComponentInParent<EnemyStun>(); // find the collider of the enemy 
        

        if(enemy != null && enemy.CompareTag("Enemy")){
            enemy.Stun(stunDuration); // stuns the enemy 
        }
        Destroy(gameObject);
    }
}
