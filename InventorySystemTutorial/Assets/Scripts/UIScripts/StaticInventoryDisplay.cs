using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticInventoryDisplay : InventoryDisplay
{
    [SerializeField] private PlayerInventoryHolder playerInventoryHolder;
    [SerializeField] private InventorySlots_UI[] slots;

    protected override void Start()
    {
        base.Start();

        if(playerInventoryHolder != null)
        {
            inventorySystem = playerInventoryHolder.PrimaryInventorySystem;
            inventorySystem.OnInventorySlotChanged += UpdateSlot;
        }
        else
        {
            Debug.LogWarning($"No inventory assigned to {this.gameObject}");
        }

        AssignSlot(inventorySystem);
    }

    public override void AssignSlot(InventorySystem invToDisplay)
    {
        slotDictionary = new Dictionary<InventorySlots_UI, InventorySlot>();

        if(slots.Length != inventorySystem.InventorySize) Debug.Log($"Inventory slots out of sync on  {this.gameObject}");

        for (int i = 0; i < inventorySystem.InventorySize; i++)
        {
            slotDictionary.Add(slots[i], inventorySystem.InventorySlots[i]);
            slots[i].Init(inventorySystem.InventorySlots[i]);
        }
    }
}
