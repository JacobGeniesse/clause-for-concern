using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    public float health;
    // Item inventory
    // Key item inventory
    // Puzzle status

    public PlayerSaveData(float currentHealth)  // Add the other vairables when they get implemented
    {
        health = currentHealth;
    }
}
