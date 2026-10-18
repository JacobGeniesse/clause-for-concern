using UnityEngine;
using UnityEngine.UI;

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

    void OnEnable()
    {
        ShowMap();
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
