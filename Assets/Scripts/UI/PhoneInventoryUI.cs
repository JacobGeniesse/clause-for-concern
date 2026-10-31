using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Fills HeldPhone inventory. Row look and empty labels live on the prefab. this script clones and labels them.
public class PhoneInventoryUI : MonoBehaviour
{
    [SerializeField] GameObject standardRowPrefab; // inactive templates under InventoryPage/Templates
    [SerializeField] GameObject keyRowPrefab;
    [SerializeField] GameObject standardEmpty;
    [SerializeField] GameObject keyEmpty;
    [SerializeField] TextMeshProUGUI healthLabel; // HP / ammo status at the top of the tab
    [SerializeField] TextMeshProUGUI ammoLabel;

    readonly List<GameObject> standardRows = new List<GameObject>();
    readonly List<GameObject> keyRows = new List<GameObject>();
    PlayerInventory inventory;
    PlayerHealth health;
    string lastHealthText; // skip rebuilds when the number has not changed
    string lastAmmoText;

    void OnEnable()
    {
        BindInventory();
        Rebuild();
        RefreshStatus();
    }

    void OnDisable()
    {
        if (inventory != null)
        {
            inventory.InventoryChanged -= Rebuild;
        }
    }

    void BindInventory()
    {
        if (inventory != null)
        {
            inventory.InventoryChanged -= Rebuild;
        }

        inventory = PlayerInventory.Instance;
        health = inventory != null ? inventory.GetComponent<PlayerHealth>() : null;

        if (inventory != null)
        {
            inventory.InventoryChanged += Rebuild;
        }
    }

    void LateUpdate()
    {
        // Damage and stapler fire do not raise InventoryChanged while open yet.
        RefreshStatus();
    }

    void RefreshStatus()
    {
        string hp = health != null
            ? Mathf.RoundToInt(health.currentHealth) + " / " + Mathf.RoundToInt(health.maxHealth)
            : "--";
        SetStatus(healthLabel, "HP  " + hp, ref lastHealthText);

        string ammo = inventory != null
            ? inventory.ammoCount + " / " + inventory.maxAmmo
            : "--";
        SetStatus(ammoLabel, "AMMO  " + ammo, ref lastAmmoText);
    }

    static void SetStatus(TextMeshProUGUI label, string text, ref string last)
    {
        if (label == null || text == last)
        {
            return;
        }

        last = text;
        label.text = text;
    }

    // item rows from the current lists.
    void Rebuild()
    {
        ClearSpawnedRows();

        bool hasStandard = inventory != null && inventory.StandardItems.Count > 0;
        bool hasKeys = inventory != null && inventory.KeyItems.Count > 0;

        if (standardEmpty != null)
        {
            standardEmpty.SetActive(!hasStandard);
        }

        if (keyEmpty != null)
        {
            keyEmpty.SetActive(!hasKeys);
        }

        if (hasStandard)
        {
            for (int i = 0; i < inventory.StandardItems.Count; i++)
            {
                CreateStandardRow(inventory.StandardItems[i]);
            }
        }

        if (hasKeys)
        {
            for (int i = 0; i < inventory.KeyItems.Count; i++)
            {
                CreateKeyRow(inventory.KeyItems[i]);
            }
        }

        RectTransform root = standardEmpty != null ? standardEmpty.transform.parent as RectTransform : null;
        if (root != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(root);
        }
    }

    void CreateStandardRow(ItemPickup item)
    {
        if (item == null || standardRowPrefab == null || standardEmpty == null)
        {
            return;
        }

        GameObject row = SpawnRow(standardRowPrefab, standardEmpty.transform);
        standardRows.Add(row);

        string itemName = !string.IsNullOrEmpty(item.description) ? item.description : item.itemType.ToString();
        SetLabel(row, itemName + "  x" + item.amount + "   USE");

        Button button = row.GetComponent<Button>();
        if (button == null)
        {
            return;
        }

        ItemPickup captured = item;
        button.onClick.AddListener(() =>
        {
            if (inventory != null)
            {
                inventory.UseItems(captured); // standard items only; key rows have no button
            }
        });
    }

    // Key items are display-only.
    void CreateKeyRow(KeyItemPickup key)
    {
        if (key == null || keyRowPrefab == null || keyEmpty == null)
        {
            return;
        }

        GameObject row = SpawnRow(keyRowPrefab, keyEmpty.transform);
        keyRows.Add(row);

        string name = !string.IsNullOrEmpty(key.keyName) ? key.keyName : "Key item";
        string text = string.IsNullOrEmpty(key.description) ? name : name + "  —  " + key.description;
        SetLabel(row, text);
    }

    static GameObject SpawnRow(GameObject prefab, Transform insertBefore)
    {
        GameObject row = Instantiate(prefab, insertBefore.parent);
        row.SetActive(true);

        // Insert above the Empty label so the order stays Header / rows / Empty.
        row.transform.SetSiblingIndex(insertBefore.GetSiblingIndex());
        return row;
    }

    void ClearSpawnedRows()
    {
        DestroyRows(standardRows);
        DestroyRows(keyRows);
    }

    static void DestroyRows(List<GameObject> rows)
    {
        for (int i = 0; i < rows.Count; i++)
        {
            if (rows[i] != null)
            {
                rows[i].SetActive(false);
                Destroy(rows[i]);
            }
        }

        rows.Clear();
    }

    static void SetLabel(GameObject row, string text)
    {
        TextMeshProUGUI label = row.GetComponentInChildren<TextMeshProUGUI>(true);
        if (label != null)
        {
            label.text = text;
        }
    }
}
