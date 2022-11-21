using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot : ItemSlot
{
    public InventorySlot(InventoryItemData source, int amount) // Constructor to make an occupied inventory slot.
    {
        itemData = source;
        _itemID = itemData.ID;
        stackSize = amount;
    }

    public InventorySlot() // Constructor to make an empty inventory slot.
    {
        ClearSlot();
    }

    public void UpdateInventorySlot(InventoryItemData data, int amount) // Updates slot directly.
    {
        itemData = data;
        _itemID = itemData.ID;
        stackSize = amount;
    }

    // To check how many item added until max size of slot and others are still in your inventory
    public bool EnoughRoomLeftInStack(int amountToAdd, out int amountRemaining)
    {
        amountRemaining = ItemData.MaxStackSize - stackSize;
        return EnoughRoomLeftInStack(amountToAdd);

    }
    public bool EnoughRoomLeftInStack(int amountToAdd)
    {
        if (itemData == null || stackSize + amountToAdd <= itemData.MaxStackSize) return true;
        else return false;
    }



    public bool SplitStack(out InventorySlot splitStack)
    {
        if (stackSize <= 1) // Is there enough to actually split? If not return false.
        {
            splitStack = null;
            return false;
        }

        int halfStack = Mathf.RoundToInt(stackSize / 2); // Get half the stack
        RemoveFromStack(halfStack);

        splitStack = new InventorySlot(ItemData, halfStack); // Creates aa copy of this slot with half the stack size.
        return true;
    }
}
