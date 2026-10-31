using UnityEngine;
using UnityEngine.UI;

public class CameraSettings : MonoBehaviour
{
    // Camera settings
    [SerializeField] private float cameraSensitivity;
    [SerializeField] private float cameraFOV;

    [SerializeField] private Slider sensSlider;
    [SerializeField] private Slider FOVSlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("SensitivityValue"))
        {
            LoadSenseSettings();
        }

        if (PlayerPrefs.HasKey("FOVValue"))
        {
            LoadFOVSettings();
        }
    }

    public void SetCameraSensitivity(float value)
    {
        cameraSensitivity = value;
        PlayerPrefs.SetFloat("SensitivityValue", value);
    }

    public void SetFOV(float value)
    {
        cameraFOV = value;
        PlayerPrefs.SetFloat("FOVValue", value);
    }

    private void LoadSenseSettings()
    {
        sensSlider.value = PlayerPrefs.GetFloat("SensitivityValue");
        SetCameraSensitivity(sensSlider.value);
    }

    private void LoadFOVSettings()
    {
        FOVSlider.value = PlayerPrefs.GetFloat("FOVValue");
        SetFOV(FOVSlider.value);
    }
}
