using UnityEngine;
using System;
using System.Collections.Generic; // needed for list

public enum ItemType{
    Ammo,
    Health,
    Speed
}

[Serializable]
public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }    // Singleton instance

    public event Action InventoryChanged; // phone inventory UI rebuilds on pickup/use

    private List<StandardItem> standardItemInventory = new List<StandardItem>();        // Inventory list of standard items
    private List<KeyItem> keyItemInventory = new List<KeyItem>();                       // Inventory list of key items

    public IReadOnlyList<KeyItem> KeyItems => keyItemInventory; // UI reads these; change only through Add/Remove/Use
    public IReadOnlyList<StandardItem> StandardItems => standardItemInventory;

    // Do we want to have the stun gun script handle the ammo management, and the inventory just adds ammo from ammo boxes?
   [SerializeField] public int ammoCount = 0;   // Current ammo count of the stapler
   [SerializeField] public int maxAmmo = 5;    // Maximum ammo count for the stapler

    public event Action<int> OnAmmoChanged;

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

   public bool AddKeyItem(KeyItem keyItem){
    if(keyItemInventory.Contains(keyItem)){ // if keyitems has this key then return false
        return false;
    }
    keyItemInventory.Add(keyItem); // add key to inventory
    InventoryChanged?.Invoke();

    return true;
   }

    // Check if the key item is needed to perform the specified action
   public bool HasKeyItem(KeyItem keyItem)
    {
    return keyItemInventory.Contains(keyItem);
   }

    // If the key item is present, remove it from the inventory. Key items are revomed automatically when needed and the player cannot use them directly from the inventory
   public bool RemoveKeyItem(KeyItem keyItem)
    {
    bool removed = keyItemInventory.Remove(keyItem);
    if (removed)
    {
        InventoryChanged?.Invoke();
    }
    return removed;
   }

    // Add an item to the standard item inventory
   public bool AddItems(StandardItem item){
        standardItemInventory.Add(item);
        InventoryChanged?.Invoke();
    return true;
   }

    // Call this function to use items from the item inventory
    public void UseItems(StandardItem item)
    {
        if (item == null || !standardItemInventory.Contains(item))
        {
            return;
        }

        int used = 0; // how many of this stack were actually consumed
        switch (item.type)
        {
            case ItemType.Ammo:
                int ammoRoom = maxAmmo - ammoCount;
                used = Mathf.Min(item.amount, ammoRoom); // partial fill if the stapler is almost full
                ammoCount += used;
                OnAmmoChanged?.Invoke(ammoCount);
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
            standardItemInventory.Remove(item);
        }

        if (used > 0)
        {
            InventoryChanged?.Invoke();
        }
    }

    public List<StandardItem> GetStandardInventory()
    {
        return standardItemInventory;
    }

    public List<KeyItem> GetKeyItemInventory()
    {
        return keyItemInventory;
    }

    public void SetStandardInventory(List<StandardItem> savedList)
    {
        standardItemInventory = savedList;
    }

    public void SetKeyItemInventory(List<KeyItem> savedKeyList)
    {
        keyItemInventory = savedKeyList;
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
                OnAmmoChanged?.Invoke(ammoCount); // onAmmoChanged is event for ui, ?. checking if the event has a subscriber, Invoke runs the event ammocount the passed value
                return true;
        }

        return false;
    }
}
