using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    // Save data manager singleton
    public static SaveDataManager Instance { get; private set; }

    // Default file id to 0
    public int fileId = 0;

    public bool newGame = false;

    private void Awake()
    {
        // Enforce singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
