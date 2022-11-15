using System;

public class Inventory
{
    private int selectedItemInd;

    public event EventHandler OnInventoryChanged;

    public Inventory(int amount)
    {
        Slots = new ItemSlot[amount];
        ItemSelected = false;
    }

    public ItemSlot[] Slots { get; }

    public bool ItemSelected { get; private set; }

    public int SelectedItemInd
    {
        get => selectedItemInd;
        private set
        {
            selectedItemInd = value;
            if (selectedItemInd >= Slots.Length) selectedItemInd = 0;
            else if (selectedItemInd < 0) selectedItemInd = Slots.Length - 1;
        }
    }

    public bool AddItem(ItemInfo itemInfo)
    {
        foreach (var slot in Slots)
            if (slot != null && slot.ItemInfo.stackable && slot.ItemInfo.id == itemInfo.id &&
                slot.Count < slot.ItemInfo.maxCount)
            {
                ++slot.Count;
                OnInventoryChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }

        for (var i = 0; i < Slots.Length; i++)
            if (Slots[i] == null)
            {
                Slots[i] = new ItemSlot {ItemInfo = itemInfo, Count = 1};
                OnInventoryChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }

        return false;
    }

    public ItemInfo GetSelectedItem()
    {
        return ItemSelected ? Slots[SelectedItemInd].ItemInfo : null;
    }

    public ItemInfo PopSelectedItem()
    {
        ItemInfo removedItemInfo = null;
        if (ItemSelected)
        {
            removedItemInfo = Slots[SelectedItemInd].ItemInfo;
            if (Slots[SelectedItemInd].Count > 1)
            {
                --Slots[SelectedItemInd].Count;
            }
            else
            {
                Slots[SelectedItemInd] = null;
                ClearItemSelection();
            }

            OnInventoryChanged?.Invoke(this, EventArgs.Empty);
        }

        return removedItemInfo;
    }

    public void SelectItem(int ind)
    {
        if (ItemSelected && ind == SelectedItemInd)
        {
            ClearItemSelection();
            return;
        }

        if (Slots[ind] != null)
        {
            SelectedItemInd = ind;
            ItemSelected = true;
            OnInventoryChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void SelectNextItem()
    {
        if (!ItemSelected && Slots[SelectedItemInd] != null)
        {
            ItemSelected = true;
            OnInventoryChanged?.Invoke(this, EventArgs.Empty);
            return;
        }

        var startInd = SelectedItemInd;
        do
        {
            ++SelectedItemInd;
        } while (SelectedItemInd != startInd && Slots[SelectedItemInd] == null);

        if (SelectedItemInd != startInd)
        {
            ItemSelected = true;
            OnInventoryChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void SelectPrevItem()
    {
        if (!ItemSelected && Slots[SelectedItemInd] != null)
        {
            ItemSelected = true;
            OnInventoryChanged?.Invoke(this, EventArgs.Empty);
            return;
        }

        var startInd = SelectedItemInd;
        do
        {
            --SelectedItemInd;
        } while (SelectedItemInd != startInd && Slots[SelectedItemInd] == null);

        if (SelectedItemInd != startInd)
        {
            ItemSelected = true;
            OnInventoryChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void ClearItemSelection()
    {
        SelectedItemInd = 0;
        ItemSelected = false;
        OnInventoryChanged?.Invoke(this, EventArgs.Empty);
    }
}