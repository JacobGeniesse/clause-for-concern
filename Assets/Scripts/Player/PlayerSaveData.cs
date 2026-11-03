using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

[System.Serializable]
public class PlayerSaveData
{
    public float health;
    public int ammo;
    public List<StandardItem> standardInventory;
    public List<KeyItem> keyItemInventory;
    // Puzzle status

    public PlayerSaveData(float currentHealth, int currentAmmo, List<StandardItem> currentItemInventory, List<KeyItem> currentKeyItemInventory)  // Add the other vairables when they get implemented
    {
        health = currentHealth;
        ammo = currentAmmo;
        standardInventory = currentItemInventory;
        keyItemInventory = currentKeyItemInventory;
    }
}
