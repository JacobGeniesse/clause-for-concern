using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    private Minimap map;
    [SerializeField] private InputActionAsset masterList;
    private InputAction pause;

    private bool paused = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pause = masterList["Pause"];

        map = FindAnyObjectByType<Minimap>();

        // Make it so the main pause menu is active when pause is hit
        pauseMenu.GetComponent<PauseMenu>().ResetPauseMenu();
        pauseMenu.SetActive(false);
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
        map.SetOpen(false, true);
        ModTime(0);
        paused = true;
        Cursor.lockState = CursorLockMode.None;
        pauseMenu.SetActive(true);
        pauseMenu.GetComponent<PauseMenu>().ResetPauseMenu();
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
    }
}
