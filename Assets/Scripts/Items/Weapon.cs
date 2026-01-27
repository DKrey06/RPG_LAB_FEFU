public enum WeaponType
{
    Sword,
    Axe,
    Bow,
    Staff,
    Dagger
}

public class Weapon : Equipment
{
    public WeaponType Type { get; private set; }

    public int Damage { get; private set; }
    public float AttackSpeed { get; private set; }
    public float Range { get; private set; }

    public Weapon(
        string id,
        string name,
        string description,
        int price,
        int requiredLevel,
        WeaponType type,
        int damage,
        float attackSpeed,
        float range
    ) : base(id, name, description, price, requiredLevel)
    {
        Type = type;
        Damage = damage;
        AttackSpeed = attackSpeed;
        Range = range;
    }
}