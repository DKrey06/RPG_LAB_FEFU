public abstract class Equipment : Item
{
    public int RequiredLevel { get; protected set; }
    public bool IsEquipped { get; private set; }

    protected Equipment(
        string id,
        string name,
        string description,
        int price,
        int requiredLevel
    ) : base(id, name, description, price)
    {
        RequiredLevel = requiredLevel;
    }

    public virtual void Equip()
    {
        IsEquipped = true;
    }

    public virtual void Unequip()
    {
        IsEquipped = false;
    }
}
