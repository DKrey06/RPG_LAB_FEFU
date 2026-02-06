[System.Serializable]
public class EquipmentSlot
{
    public ArmorItem EquippedArmor { get; private set; }
    public bool IsEmpty => EquippedArmor == null;

    public void Equip(ArmorItem armor)
    {
        EquippedArmor = armor;
    }

    public ArmorItem Unequip()
    {
        var armor = EquippedArmor;
        EquippedArmor = null;
        return armor;
    }
}