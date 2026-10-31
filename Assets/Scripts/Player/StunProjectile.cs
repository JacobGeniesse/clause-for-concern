using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class StunProjectile : MonoBehaviour
{
    [SerializeField]
    private int lifetime = 5; // how long the projectile lasts
    private Rigidbody rb;
    private int damage = 1; // how long the enemy is stunned for 

    void Awake(){
        rb = GetComponent<Rigidbody>(); // get the rigidbody component
    }

    public void Fire(Vector3 velocity, int duration){
        //stunDuration = duration; // set the stun duration
        rb.linearVelocity = velocity; // sends the bullet flying
        Destroy(gameObject, lifetime); // destroy the bullet after it's lifetime
    }

    void OnCollisionEnter(Collision collision){
         

        EnemyHealth enemy = collision.collider.GetComponentInParent<EnemyHealth>(); // find the collider of the enemy 
        

        if(enemy != null && enemy.CompareTag("Enemy")){
            enemy.TakeDamage(damage); // stuns the enemy 
        }
        Destroy(gameObject);
    }
}
