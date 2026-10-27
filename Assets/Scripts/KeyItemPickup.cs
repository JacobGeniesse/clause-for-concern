using UnityEngine; // lets us use Unity stuff like MonoBehaviour and Destroy
// Attach this script to key items and name them properly 
public class KeyItemPickup : MonoBehaviour, IInteractable // a Unity script that PlayerInteract can interact with
{
    [SerializeField] string keyName; // the name of the key this pickup gives, typed in the Inspector
    private PlayerInventory inventory; // saved reference to the player's inventory

    void Start()
    {
        inventory = FindAnyObjectByType<PlayerInventory>(); // finds the PlayerInventory in the scene and saves it
    }

    public void Interact() // called by PlayerInteract when the player looks at this and presses interact
    {
        if (inventory.AddKeyItem(keyName))// adds this key's name to the player's key item list
        { 
            Destroy(gameObject); // removes this pickup from the scene
        } 
    }

    public void IncrementTask() // required by IInteractable
    {
      
    }
}