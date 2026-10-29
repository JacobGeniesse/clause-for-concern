using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private GameObject controlSettings;
    [SerializeField] private GameObject volumeSettings;

    [SerializeField] private GameObject quitPanel;

    [SerializeField] private GameObject keyboardBindings;
    [SerializeField] private GameObject controllerBindings;

    private void Start()
    {
        ResetPauseMenu();
    }

    public void ResetPauseMenu()
    {
        // Make the pause menu canvas active first
        pauseCanvas.SetActive(true);
        settingsCanvas.SetActive(false);
        controlSettings.SetActive(false);
        volumeSettings.SetActive(false);

        quitPanel.SetActive(false);

        // Make the keyboard bindings active first (can probably make this so whichever is first depends on what control option the player is using)
        keyboardBindings.SetActive(true);
        controllerBindings.SetActive(false);
    }

    public void SwitchToSettings()
    {
        pauseCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }

    public void SwitchFromSettings()
    {
        settingsCanvas.SetActive(false);
        pauseCanvas.SetActive(true);
    }

    public void SwitchToControllerSettings()
    {
        settingsCanvas.SetActive(false);
        controlSettings.SetActive(true);
    }

    public void SwitchFromControllerSettings()
    {
        controlSettings.SetActive(false);
        settingsCanvas.SetActive(true);
    }

    public void SwitchToVolumeSettings()
    {
        settingsCanvas.SetActive(false);
        volumeSettings.SetActive(true);
    }

    public void SwitchFromVolumeSettings()
    {
        volumeSettings.SetActive(false);
        settingsCanvas.SetActive(true);
    }

    public void SwitchToControllerBindings()
    {
        keyboardBindings.SetActive(false);
        controllerBindings.SetActive(true);
    }

    public void SwitchToKeyBoardBindings()
    {
        controllerBindings.SetActive(false);
        keyboardBindings.SetActive(true);
    }

    public void ShowQuitPanel()
    {
        quitPanel.SetActive(true);
    }

    public void HideQuitPanel()
    {
        quitPanel.SetActive(false);
    }
}
