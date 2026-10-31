using UnityEngine;
using System;
using System.Collections.Generic; // needed for list

public enum ItemType{
    Ammo,
    Health,
    Speed
}

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }    // Singleton instance

    public event Action InventoryChanged; // phone inventory UI rebuilds on pickup/use

   [SerializeField] private List<KeyItemPickup> keyItems = new List<KeyItemPickup>();   // Inventory list of key items
    [SerializeField] private List<ItemPickup> standardItems = new List<ItemPickup>();   // Inventory list of standard items

    public IReadOnlyList<KeyItemPickup> KeyItems => keyItems; // UI reads these; change only through Add/Remove/Use
    public IReadOnlyList<ItemPickup> StandardItems => standardItems;

    // Do we want to have the stun gun script handle the ammo management, and the inventory just adds ammo from ammo boxes?
   [SerializeField] public int ammoCount = 0;   // Current ammo count of the stapler
   [SerializeField] public int maxAmmo = 5;    // Maximum ammo count for the stapler

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

   public bool AddKeyItem(KeyItemPickup keyItem){
    if(keyItems.Contains(keyItem)){ // if keyitems has this key then return false
        return false;
    }
    keyItems.Add(keyItem); // add key to inventory
    Debug.Log("Picked up key item: " + keyItem.keyName);
    InventoryChanged?.Invoke();

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
    bool removed = keyItems.Remove(keyItem);
    if (removed)
    {
        InventoryChanged?.Invoke();
    }
    return removed;
   }

    // Add an item to the standard item inventory
   public bool AddItems(ItemPickup item){
        standardItems.Add(item);
        Debug.Log("Added " + item.amount + " " + item.itemType);
        InventoryChanged?.Invoke();
    return true;
   }

    // Call this function to use items from the item inventory
    public void UseItems(ItemPickup item)
    {
        if (item == null || !standardItems.Contains(item))
        {
            return;
        }

        int used = 0; // how many of this stack were actually consumed
        switch (item.itemType)
        {
            case ItemType.Ammo:
                int ammoRoom = maxAmmo - ammoCount;
                used = Mathf.Min(item.amount, ammoRoom); // partial fill if the stapler is almost full
                ammoCount += used;
                item.amount -= used;
                break;

            case ItemType.Health:
                // Heal the player's health from the playerhealth script
                PlayerHealth health = gameObject.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.Heal(item.amount);
                    used = item.amount;
                    item.amount = 0;
                }
                break;

            case ItemType.Speed:
                // Code for activating speedboost item here, ideally reference the player movement script
                break;
        }

        if (item.amount <= 0)
        {
            standardItems.Remove(item);
        }

        if (used > 0)
        {
            InventoryChanged?.Invoke();
        }
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
