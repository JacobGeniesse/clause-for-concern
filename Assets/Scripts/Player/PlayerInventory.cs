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


    //[SerializeField] private Dictionary<int, ItemPickup> standardItemInventory = new Dictionary<int, ItemPickup>();     // Standard item inventory dictionary
    //[SerializeField] private Dictionary<int, KeyItemPickup> keyItemInventory = new Dictionary<int, KeyItemPickup>();    // Key item inventory dictionary


   [SerializeField] private List<KeyItemPickup> keyItems = new List<KeyItemPickup>();
    [SerializeField] private List<ItemPickup> standardItems = new List<ItemPickup>();
   [SerializeField] public int ammoCount = 0;
   [SerializeField] private int maxAmmo = 5;
   [SerializeField] private int healthCount = 0;
   [SerializeField] private int maxHealth = 5;

   public bool AddKeyItem(KeyItemPickup keyItem){
    if(keyItems.Contains(keyItem)){ // if keyitems has this key then return false
        return false;
    }
    keyItems.Add(keyItem); // add key to inventory
    Debug.Log("Picked up key item: " + keyItem.keyName);

    return true;
   }

   public bool HasKeyItem(KeyItemPickup keyItem)
    {
    return keyItems.Contains(keyItem);
   }

   public bool RemoveKeyItem(KeyItemPickup keyItem)
    {
    return keyItems.Remove(keyItem);
   }

   public bool AddItems(ItemPickup item){
        /*
    switch(type){ // determining which kind of item was picked up 
        case ItemType.Ammo:
            int ammoRoom = maxAmmo - ammoCount; // 
            int ammoAdded = Mathf.Min(amount, ammoRoom);
            ammoCount += ammoAdded;
            return ammoAdded;
        
        case ItemType.Health:
            int healthRoom = maxHealth - healthCount;
            int healthAdded = Mathf.Min(amount, healthRoom);
            healthCount += healthAdded;
            return healthAdded; 
    }
        */
        standardItems.Add(item);
        Debug.Log("Added " + item.amount + " " + item.itemType);
    return true;
   }

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
                int healthRoom = maxHealth - healthCount;
                int healthAdded = Mathf.Min(item.amount, healthRoom);
                healthCount += healthAdded;
                return healthAdded;

            case ItemType.Speed:
                // Code for activating speedboost item here
                return 0;
        }
        return 0;
    }

   public int GetCount(ItemType type)
    {
        switch (type)
        {
            case ItemType.Ammo:
                return ammoCount;

            case ItemType.Health:
                return healthCount;
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

            case ItemType.Health:
                if (healthCount < amount)
                {
                    return false;
                }
                healthCount -= amount;
                return true;
        }

        return false;
    }
}
