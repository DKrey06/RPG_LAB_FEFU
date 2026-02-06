using System;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [Header("Настройки инвентаря")]
    [SerializeField] private int inventoryWidth = 6;
    [SerializeField] private int inventoryHeight = 4;

    [Header("Начальные предметы")]
    [SerializeField] private bool addStartingItems = true;

    [Header("Ссылки на спрайты")]
    [SerializeField] private Sprite cheeseSprite;
    [SerializeField] private Sprite strengthPotionSprite;
    [SerializeField] private Sprite[] armorSprites;

    private InventorySlot[,] inventoryGrid;
    private ArmorItem equippedArmor;

    private PlayerStats playerStats;
    private PlayerCombat playerCombat;

    public event Action OnInventoryChanged;
    public event Action<ArmorItem> OnArmorChanged;

    void Awake()
    {
        InitializeInventory();

        playerStats = GetComponent<PlayerStats>();
        playerCombat = GetComponent<PlayerCombat>();

        if (playerStats == null)
        {
            playerStats = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStats>();
        }
    }

    void Start()
    {
        if (addStartingItems)
        {
            AddStartingItems();
        }
    }

    void InitializeInventory()
    {
        inventoryGrid = new InventorySlot[inventoryWidth, inventoryHeight];

        for (int x = 0; x < inventoryWidth; x++)
        {
            for (int y = 0; y < inventoryHeight; y++)
            {
                inventoryGrid[x, y] = new InventorySlot();
            }
        }

        // Начальная броня 1 уровня
        Sprite armorSprite = null;
        if (armorSprites != null && armorSprites.Length > 0)
        {
            armorSprite = armorSprites[0];
        }

        equippedArmor = new ArmorItem(1, armorSprite);

        // Экипируем начальную броню
        if (playerStats != null)
        {
            equippedArmor.Equip(playerStats);
        }
    }

    void AddStartingItems()
    {
        // Добавляем 2 сыра
        Cheese cheese = new Cheese(cheeseSprite);
        AddItem(cheese, 2);

        // Добавляем 1 зелье силы
        StrengthPotion potion = new StrengthPotion(strengthPotionSprite);
        AddItem(potion, 1);

        Debug.Log("Начальные предметы добавлены: 2 сыра, 1 зелье силы");
    }

    public bool AddItem(Item item, int quantity = 1)
    {
        if (item == null) return false;

        if (item.IsStackable)
        {
            if (AddToExistingStack(item, quantity))
            {
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        for (int y = 0; y < inventoryHeight; y++)
        {
            for (int x = 0; x < inventoryWidth; x++)
            {
                if (inventoryGrid[x, y].IsEmpty)
                {
                    inventoryGrid[x, y].SetItem(item, quantity);
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }
        }

        Debug.LogWarning("Инвентарь полон!");
        return false;
    }

    private bool AddToExistingStack(Item item, int quantity)
    {
        for (int y = 0; y < inventoryHeight; y++)
        {
            for (int x = 0; x < inventoryWidth; x++)
            {
                var slot = inventoryGrid[x, y];
                if (!slot.IsEmpty && slot.Item.Id == item.Id)
                {
                    slot.Quantity += quantity;
                    return true;
                }
            }
        }
        return false;
    }

    public bool UseItem(int x, int y)
    {
        if (!IsValidSlot(x, y)) return false;

        var slot = inventoryGrid[x, y];
        if (slot.IsEmpty) return false;

        bool used = false;

        if (slot.Item is Cheese cheese)
        {
            cheese.Use(playerStats);
            used = true;
        }
        else if (slot.Item is StrengthPotion potion)
        {
            potion.Use(playerCombat);
            used = true;
        }
        else if (slot.Item is ArmorItem armor)
        {
            EquipArmor(armor);
            used = true;
        }

        if (used)
        {
            if (slot.Quantity > 1)
            {
                slot.Quantity--;
            }
            else
            {
                slot.Clear();
            }

            OnInventoryChanged?.Invoke();
            return true;
        }

        return false;
    }

    private void EquipArmor(ArmorItem armor)
    {
        // Старую броню перемещаем в инвентарь
        if (equippedArmor != null)
        {
            AddItem(equippedArmor);
        }

        // Надеваем новую броню
        equippedArmor = armor;

        // Применяем эффект брони к игроку
        if (playerStats != null)
        {
            armor.Equip(playerStats);
        }

        // Вызываем событие с передачей новой брони
        OnArmorChanged?.Invoke(armor);

        Debug.Log($"Экипирована броня уровня {armor.ArmorLevel}");
    }

    public bool RemoveItem(int x, int y, int quantity = 1)
    {
        if (!IsValidSlot(x, y)) return false;

        var slot = inventoryGrid[x, y];
        if (slot.IsEmpty) return false;

        if (slot.Quantity > quantity)
        {
            slot.Quantity -= quantity;
        }
        else
        {
            slot.Clear();
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    private bool IsValidSlot(int x, int y)
    {
        return x >= 0 && x < inventoryWidth && y >= 0 && y < inventoryHeight;
    }

    public InventorySlot GetSlot(int x, int y) => inventoryGrid[x, y];
    public ArmorItem GetEquippedArmor() => equippedArmor;
    public int GetInventoryWidth() => inventoryWidth;
    public int GetInventoryHeight() => inventoryHeight;

    // Тестовый метод для добавления брони
    public void AddTestArmor(int level)
    {
        Sprite sprite = null;
        if (armorSprites != null && level >= 1 && level <= armorSprites.Length)
        {
            sprite = armorSprites[level - 1];
        }

        ArmorItem armor = new ArmorItem(level, sprite);
        AddItem(armor);
    }
}