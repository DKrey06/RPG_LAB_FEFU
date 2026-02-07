using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentSlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Компоненты")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private Text slotNameText;
    [SerializeField] private Image background;

    [Header("Цвета")]
    [SerializeField] private Color normalColor = new Color(0.3f, 0.3f, 0.6f, 0.8f);
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color emptyColor = new Color(0.3f, 0.3f, 0.6f, 0.5f);

    private InventoryUI inventoryUI;
    private bool isSelected = false;

    public void Initialize(InventoryUI ui)
    {
        inventoryUI = ui;

        if (slotNameText != null)
        {
            slotNameText.text = "Броня";
        }

        ClearSlot();
    }

    public void UpdateSlot(ArmorItem armor)
    {
        if (armor == null)
        {
            ClearSlot();
            return;
        }

        if (armor.Icon != null)
        {
            itemIcon.sprite = armor.Icon;
            itemIcon.color = Color.white;
            itemIcon.gameObject.SetActive(true);
            background.color = normalColor;
        }
        else
        {
            ClearSlot();
        }
    }

    void ClearSlot()
    {
        itemIcon.gameObject.SetActive(false);
        SetSelected(false);
        background.color = emptyColor;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        background.color = selected ? selectedColor : (itemIcon.gameObject.activeSelf ? normalColor : emptyColor);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        inventoryUI.OnArmorSlotClicked();
    }
}