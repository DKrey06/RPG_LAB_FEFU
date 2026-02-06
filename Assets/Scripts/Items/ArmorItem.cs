using UnityEngine;

public class ArmorItem : Item
{
    public int ArmorLevel { get; private set; }

    public ArmorItem(int armorLevel, Sprite icon = null)
        : base($"armor_{armorLevel}", $"Ѕрон€ {armorLevel}", $"”ровень брони: {armorLevel}", 100 * armorLevel, icon, false)
    {
        ArmorLevel = Mathf.Clamp(armorLevel, 1, 5);
    }

    public void Equip(PlayerStats player)
    {
        if (player != null)
        {
            player.ArmorLevel = ArmorLevel;
            Debug.Log($"Ёкипирована брон€ уровн€ {ArmorLevel}");
        }
    }
}