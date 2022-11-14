using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ItemPickUp : MonoBehaviour
{
    public float PickUpRadius = 1f;
    public InventoryItemData ItemData;

    private SphereCollider myCollider;

    private void Awake()
    {
        myCollider = GetComponent<SphereCollider>();
        myCollider.isTrigger = true;
        myCollider.radius = PickUpRadius;
        
    }

    // When player hits the item , it will destroy and it will get add the player's inventory
    private void OnTriggerEnter(Collider other)
    {
        var inventory  = other.transform.GetComponent<InventoryHolder>();

        if (!inventory) return;

        if(inventory.InventorySystem.AddToInventory(ItemData, 1))
        {
            Destroy(this.gameObject );
        }
    }
}
