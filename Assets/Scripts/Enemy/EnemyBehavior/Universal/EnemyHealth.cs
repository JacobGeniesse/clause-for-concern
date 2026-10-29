using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Tooltip("Maximum health the enemy can have.")]
    [SerializeField] private float maxHealth = 1;
    private float currentHealth; //Current health of the enemy

    [Tooltip("Is the enemy dead? DON'T TOUCH UNLESS DEBUGGING!")]
    public bool dead; //Is the enemy dead?

    //On enabling the enemy set the current health to the max health and have the enemy not be dead
    void OnEnable()
    {
        currentHealth = maxHealth; 
        dead = false;
    }

    //Func for taking damage

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount; //Subtract the damage from the current health

        //If after taking damage the enemy's current health is below zero the enemy dies
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(float damageAmount, float damageModifier)
    {
        //Subtract the damage from the current health times the damageModifier of the spot hit
        currentHealth -= damageAmount * damageModifier;  

        //If after taking damage the enemy's current health is below zero the enemy dies
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    //Function for resetting the enemy's health
    public void ResetHealth(float additionalHealth)
    {
        dead = false; //Mark the enemy as dead
        maxHealth += additionalHealth; //Increase the enemy's base health by a certain amount
        currentHealth = maxHealth; //Reset the enemy's health
    }

    //Func for the enemy dying
    public void Die()
    {
        dead = true; //Set the dead bool to true
    }
}
