using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class InventoryDisplay : MonoBehaviour
{
    [SerializeField] MouseItemData mouseInventoryItem;
    protected InventorySystem inventorySystem;
    protected Dictionary<InventorySlots_UI, InventorySlot> slotDictionary; // Pair up the UI slots with the system slots.
    public InventorySystem InventorySystem => inventorySystem;
    public Dictionary<InventorySlots_UI, InventorySlot> SlotDictionary => slotDictionary;

    protected virtual void Start()
    {

    }

    public abstract void AssignSlot(InventorySystem invToDisplay); // Implemented in child classes.

    protected virtual void UpdateSlot(InventorySlot updatedSlot)
    {
        foreach (var slot in SlotDictionary)
        {
            if(slot.Value == updatedSlot) // Slot value - the "under the hood" inventory slot
            {
                slot.Key.UpdateUISlot(updatedSlot);// Slot key - the UI representation of the value
            }
        }
    }

    // What happened when clicking Slots
    public void SlotClicked(InventorySlots_UI clickedUISlot)
    {
        bool isShiftPressed = Keyboard.current.leftShiftKey.isPressed;

        // Clicked slot has an item - mouse doesn't have an item - pick up that item
        if (clickedUISlot.AssignedInventorySlot.ItemData != null && mouseInventoryItem.AssignedInventorySlot.ItemData == null)
        {
            // If the player is holding shift key? Split the stack.
            if(isShiftPressed && clickedUISlot.AssignedInventorySlot.SplitStack(out InventorySlot halfStackSlot)) // split stack
            {
                mouseInventoryItem.UpdateMouseSlot(halfStackSlot);
                clickedUISlot.UpdateUISlot();
                return;
            }
            else // Pick up the item in the clicked slot.
            {
                mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);
                clickedUISlot.ClearSlot();
                return;
            }
        }

        // Clicked slot doesn't have an item - Mouse does have an item - place the mouse item into the empty slot.
        if (clickedUISlot.AssignedInventorySlot.ItemData == null && mouseInventoryItem.AssignedInventorySlot.ItemData != null)
        {
            clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
            clickedUISlot.UpdateUISlot();

            mouseInventoryItem.ClearSlot();
            return;
        }

        // Both slots have an item - decide what to do..
        if (clickedUISlot.AssignedInventorySlot.ItemData != null && mouseInventoryItem.AssignedInventorySlot.ItemData != null)
        {
            bool isSameItem = clickedUISlot.AssignedInventorySlot.ItemData == mouseInventoryItem.AssignedInventorySlot.ItemData;
            // If the items of clicked slot and mouse slot are same AND the clicked slot's stack size + mouse slot's stack size is LESS than its max size;
            if (isSameItem && clickedUISlot.AssignedInventorySlot.EnoughRoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize))
            {
                // Assign items to clicked slots when it reaches to its max stack size
                clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
                clickedUISlot.UpdateUISlot();

                mouseInventoryItem.ClearSlot();   
                return;
            }
            // If the items of clicked slot and mouse slot are same AND the clicked slot's stack size + mouse slot's stack size is MORE than its max size;
            else if (isSameItem && !clickedUISlot.AssignedInventorySlot.EnoughRoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize, out int leftInStack))
            {
                if (leftInStack < 1) SwapSlots(clickedUISlot); // Stack is full so swap the items.
                else // slot is not at max, so what's need the mouse inventory
                {
                    int remainingOnMouse = mouseInventoryItem.AssignedInventorySlot.StackSize - leftInStack;
                    
                    clickedUISlot.AssignedInventorySlot.AddToStack(leftInStack);
                    clickedUISlot.UpdateUISlot();

                    var newItem = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemData, remainingOnMouse);
                    mouseInventoryItem.ClearSlot();
                    mouseInventoryItem.UpdateMouseSlot(newItem);
                    return;
                }
            }
            // If the items of clicked slots and mouse slot are Different
            else if (!isSameItem)
            {
                SwapSlots(clickedUISlot);
                return;
            }
        }        
    }

    // Swapping items between clicked slot and mouse slot when both has an item.
    private void SwapSlots(InventorySlots_UI clickedUISlot)
    {
        var clonedSlot = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemData, mouseInventoryItem.AssignedInventorySlot.StackSize);
        mouseInventoryItem.ClearSlot();

        // Assign the clicked item to mouse slot
        mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);

        // Assign the mouse item to clicked slot
        clickedUISlot.ClearSlot();
        clickedUISlot.AssignedInventorySlot.AssignItem(clonedSlot);
        clickedUISlot.UpdateUISlot();

    }
}
