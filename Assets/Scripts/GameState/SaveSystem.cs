using System.IO;
using UnityEngine;

public static class SaveSystem
{
    // The different save file names, will be set by the game manager, ideally
    // The 3 save files will be saveFile1, saveFile2, and saveFile3
    // Maybe create all 3 files here, and have the game manager send an id value to the SaveGame function
    //public static string fileName = "saveFile1";

    public static string filePath1 = Path.Combine(Application.persistentDataPath, "saveFile1");
    public static string filePath2 = Path.Combine(Application.persistentDataPath, "saveFile2");
    public static string filePath3 = Path.Combine(Application.persistentDataPath, "saveFile3");

    public static void SaveGame(PlayerSaveData data, int id)
    {
        string json = JsonUtility.ToJson(data, true);

        switch (id)
        {
            case 0:
                File.WriteAllText(filePath1, json);
                Debug.Log($"Game saved to {filePath1}");
                break;
            case 1:
                File.WriteAllText(filePath2, json);
                Debug.Log($"Game saved to {filePath2}");
                break;
            case 2:
                File.WriteAllText(filePath3, json);
                Debug.Log($"Game saved to {filePath3}");
                break;
            default:
                Debug.LogError("Invalid save file id");
                break;
        }
    }

    public static PlayerSaveData LoadGame(int id)
    {
        string json;
        PlayerSaveData data;

        switch (id)
        {
            case 0:
                if (!File.Exists(filePath1))
                {
                    Debug.LogWarning("Save file not found");
                    return null;
                }
                json = File.ReadAllText(filePath1);
                data = JsonUtility.FromJson<PlayerSaveData>(json);
                return data;
            case 1:
                if (!File.Exists(filePath2))
                {
                    Debug.LogWarning("Save file not found");
                    return null;
                }
                json = File.ReadAllText(filePath2);
                data = JsonUtility.FromJson<PlayerSaveData>(json);
                return data;
            case 2:
                if (!File.Exists(filePath3))
                {
                    Debug.LogWarning("Save file not found");
                    return null;
                }
                json = File.ReadAllText(filePath3);
                data = JsonUtility.FromJson<PlayerSaveData>(json);
                return data;
            default:
                Debug.LogError("Invalid save file id");
                return null;
        }
    }
}
