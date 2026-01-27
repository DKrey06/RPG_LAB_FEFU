public class HealingConsumable : Consumable
{
    public int HealAmount { get; private set; }

    public HealingConsumable(
        string id,
        string name,
        string description,
        int price,
        int healAmount
    ) : base(id, name, description, price)
    {
        HealAmount = healAmount;
    }

    protected override void ApplyEffect(PlayerStats player)
    {
        player.Heal(HealAmount);
    }
}