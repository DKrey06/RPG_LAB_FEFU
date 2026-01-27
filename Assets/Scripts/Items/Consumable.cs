public abstract class Consumable : Item
{
    public bool IsConsumed { get; private set; }

    protected Consumable(
        string id,
        string name,
        string description,
        int price
    ) : base(id, name, description, price)
    {
    }

    public void Consume(PlayerStats player)
    {
        if (IsConsumed || player == null) return;

        ApplyEffect(player);
        IsConsumed = true;
    }

    protected abstract void ApplyEffect(PlayerStats player);
}