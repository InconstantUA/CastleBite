using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditPartyScreenContext : Singleton<EditPartyScreenContext>
{
    // inventory item which has been used
    public static InventoryItem ItemBeingUsed { get; set; }

    // unit which has been targeted by active unit's ability or item
    public static UnitSlot TargetedUnitSlot { get; set; }

    // id which can be used to uniquely identify UPM
    public static UnitSlot DestinationUnitSlot { get; set; }

    // id of activated unique power modifier (for Apply())
    public static UniquePowerModifierID UniquePowerModifierID { get; set; }

    // configuration of activated unique power modifier (there can be more than one UPM defined in unit's ability, so we need to keep track of which one is used now)
    public static int ActivatedUPMConfigIndex { get; set; }

    public void OnBeginItemDrag()
    {
        // Reset targeted unit slot
        TargetedUnitSlot = null;
        // save item being used in cache
        ItemBeingUsed = InventoryItemDragHandler.itemBeingDragged.LInventoryItem;
    }

    public void OnEndItemDrag()
    {
        // reset item being used
        ItemBeingUsed = null;
        // Reset targeted unit slot
        TargetedUnitSlot = null;
    }

    public void OnItemHasBeenDroppedIntoTheUnitSlot(System.Object context)
    {
        // verify if context is wrong
        if (!(context is UnitSlotDropHandler))
        {
            Debug.LogError("Context is not PartyUnit");
            // exit
            return;
        }
        // cache target unit slot in context (unit which has been targeted)
        TargetedUnitSlot = ((UnitSlotDropHandler)context).GetComponent<UnitSlot>();
    }
}
