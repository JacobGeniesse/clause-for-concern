using UnityEngine;
using System.Collections.Generic; // needed for list

public enum ItemType{
    Ammo,
    Health,
    Speed
}

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }    // Singleton instance


    


   [SerializeField] private List<KeyItemPickup> keyItems = new List<KeyItemPickup>();   // Inventory list of key items
    [SerializeField] private List<ItemPickup> standardItems = new List<ItemPickup>();   // Inventory list of standard items

    // Do we want to have the stun gun script handle the ammo management, and the inventory just adds ammo from ammo boxes?
   [SerializeField] public int ammoCount = 0;   // Current ammo count of the stapler
   [SerializeField] private int maxAmmo = 5;    // Maximum ammo count for the stapler

   public bool AddKeyItem(KeyItemPickup keyItem){
    if(keyItems.Contains(keyItem)){ // if keyitems has this key then return false
        return false;
    }
    keyItems.Add(keyItem); // add key to inventory
    Debug.Log("Picked up key item: " + keyItem.keyName);

    return true;
   }

    // Check if the key item is needed to perform the specified action
   public bool HasKeyItem(KeyItemPickup keyItem)
    {
    return keyItems.Contains(keyItem);
   }

    // If the key item is present, remove it from the inventory. Key items are revomed automatically when needed and the player cannot use them directly from the inventory
   public bool RemoveKeyItem(KeyItemPickup keyItem)
    {
    return keyItems.Remove(keyItem);
   }

    // Add an item to the standard item inventory
   public bool AddItems(ItemPickup item){
        standardItems.Add(item);
        Debug.Log("Added " + item.amount + " " + item.itemType);
    return true;
   }

    // Call this function to use items from the item inventory
    public int UseItems(ItemPickup item)
    {
        switch (item.itemType)
        {
            case ItemType.Ammo:
                int ammoRoom = maxAmmo - ammoCount;
                int ammoAdded = Mathf.Min(item.amount, ammoRoom);
                ammoCount += ammoAdded;
                return ammoAdded;

            case ItemType.Health:
                // Heal the player's health from the playerhealth script
                gameObject.GetComponent<PlayerHealth>().Heal(item.amount);
                return item.amount;

            case ItemType.Speed:
                // Code for activating speedboost item here, ideally reference the player movement script
                return 0;
        }
        return 0;
    }

    // These functions are only used by the stun gun script
   public int GetCount(ItemType type)
    {
        switch (type)
        {
            case ItemType.Ammo:
                return ammoCount;
        }

        return 0;
    }

    public bool RemoveItem(ItemType type, int amount)
    {
        switch (type)
        {
            case ItemType.Ammo:
                if (ammoCount < amount)
                {
                    return false;
                }
                ammoCount -= amount;
                return true;
        }

        return false;
    }
}
