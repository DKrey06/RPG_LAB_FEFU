using UnityEngine;

public class Cheese : Item
{
    public int HealAmount { get; private set; }

    public Cheese(Sprite icon = null)
        : base("cheese", "Сыр", "Восстанавливает 1 HP", 10, icon, true)
    {
        HealAmount = 1;
    }

    public void Use(PlayerStats player)
    {
        if (player != null)
        {
            player.Heal(HealAmount);
            Debug.Log($"Использован сыр. Восстановлено {HealAmount} HP");
        }
    }
}