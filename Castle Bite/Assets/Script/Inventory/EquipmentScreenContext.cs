using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentScreenContext : Singleton<EquipmentScreenContext>
{
    // unit which is being modified
    public static PartyUnit PartyUnitBeingModified { get; set; }

    // inventory item which has been used
    public static InventoryItem ItemBeingUsed { get; set; }

    // validated item slot
    public static ItemSlotDropHandler DestinationItemSlotDropHandler { get; set; }

    public void OnBeginItemDrag()
    {
        // save item being used in cache
        ItemBeingUsed = InventoryItemDragHandler.itemBeingDragged.LInventoryItem;
    }

    public void OnEndItemDrag()
    {
        // reset item being used
        ItemBeingUsed = null;
    }

    public static void Reset()
    {
        PartyUnitBeingModified = null;
        ItemBeingUsed = null;
    }
}
