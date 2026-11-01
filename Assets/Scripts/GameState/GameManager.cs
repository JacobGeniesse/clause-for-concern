using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Duplicate GameManager instance found in scene!");
            Destroy(this);
        }
    }
    [SerializeField] private GameObject pauseMenu;
    private Minimap map;
    [SerializeField] private InputActionAsset masterList;
    private InputAction pause;
    [SerializeField] private GameObject player;

    private bool paused = false;

    [SerializeField] private int saveFileId;

    private SaveDataManager saveDataManager;

    private PlayerInventory playerInventory;

    private PlayerHealth playerHealth;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pause = masterList["Pause"];

        map = FindAnyObjectByType<Minimap>();

        if (SaveDataManager.Instance != null)
        {
            saveDataManager = SaveDataManager.Instance;
        }

        if (PlayerInventory.Instance != null)
        {
            playerInventory = PlayerInventory.Instance;
        }

        if (PlayerHealth.Instance != null)
        {
            playerHealth = PlayerHealth.Instance;
            
        }

        // Set the save file id from the save data manager singleton
        if (saveDataManager != null )
        {
            saveFileId = saveDataManager.fileId;

            if (!saveDataManager.newGame)
            {
                // Load save data right at the start if not a new game
                LoadData();
            }
            else
            {
                // Set the player's health to 100 on a new game
                playerHealth.SetHealth(100);
            }
        }
        else
        {
            // If no save data manager is present, default to id 0
            saveFileId = 0;
        }

        // Make it so the main pause menu is active when pause is hit
        // Essentially reset the pause menu
        UnPause();
    }

    private void Update()
    {
        if (pause.WasPressedThisFrame())
        {
            if(paused == true)
            {
                UnPause();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        if(map != null)
        {
            map.SetOpen(false, true);
        }
        ModTime(0);
        paused = true;
        Cursor.lockState = CursorLockMode.None;
        if(pauseMenu != null)
        {
            pauseMenu.SetActive(true);
            pauseMenu.GetComponent<PauseMenu>().ResetPauseMenu();   // Make it so the main pause menu is active when you close and open the pause menu
        }
        masterList.FindAction("Fire").Disable();             // Disable using items when the pause menu is active, so as to prevent inputs from accumilating
    }

    public void ModTime(int scale)
    {
        Time.timeScale = scale;
    }

    public void UnPause()
    {
        paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        if(pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        ModTime(1);
        masterList.FindAction("Fire").Enable();  // Reenable using items when the menu is closed
    }

    public void SaveData()
    {
        PlayerSaveData data = new PlayerSaveData(playerHealth.currentHealth, playerInventory.ammoCount, playerInventory.GetStandardInventory(), playerInventory.GetKeyItemInventory());
        SaveSystem.SaveGame(data, saveFileId);
    }

    public void LoadData()
    {
        PlayerSaveData loadedData = SaveSystem.LoadGame(saveFileId);

        if (loadedData != null)
        {
            playerHealth.currentHealth = loadedData.health;
            playerInventory.ammoCount = loadedData.ammo;
            playerInventory.SetStandardInventory(loadedData.standardInventory);
            playerInventory.SetKeyItemInventory(loadedData.keyItemInventory);
            Debug.Log("Data loaded successfully");
        }
    }
}
