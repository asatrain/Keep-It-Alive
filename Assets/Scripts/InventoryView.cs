using System;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private ItemSlotView[] itemSlots;

    public Inventory Inventory { get; private set; }

    private void Awake()
    {
        Inventory = new Inventory(itemSlots.Length);
        Inventory.OnInventoryChanged += RefreshInventoryView;
        RefreshInventoryView();
    }

    private void RefreshInventoryView(object sender, EventArgs e)
    {
        RefreshInventoryView();
    }

    private void RefreshInventoryView()
    {
        for (var i = 0; i < Inventory.Slots.Length; i++)
            if (Inventory.Slots[i] == null)
            {
                var itemIconColor = itemSlots[i].itemIcon.color;
                itemIconColor.a = 0;
                itemSlots[i].itemIcon.color = itemIconColor;
                var borderColor = itemSlots[i].border.color;
                borderColor.a = 0;
                itemSlots[i].border.color = borderColor;
                itemSlots[i].itemCountText.text = string.Empty;
            }
            else
            {
                var itemIconColor = itemSlots[i].itemIcon.color;
                itemIconColor.a = 1;
                itemSlots[i].itemIcon.color = itemIconColor;
                var borderColor = itemSlots[i].border.color;
                borderColor.a = Inventory.ItemSelected && i == Inventory.SelectedItemInd ? 1 : 0;
                itemSlots[i].border.color = borderColor;
                itemSlots[i].itemIcon.sprite = Inventory.Slots[i].ItemInfo.icon;
                itemSlots[i].itemCountText.text = Inventory.Slots[i].ItemInfo.stackable
                    ? Inventory.Slots[i].Count.ToString()
                    : string.Empty;
            }
    }
}