using System.IO;
using UnityEngine;

public static class SaveSystem
{
    // The different save file names, will be set by the game manager, ideally
    // The 3 save files will be saveFile1, saveFile2, and saveFile3
    // Maybe create all 3 files here, and have the game manager send an id value to the SaveGame function
    public static string fileName = "saveFile1";

    public static string filePath = Path.Combine(Application.persistentDataPath, fileName);

    public static void SaveGame(PlayerSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(filePath, json);
        Debug.Log($"Game saved to {filePath}");
    }

    public static PlayerSaveData LoadGame()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Save file not found");
            return null;
        }

        string json = File.ReadAllText(filePath);
        PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);
        return data;
    }
}
