using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public Item Item { get; private set; }
    public int Quantity { get; set; }
    public bool IsEmpty => Item == null;

    public InventorySlot()
    {
        Item = null;
        Quantity = 0;
    }

    public void SetItem(Item item, int quantity)
    {
        Item = item;
        Quantity = quantity;
    }

    public void Clear()
    {
        Item = null;
        Quantity = 0;
    }
}