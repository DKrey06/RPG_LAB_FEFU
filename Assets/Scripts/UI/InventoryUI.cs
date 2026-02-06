using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Основные компоненты")]
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private bool isInventoryOpen = false;

    [Header("Сетка инвентаря")]
    [SerializeField] private Transform inventoryGridParent;
    [SerializeField] private GameObject slotPrefab;
    private InventorySlotUI[,] slotUIs;

    [Header("Слот брони")]
    [SerializeField] private EquipmentSlotUI armorSlotUI;
    [SerializeField] private Text armorLevelText;

    [Header("Информация о предмете")]
    [SerializeField] private GameObject itemInfoPanel;
    [SerializeField] private Text itemNameText;
    [SerializeField] private Text itemDescriptionText;
    [SerializeField] private Text itemQuantityText;

    [Header("Кнопки действий")]
    [SerializeField] private Button useButton;
    [SerializeField] private Button dropButton;

    private InventorySlot selectedSlot;
    private int selectedSlotX = -1;
    private int selectedSlotY = -1;

    void Start()
    {
        if (inventorySystem == null)
        {
            inventorySystem = GameObject.FindGameObjectWithTag("Player").GetComponent<InventorySystem>();
        }

        InitializeUI();

        // Подписываемся на события
        inventorySystem.OnInventoryChanged += UpdateUI;
        inventorySystem.OnArmorChanged += UpdateArmorUI;

        // Инициализируем UI
        UpdateArmorUI(inventorySystem.GetEquippedArmor());

        inventoryPanel.SetActive(false);

        if (useButton != null) useButton.onClick.AddListener(OnUseButtonClicked);
        if (dropButton != null) dropButton.onClick.AddListener(OnDropButtonClicked);
    }

    void InitializeUI()
    {
        int width = inventorySystem.GetInventoryWidth();
        int height = inventorySystem.GetInventoryHeight();

        slotUIs = new InventorySlotUI[width, height];

        foreach (Transform child in inventoryGridParent)
        {
            Destroy(child.gameObject);
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject slotObj = Instantiate(slotPrefab, inventoryGridParent);
                InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();
                slotUI.Initialize(x, y, this);
                slotUIs[x, y] = slotUI;
            }
        }

        if (armorSlotUI != null)
        {
            armorSlotUI.Initialize(this);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);

        if (isInventoryOpen)
        {
            UpdateUI();
            Cursor.visible = true;
            Time.timeScale = 0.3f;
        }
        else
        {
            Cursor.visible = false;
            Time.timeScale = 1f;
            HideItemInfo();
        }
    }

    void UpdateUI()
    {
        if (!isInventoryOpen) return;

        int width = inventorySystem.GetInventoryWidth();
        int height = inventorySystem.GetInventoryHeight();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                InventorySlot slot = inventorySystem.GetSlot(x, y);
                slotUIs[x, y].UpdateSlot(slot);
            }
        }

        // Обновляем также слот брони при обновлении UI
        UpdateArmorUI(inventorySystem.GetEquippedArmor());
    }

    // Изменяем метод - теперь он принимает параметр ArmorItem
    void UpdateArmorUI(ArmorItem armor)
    {
        if (armorSlotUI != null)
        {
            armorSlotUI.UpdateSlot(armor);
        }

        if (armorLevelText != null)
        {
            if (armor != null)
            {
                armorLevelText.text = $"Уровень: {armor.ArmorLevel}";
            }
            else
            {
                armorLevelText.text = "Уровень: 1";
            }
        }
    }

    public void OnSlotClicked(int x, int y)
    {
        DeselectAllSlots();

        InventorySlot slot = inventorySystem.GetSlot(x, y);

        if (slot.IsEmpty)
        {
            HideItemInfo();
            return;
        }

        selectedSlot = slot;
        selectedSlotX = x;
        selectedSlotY = y;

        slotUIs[x, y].SetSelected(true);
        ShowItemInfo(slot.Item, slot.Quantity);
    }

    public void OnArmorSlotClicked()
    {
        DeselectAllSlots();

        var armor = inventorySystem.GetEquippedArmor();
        if (armor != null)
        {
            armorSlotUI.SetSelected(true);
            ShowItemInfo(armor, 1);
        }
        else
        {
            HideItemInfo();
        }
    }

    void DeselectAllSlots()
    {
        int width = inventorySystem.GetInventoryWidth();
        int height = inventorySystem.GetInventoryHeight();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                slotUIs[x, y].SetSelected(false);
            }
        }

        if (armorSlotUI != null)
        {
            armorSlotUI.SetSelected(false);
        }
    }

    void ShowItemInfo(Item item, int quantity)
    {
        if (itemInfoPanel == null) return;

        itemInfoPanel.SetActive(true);
        itemNameText.text = item.Name;
        itemDescriptionText.text = item.Description;
        itemQuantityText.text = $"Количество: {quantity}";

        bool isUsable = (item is Cheese) || (item is StrengthPotion) || (item is ArmorItem);
        if (useButton != null) useButton.interactable = isUsable;
        if (dropButton != null) dropButton.interactable = true;
    }

    void HideItemInfo()
    {
        if (itemInfoPanel != null)
        {
            itemInfoPanel.SetActive(false);
        }

        if (useButton != null) useButton.interactable = false;
        if (dropButton != null) dropButton.interactable = false;
    }

    public void OnUseButtonClicked()
    {
        if (selectedSlotX >= 0 && selectedSlotY >= 0)
        {
            inventorySystem.UseItem(selectedSlotX, selectedSlotY);
            HideItemInfo();
        }
    }

    public void OnDropButtonClicked()
    {
        if (selectedSlotX >= 0 && selectedSlotY >= 0)
        {
            inventorySystem.RemoveItem(selectedSlotX, selectedSlotY);
            HideItemInfo();
        }
    }

    void OnDestroy()
    {
        if (inventorySystem != null)
        {
            inventorySystem.OnInventoryChanged -= UpdateUI;
            inventorySystem.OnArmorChanged -= UpdateArmorUI;
        }
    }
}