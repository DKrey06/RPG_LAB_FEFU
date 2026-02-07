using UnityEngine;

public abstract class Item
{
    public string Id { get; protected set; }
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public Sprite Icon { get; protected set; }
    public int Price { get; protected set; }
    public bool IsStackable { get; protected set; }

    protected Item(string id, string name, string description, int price, Sprite icon = null, bool isStackable = true)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        Icon = icon;
        IsStackable = isStackable;
    }

    public void SetIcon(Sprite sprite)
    {
        Icon = sprite;
    }
}