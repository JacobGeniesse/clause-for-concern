using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] ItemType itemType;
    [SerializeField] int amount;
    private PlayerInventory inventory; // saved reference to the player's inventory
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = FindAnyObjectByType<PlayerInventory>(); // finds the PlayerInventory in the scene and saves it
    }

    public void Interact()
    {
        if (inventory.AddItems(itemType, amount) > 0)
        {
           Destroy(gameObject);
        }
    }
    public void IncrementTask() // required by IInteractable
    {

    }
}
