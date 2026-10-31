using System.Collections.Generic;
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
    [SerializeField] Image mapTab;
    [SerializeField] Image tasksTab;
    [SerializeField] Image inventoryTab;

    static readonly Color ActiveTab = new Color(0.34f, 0.42f, 0.52f, 1f);
    static readonly Color IdleTab = new Color(0.16f, 0.18f, 0.22f, 1f);

    GraphicRaycaster raycaster;
    readonly List<RaycastResult> hits = new List<RaycastResult>();
    InputAction previousTab;
    InputAction nextTab;
    int tabIndex;

    void Awake()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            raycaster = canvas.GetComponent<GraphicRaycaster>();
        }

        BindTabActions();
    }

    void OnEnable()
    {
        BindTabActions();

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

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryClickTab();
        }
    }

    void BindTabActions()
    {
        if (previousTab != null && nextTab != null)
        {
            return;
        }

        if (masterList == null)
        {
            PlayerMovement movement = FindFirstObjectByType<PlayerMovement>();
            if (movement != null)
            {
                masterList = movement.masterList;
            }
        }

        if (masterList == null)
        {
            return;
        }

        previousTab = masterList.FindAction("Previous");
        nextTab = masterList.FindAction("Next");
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

    // World-space UI clicks on the MAP / TASKS / INV buttons.
    void TryClickTab()
    {
        if (raycaster == null || EventSystem.current == null)
        {
            return;
        }

        PointerEventData pointer = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };
        hits.Clear();
        raycaster.Raycast(pointer, hits);
        for (int i = 0; i < hits.Count; i++)
        {
            Button button = hits[i].gameObject.GetComponentInParent<Button>();
            if (button != null && button.transform.IsChildOf(transform))
            {
                button.onClick.Invoke();
                return;
            }
        }
    }

    void Show(GameObject page, Image tab)
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

        SetTab(mapTab, tab == mapTab);
        SetTab(tasksTab, tab == tasksTab);
        SetTab(inventoryTab, tab == inventoryTab);
    }

    static void SetTab(Image tab, bool active)
    {
        if (tab != null)
        {
            tab.color = active ? ActiveTab : IdleTab;
        }
    }
}
