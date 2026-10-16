using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControlRebind : MonoBehaviour
{
    // Input action elements
    [SerializeField] private InputActionAsset inputActions;     // The input action asset
    [SerializeField] private string actionMapName = "Player";   // The name of the action map
    [SerializeField] private string actionName = "Interact";    // The name of the input action
    [SerializeField] private int bindingId = 0;                 // The Id of the action binding

    // UI elements
    [SerializeField] private Button rebindButton;
    [SerializeField] private TextMeshProUGUI bindingDisplayText;

    private InputAction targetAction;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private const string SaveKey = "InputBindingOverrides";

    private void Awake()
    {
        targetAction = inputActions.FindActionMap(actionMapName).FindAction(actionName);

        LoadBindings();
        UpdateUI();
    }

    public void StartRebinding()
    {
        targetAction.Disable();

        rebindButton.interactable = false;
        bindingDisplayText.text = "Press new button";

        rebindingOperation = targetAction.PerformInteractiveRebinding(bindingId)
            .WithControlsExcluding("<Mouse>/position")
            .WithControlsExcluding("<Mouse>/delta")
            .WithTimeout(10f)
            .OnComplete(operation => FinishRebinding())
            //.OnCancel(operation => FinishRebinding())
            .Start();
    }

    private void FinishRebinding()
    {
        rebindingOperation.Dispose();

        targetAction.Enable();
        rebindButton.interactable = true;

        UpdateUI();
        SaveBindings();
    }

    private void UpdateUI()
    {
        if (targetAction != null)
        {
            bindingDisplayText.text = targetAction.GetBindingDisplayString();
        }
    }

    private void SaveBindings()
    {
        string overridesJson = inputActions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(SaveKey, overridesJson);
        PlayerPrefs.Save();
    }

    private void LoadBindings()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string overridesJson = PlayerPrefs.GetString(SaveKey);
            inputActions.LoadBindingOverridesFromJson(overridesJson);
        }
    }
}
