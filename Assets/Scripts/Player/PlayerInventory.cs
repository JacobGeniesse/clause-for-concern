using UnityEngine;
using System.Collections.Generic; // needed for list

public enum ItemType{
    Ammo,
    Health
}

public class PlayerInventory : MonoBehaviour
{
   [SerializeField] private List<string> keyItems = new List<string>();
   [SerializeField] private int ammoCount = 0;
   [SerializeField] private int maxAmmo = 5;
   [SerializeField] private int healthCount = 0;
   [SerializeField] private int maxHealth = 5;

   public bool AddKeyItem(string keyname){
    if(keyItems.Contains(keyname)){ // if keyitems has this key then return false
        return false;
    }
    keyItems.Add(keyname); // add key to inventory
    Debug.Log("Picked up key item: " + keyname);
    return true;
   }

   public bool HasKeyItem(string keyName){
    return keyItems.Contains(keyName);
   }

   public bool RemoveKeyItem(string keyName){
    return keyItems.Remove(keyName);
   }

   public int AddItems(ItemType type, int amount){
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
