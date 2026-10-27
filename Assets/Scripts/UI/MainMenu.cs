using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsMenuPanel;
    [SerializeField] private GameObject startMenuPanel;
    [SerializeField] private GameObject controllerMenuPanel;
    [SerializeField] private GameObject volumeMenuPanel;
    [SerializeField] private GameObject keyboardBindings;
    [SerializeField] private GameObject controllerBindings;
    [SerializeField] private GameObject fileLoadPanel;
    [SerializeField] private GameObject newGamePanel;
    [SerializeField] private GameObject deleteFilePanel;
    [SerializeField] private GameObject keyboardButtonBackground;
    [SerializeField] private GameObject controllerButtonBackground;
    [SerializeField] private GameObject confirmFile1Deletion;
    [SerializeField] private GameObject confirmFile2Deletion;
    [SerializeField] private GameObject confirmFile3Deletion;

    [SerializeField] private Button loadFile1Button;
    [SerializeField] private Button loadFile2Button;
    [SerializeField] private Button loadFile3Button;

    [SerializeField] private Button deleteFile1Button;
    [SerializeField] private Button deleteFile2Button;
    [SerializeField] private Button deleteFile3Button;

    private SaveDataManager saveDataManager;


    private void Start()
    {
        if (SaveDataManager.Instance != null)
        {
            saveDataManager = SaveDataManager.Instance;
        }

        // Make it so the main menu panel is the one that's active at the start
        mainMenuPanel.SetActive(true);
        settingsMenuPanel.SetActive(false);
        startMenuPanel.SetActive(false);
        controllerMenuPanel.SetActive(false);
        volumeMenuPanel.SetActive(false);
        fileLoadPanel.SetActive(false);
        newGamePanel.SetActive(false);
        deleteFilePanel.SetActive(false);
        confirmFile1Deletion.SetActive(false);
        confirmFile2Deletion.SetActive(false);
        confirmFile3Deletion.SetActive(false);

        // Have the keyboard bindings be active first
        keyboardBindings.SetActive(true);
        keyboardButtonBackground.SetActive(true);
        controllerBindings.SetActive(false);
        controllerButtonBackground.SetActive(false);

        // Enable the load and delete file buttons only if a save file exists already for them
        CheckFile1Button();
        CheckFile2Button();
        CheckFile3Button();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SwitchToSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsMenuPanel.SetActive(true);
    }

    public void SwitchFromSettings()
    {
        settingsMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void SwitchToStart()
    {
        mainMenuPanel.SetActive(false);
        startMenuPanel.SetActive(true);
    }

    public void SwitchFromStart()
    {
        startMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void SwitchToController()
    {
        settingsMenuPanel.SetActive(false);
        controllerMenuPanel.SetActive(true);
    }

    public void SwitchFromController()
    {
        controllerMenuPanel.SetActive(false);
        settingsMenuPanel.SetActive(true);
    }

    public void SwitchToVolume()
    {
        settingsMenuPanel.SetActive(false);
        volumeMenuPanel.SetActive(true);
    }

    public void SwitchFromVolume()
    {
        volumeMenuPanel.SetActive(false);
        settingsMenuPanel.SetActive(true);
    }

    public void SwitchToKeyboardBindings()
    {
        keyboardBindings.SetActive(true);
        keyboardButtonBackground.SetActive(true);
        controllerBindings.SetActive(false);
        controllerButtonBackground.SetActive(false);
    }

    public void SwitchToControllerBindings()
    {
        controllerBindings.SetActive(true);
        controllerButtonBackground.SetActive(true);
        keyboardBindings.SetActive(false);
        keyboardButtonBackground.SetActive(false);
    }

    public void OpenFileLoadPanel()
    {
        fileLoadPanel.SetActive(true);
    }

    public void CloseFileLoadPanel()
    {
        fileLoadPanel.SetActive(false);
    }

    public void OpenNewGamePanel()
    {
        newGamePanel.SetActive(true);
    }

    public void CloseNewGamePanel()
    {
        newGamePanel.SetActive(false);
    }

    public void OpenDeleteGamePanel()
    {
        deleteFilePanel.SetActive(true);
    }

    public void CloseDeleteGamePanel()
    {
        deleteFilePanel.SetActive(false);
    }

    public void StartGame()
    {
        // Eventually change this to also set all savable data to starting values
        // Also change this to load the main game scene when that gets implemented
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }

    private void CheckFile1Button()
    {
        if (!File.Exists(SaveSystem.filePath1))
        {
            loadFile1Button.interactable = false;
            deleteFile1Button.interactable = false;
        }
        else
        {
            loadFile1Button.interactable = true;
            deleteFile1Button.interactable = true;
        }
    }

    private void CheckFile2Button()
    {
        if (!File.Exists(SaveSystem.filePath2))
        {
            loadFile2Button.interactable = false;
            deleteFile2Button.interactable = false;
        }
        else
        {
            loadFile2Button.interactable = true;
            deleteFile2Button.interactable = true;
        }
    }

    private void CheckFile3Button()
    {
        if (!File.Exists(SaveSystem.filePath3))
        {
            loadFile3Button.interactable = false;
            deleteFile3Button.interactable = false;
        }
        else
        {
            loadFile3Button.interactable = true;
            deleteFile3Button.interactable = true;
        }
    }

    public void SetSaveFileId1()
    {
        saveDataManager.fileId = 0;
    }

    public void SetSaveFileId2()
    {
        saveDataManager.fileId = 1;
    }

    public void SetSaveFileId3()
    {
        saveDataManager.fileId = 2;
    }

    public void SetNewGame()
    {
        saveDataManager.newGame = true;
    }

    public void ResetNewGame()
    {
        saveDataManager.newGame = false;
    }

    public void ShowFile1Deletion()
    {
        confirmFile1Deletion.SetActive(true);
    }

    public void CloseFile1Deletion()
    {
        confirmFile1Deletion.SetActive(false);
    }

    public void ShowFile2Deletion()
    {
        confirmFile2Deletion.SetActive(true);
    }

    public void CloseFile2Deletion()
    {
        confirmFile2Deletion.SetActive(false);
    }
    public void ShowFile3Deletion()
    {
        confirmFile3Deletion.SetActive(true);
    }

    public void CloseFile3Deletion()
    {
        confirmFile3Deletion.SetActive(false);
    }

    public void DeleteFile1()
    {
        if (File.Exists(SaveSystem.filePath1))
        {
            File.Delete(SaveSystem.filePath1);
            Debug.Log("Deleted file at " + SaveSystem.filePath1);
        }
        CheckFile1Button();
        CloseFile1Deletion();
    }

    public void DeleteFile2()
    {
        if (File.Exists(SaveSystem.filePath2))
        {
            File.Delete(SaveSystem.filePath2);
            Debug.Log("Deleted file at " + SaveSystem.filePath2);
        }
        CheckFile2Button();
        CloseFile2Deletion();
    }

    public void DeleteFile3()
    {
        if (File.Exists(SaveSystem.filePath3))
        {
            File.Delete(SaveSystem.filePath3);
            Debug.Log("Deleted file at " + SaveSystem.filePath3);
        }
        CheckFile3Button();
        CloseFile3Deletion();
    }
}
