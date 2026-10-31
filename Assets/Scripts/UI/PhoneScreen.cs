using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Switches the phone UI between map, tasks, and inventory pages.
public class PhoneScreen : MonoBehaviour
{
    [SerializeField] InputActionAsset masterList;
    [SerializeField] GameObject mapPage;
    [SerializeField] GameObject tasksPage;
    [SerializeField] GameObject inventoryPage;
    [SerializeField] Button mapTab;
    [SerializeField] Button tasksTab;
    [SerializeField] Button inventoryTab;

    InputAction previousTab;
    InputAction nextTab;
    int tabIndex;

    void Awake()
    {
        if (masterList != null)
        {
            previousTab = masterList.FindAction("Previous");
            nextTab = masterList.FindAction("Next");
        }
    }

    void OnEnable()
    {
        if (previousTab != null)
        {
            previousTab.Enable();
            previousTab.performed += OnPrevious;
        }

        if (nextTab != null)
        {
            nextTab.Enable();
            nextTab.performed += OnNext;
        }

        tabIndex = 0;
        ShowCurrent();
    }

    void OnDisable()
    {
        if (previousTab != null)
        {
            previousTab.performed -= OnPrevious;
            previousTab.Disable();
        }

        if (nextTab != null)
        {
            nextTab.performed -= OnNext;
            nextTab.Disable();
        }
    }

    void Update()
    {
        if (!Minimap.IsOpen)
        {
            return;
        }

        Button tab = CurrentTab();
        if (tab != null && EventSystem.current != null
            && EventSystem.current.currentSelectedGameObject != tab.gameObject)
        {
            EventSystem.current.SetSelectedGameObject(tab.gameObject);
        }
    }

    Button CurrentTab()
    {
        switch (tabIndex)
        {
            case 1:
                return tasksTab;
            case 2:
                return inventoryTab;
            default:
                return mapTab;
        }
    }

    void OnPrevious(InputAction.CallbackContext context)
    {
        Cycle(-1);
    }

    void OnNext(InputAction.CallbackContext context)
    {
        Cycle(1);
    }

    public void ShowMap()
    {
        tabIndex = 0;
        ShowCurrent();
    }

    public void ShowTasks()
    {
        tabIndex = 1;
        ShowCurrent();
    }

    public void ShowInventory()
    {
        tabIndex = 2;
        ShowCurrent();
    }

    // Previous/Next wrap Map -> Tasks -> Inventory.
    void Cycle(int direction)
    {
        if (!Minimap.IsOpen)
        {
            return;
        }

        tabIndex = (tabIndex + direction + 3) % 3;
        ShowCurrent();
    }

    void ShowCurrent()
    {
        switch (tabIndex)
        {
            case 1:
                Show(tasksPage, tasksTab);
                break;
            case 2:
                Show(inventoryPage, inventoryTab);
                break;
            default:
                Show(mapPage, mapTab);
                break;
        }
    }

    void Show(GameObject page, Button tab)
    {
        if (mapPage != null)
        {
            mapPage.SetActive(page == mapPage);
        }

        if (tasksPage != null)
        {
            tasksPage.SetActive(page == tasksPage);
        }

        if (inventoryPage != null)
        {
            inventoryPage.SetActive(page == inventoryPage);
        }

        if (tab != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(tab.gameObject);
        }
    }
}
