using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Switches the phone UI between map, tasks, and inventory pages
public class PhoneScreen : MonoBehaviour
{
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

    void Awake()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            raycaster = canvas.GetComponent<GraphicRaycaster>();
        }
    }

    void OnEnable()
    {
        ShowMap();
    }

    void Update()
    {
        if (!Minimap.IsOpen)
        {
            return;
        }

        // 1/2/3 also switch pages
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.digit1Key.wasPressedThisFrame || keyboard.numpad1Key.wasPressedThisFrame)
            {
                ShowMap();
            }
            else if (keyboard.digit2Key.wasPressedThisFrame || keyboard.numpad2Key.wasPressedThisFrame)
            {
                ShowTasks();
            }
            else if (keyboard.digit3Key.wasPressedThisFrame || keyboard.numpad3Key.wasPressedThisFrame)
            {
                ShowInventory();
            }
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryClickTab();
        }
    }

    public void ShowMap()
    {
        Show(mapPage, mapTab);
    }

    public void ShowTasks()
    {
        Show(tasksPage, tasksTab);
    }

    public void ShowInventory()
    {
        Show(inventoryPage, inventoryTab);
    }

    // clicks on the MAP / TASKS / INV buttons.
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
