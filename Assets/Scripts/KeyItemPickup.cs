using UnityEngine;
// Attach this script to key items and name them properly 
public class KeyItemPickup : MonoBehaviour, IInteractable // a Unity script that PlayerInteract can interact with
{
    public string keyName; // the name of the key this pickup gives, typed in the Inspector
    public string description;
    private PlayerInventory inventory; // saved reference to the player's inventory

    void Start()
    {
        inventory = FindAnyObjectByType<PlayerInventory>(); // finds the PlayerInventory in the scene and saves it
        /*
        if (PlayerInventory.Instance != null)
        {
            inventory = PlayerInventory.Instance;
        }
        */
    }

    public void Interact() // called by PlayerInteract when the player looks at this and presses interact
    {
        if (inventory.AddKeyItem(this.GetComponent<KeyItemPickup>()))// adds this key item to the player's key item list
        {
            gameObject.SetActive(false); // removes this pickup from the scene
        } 
    }

    public void IncrementTask() // required by IInteractable
    {
      
    }
}