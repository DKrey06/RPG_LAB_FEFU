using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Компоненты")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private Text quantityText;
    [SerializeField] private Image background;

    [Header("Цвета")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color emptyColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);

    private int slotX;
    private int slotY;
    private InventoryUI inventoryUI;
    private bool isSelected = false;

    public void Initialize(int x, int y, InventoryUI ui)
    {
        slotX = x;
        slotY = y;
        inventoryUI = ui;

        ClearSlot();
    }

    public void UpdateSlot(InventorySlot slot)
    {
        if (slot == null || slot.IsEmpty)
        {
            ClearSlot();
            return;
        }

        if (slot.Item.Icon != null)
        {
            itemIcon.sprite = slot.Item.Icon;
            itemIcon.color = Color.white;
            itemIcon.gameObject.SetActive(true);
        }
        else
        {
            itemIcon.gameObject.SetActive(false);
        }

        if (slot.Quantity > 1 && slot.Item.IsStackable)
        {
            quantityText.text = slot.Quantity.ToString();
            quantityText.gameObject.SetActive(true);
        }
        else
        {
            quantityText.gameObject.SetActive(false);
        }
    }

    void ClearSlot()
    {
        itemIcon.gameObject.SetActive(false);
        quantityText.gameObject.SetActive(false);
        SetSelected(false);
        background.color = emptyColor;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        background.color = selected ? selectedColor : emptyColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        inventoryUI.OnSlotClicked(slotX, slotY);
    }
}