using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsMenuPanel;
    [SerializeField] private GameObject startMenuPanel;
    [SerializeField] private GameObject controllerMenuPanel;
    [SerializeField] private GameObject volumeMenuPanel;
    [SerializeField] private GameObject keyboardBindings;
    [SerializeField] private GameObject controllerBindings;

    private void Start()
    {
        // Make it so the main menu panel is the one that's active at the start
        mainMenuPanel.SetActive(true);
        settingsMenuPanel.SetActive(false);
        startMenuPanel.SetActive(false);
        controllerMenuPanel.SetActive(false);
        volumeMenuPanel.SetActive(false);

        // Have the keyboard bindings be active first
        keyboardBindings.SetActive(true);
        controllerBindings.SetActive(false);
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
        controllerBindings.SetActive(false);
    }

    public void SwitchToControllerBindings()
    {
        controllerBindings.SetActive(true);
        keyboardBindings.SetActive(false);
    }
}
