using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100; // max health of the player
    public int currentHealth; // current health of the player
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth; // sets the current health to the max health
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage){
        currentHealth -= damage; // subtracts the damage from the current health
        if(currentHealth <= 0){
            Die(); // calls the Die function
        }
    }
    private void Die(){
        // TODO: Implement the death logic
        Debug.Log("Player has died");
        
    }

    public void Heal(int amount){
        currentHealth += amount; // adds the amount to the current health
        if(currentHealth > maxHealth){
            currentHealth = maxHealth; // sets the current health to the max health
        }
    }
}
