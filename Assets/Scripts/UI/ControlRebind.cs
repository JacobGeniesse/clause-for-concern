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
    [SerializeField] private TextMeshProUGUI bindWarningText;

    private InputAction targetAction;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private const string SaveKey = "InputBindingOverrides";

    private void Awake()
    {
        targetAction = inputActions.FindActionMap(actionMapName).FindAction(actionName);

        LoadBindings();
        UpdateUI();
        bindWarningText.text = "";
    }

    public void StartRebinding()
    {
        targetAction.Disable();

        rebindButton.interactable = false;
        bindingDisplayText.text = "Press new button";

        // The initial binding of the target action, saved incase the new binding is already used for another action
        InputBinding originalBinding = targetAction.bindings[bindingId];

        rebindingOperation = targetAction.PerformInteractiveRebinding(bindingId)
            .WithControlsExcluding("<Mouse>/position")
            .WithControlsExcluding("<Mouse>/delta")
            .WithTimeout(10f)
            .OnComplete(operation => FinishRebinding(originalBinding))
            .OnCancel(operation => FinishRebinding())
            .Start();
    }

    private void FinishRebinding(InputBinding originaBinding)
    {
        rebindingOperation.Dispose();

        // Check if the new binding is currently in use with another action
        if (IsBindingDuplicate(targetAction, bindingId))
        {
            // Warn the player the selected binding is already in use, and revert to the previous binding
            bindWarningText.text = targetAction.GetBindingDisplayString(bindingId) + " already in use";
            targetAction.ChangeBinding(bindingId).To(originaBinding);
            UpdateUI();
        }
        else
        {
            UpdateUI();
            SaveBindings();
            bindWarningText.text = "";
        }

        targetAction.Enable();
        rebindButton.interactable = true;
    }

    private void FinishRebinding()
    {
        rebindingOperation.Dispose();

        targetAction.Enable();
        rebindButton.interactable = true;

        UpdateUI();
        SaveBindings();
    }

    private bool IsBindingDuplicate(InputAction actionRebind, int bindingId)
    {
        string newPath = actionRebind.bindings[bindingId].effectivePath;

        foreach (var action in actionRebind.actionMap.actions)
        {
            if (action == actionRebind) continue;

            foreach (var binding in action.bindings)
            {
                if (binding.effectivePath == newPath) return true;
            }
        }
        return false;
    }

    private void UpdateUI()
    {
        if (targetAction != null)
        {
            bindingDisplayText.text = targetAction.GetBindingDisplayString(bindingId);
        }
    }

    private void SaveBindings()
    {
        // Save the rebind to player prefs
        string overridesJson = inputActions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(SaveKey, overridesJson);
        PlayerPrefs.Save();
    }

    private void LoadBindings()
    {
        // Load rebind from player prefs
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string overridesJson = PlayerPrefs.GetString(SaveKey);
            inputActions.LoadBindingOverridesFromJson(overridesJson);
        }
    }
}
