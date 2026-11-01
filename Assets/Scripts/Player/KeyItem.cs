using System;
using UnityEngine;

[Serializable]
public class KeyItem
{
    public string name;
    public string description;

    public KeyItem(string itemName, string itemDescription)
    {
        name = itemName;
        description = itemDescription;
    }
}
