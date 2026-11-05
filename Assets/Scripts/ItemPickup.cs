using System;
using UnityEngine;

[Serializable]
public class ItemPickup : MonoBehaviour, IInteractable
{
    public ItemType itemType;
    public int amount;
    public string description;
    private PlayerInventory inventory; // saved reference to the player's inventory
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = FindAnyObjectByType<PlayerInventory>(); // finds the PlayerInventory in the scene and saves it
    }

    public void Interact()
    {
        if (inventory.AddItems(new StandardItem(itemType, amount, description)))
        {
            gameObject.SetActive(false);
        }
    }
    public void IncrementTask() // required by IInteractable
    {

    }
}
