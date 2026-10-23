using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    private Minimap map;
    [SerializeField] private InputActionAsset masterList;
    private InputAction pause;
    [SerializeField] private GameObject player;

    private bool paused = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pause = masterList["Pause"];

        map = FindAnyObjectByType<Minimap>();

        // Load save data right at the start
        LoadData();

        // Make it so the main pause menu is active when pause is hit
        //pauseMenu.GetComponent<PauseMenu>().ResetPauseMenu();
        //pauseMenu.SetActive(false);

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
        map.SetOpen(false);
        ModTime(0);
        paused = true;
        Cursor.lockState = CursorLockMode.None;
        pauseMenu.SetActive(true);
        pauseMenu.GetComponent<PauseMenu>().ResetPauseMenu();   // Make it so the main pause menu is active when you close and open the pause menu
        masterList.FindAction("UseItem").Disable();             // Disable using items when the pause menu is active, so as to prevent inputs from accumilating
    }

    public void ModTime(int scale)
    {
        Time.timeScale = scale;
    }

    public void UnPause()
    {
        paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenu.SetActive(false);
        ModTime(1);
        masterList.FindAction("UseItem").Enable();  // Reenable using items when the menu is closed
    }

    public void SaveData()
    {
        PlayerSaveData data = new PlayerSaveData(player.GetComponent<PlayerHealth>().currentHealth);
        //SaveSystem.fileName = "saveFile1";  // Make it so this can by dynamically changed 
        SaveSystem.SaveGame(data);
    }

    public void LoadData()
    {
        PlayerSaveData loadedData = SaveSystem.LoadGame();

        if (loadedData != null)
        {
            player.GetComponent<PlayerHealth>().currentHealth = loadedData.health;

            Debug.Log("Data loaded successfully");
        }
    }
}
