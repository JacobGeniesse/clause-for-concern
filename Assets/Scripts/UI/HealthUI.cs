using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{

    [SerializeField] Image healthBar;
    [SerializeField] PlayerHealth playerHealth;
    
    private void OnEnable()
    {
        if(playerHealth == null)
        {
            Debug.Log(" Health UI has not been assigned.");
            return;
        }
        playerHealth.OnHealthChanged += UpdateHealthUI; // assigned the event
        UpdateHealthUI(playerHealth.currentHealth);
    }
    private void OnDisable()
    {
        if (playerHealth == null)
        {
            Debug.Log(" Health UI has not been assigned.");
            return;
        }
        playerHealth.OnHealthChanged -= UpdateHealthUI; // assigned the event
    }
    private void UpdateHealthUI(float health)
    {
        float percent = health / playerHealth.maxHealth; // gets the percetage of current health
        if(percent > .66f) // if health is above 66 percent then make it green
        {
            healthBar.color = Color.green;
        }
        else if(percent > .33f) // if health is above 33 percent then make it yellow
        {
            healthBar.color = Color.yellow;
        }
        else // make it red
        {
            healthBar.color = Color.red;
        }

    }
}
