using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot : ISerializationCallbackReceiver
{
    [NonSerialized] private InventoryItemData itemData; // Reference to the data
    [SerializeField] private int _itemID = -1;
    [SerializeField] private int stackSize; // Current stack size - how many of the data do we have

    public InventoryItemData ItemData => itemData;
    public int StackSize => stackSize;

    public InventorySlot (InventoryItemData source, int amount) // Constructor to make an occupied inventory slot.
    {
        itemData = source;
        _itemID = itemData.ID;
        stackSize = amount;
    }

    public InventorySlot() // Constructor to make an empty inventory slot.
    {
        ClearSlot();
    }

    public void ClearSlot() // Clears the slot
    {
        itemData = null;
        _itemID = -1;
        stackSize = -1;
    }

    public void AssignItem(InventorySlot invSlot) // Assigns an item to the slot
    {
        if (itemData == invSlot.itemData) AddToStack(invSlot.stackSize); // does the slot contains the same item ? Add to stack if so.
        else // Overwrite slot with the inventory slot that we're passing in.
        {
            itemData = invSlot.itemData;
            _itemID = itemData.ID;
            stackSize = 0;
            AddToStack(invSlot.stackSize);
        }

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

    // Increase stack size with adding Item.
    public void AddToStack(int amount)
    {
        stackSize += amount;
    }

    // Decrease stack size with removing Item.
    public void RemoveFromStack(int amount)
    {
        stackSize -= amount;
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

    public void OnBeforeSerialize()
    {

    }

    public void OnAfterDeserialize()
    {
        if (_itemID == -1) return;

        var db = Resources.Load<Database>("Database");
        itemData = db.GetItem(_itemID);
    }
}
