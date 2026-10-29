using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100; // max health of the player
    public float currentHealth; // current health of the player
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth; // sets the current health to the max health
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage){
        currentHealth -= damage; // subtracts the damage from the current health
        if (currentHealth <= 0){
            Die(); // calls the Die function
        }
    }
    private void Die(){
        // TODO: Implement the death logic
        GameOverManager.instance.GameOver();
        Debug.Log("Player has died");
        
    }

    public void Heal(int amount){
        currentHealth += amount; // adds the amount to the current health
        if(currentHealth > maxHealth){
            currentHealth = maxHealth; // sets the current health to the max health
        }
    }
}
