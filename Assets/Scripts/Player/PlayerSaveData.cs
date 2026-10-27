using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    public int health;
    // Item inventory
    // Key item inventory
    // Puzzle status

    public PlayerSaveData(int currentHealth)  // Add the other vairables when they get implemented
    {
        health = currentHealth;
    }
}
