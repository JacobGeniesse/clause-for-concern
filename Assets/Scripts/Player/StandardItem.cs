using UnityEngine;

public class StandardItem
{
    public ItemType type;
    public int amount;
    public string description;

    public StandardItem(ItemType itemType, int itemAmount, string itemDescription)
    {
        type = itemType;
        amount = itemAmount;
        description = itemDescription;
    }
}
