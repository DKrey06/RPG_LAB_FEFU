using UnityEngine;

public class StrengthPotion : Item
{
    public int DamageBonus { get; private set; }
    public float Duration { get; private set; }

    public StrengthPotion(Sprite icon = null)
        : base("strength_potion", "Зелье силы", "Увеличивает урон на 1 на 30 секунд", 50, icon, true)
    {
        DamageBonus = 1;
        Duration = 30f;
    }

    public void Use(PlayerCombat playerCombat)
    {
        if (playerCombat != null)
        {
            playerCombat.AddDamageBuff(Duration, DamageBonus);
            Debug.Log($"Использовано зелье силы. +{DamageBonus} урона на {Duration} секунд");
        }
    }
}