public enum ArmorType
{
    Helmet,
    Chest,
    Gloves,
    Boots,
    Shield
}

public class Armor : Equipment
{
    public ArmorType Type { get; private set; }

    public int Defense { get; private set; }
    public int BonusHP { get; private set; }

    public Armor(
        string id,
        string name,
        string description,
        int price,
        int requiredLevel,
        ArmorType type,
        int defense,
        int bonusHP
    ) : base(id, name, description, price, requiredLevel)
    {
        Type = type;
        Defense = defense;
        BonusHP = bonusHP;
    }
}