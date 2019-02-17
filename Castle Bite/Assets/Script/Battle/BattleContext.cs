using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleContext : Singleton<BattleContext>
{
    public static void Reset()
    {
        ActivePartyUnitUI = null;
        ActivatedUPMConfigIndex = 0;
        TargetedUnitSlot = null;
        DestinationUnitSlot = null;
        UniquePowerModifierID = null;
        ItemBeingUsed = null;
    }

    // unit which has been given a turn in battle
    public static PartyUnitUI ActivePartyUnitUI { get; set; }

    // configuration of activated unique power modifier (there can be more than one UPM defined in unit's ability, so we need to keep track of which one is used now)
    public static int ActivatedUPMConfigIndex { get; set; }

    // id of activated unique power modifier
    public static UniquePowerModifierID UniquePowerModifierID { get; set; }

    // unit which has been targeted by active unit's ability or item
    public static UnitSlot TargetedUnitSlot { get; set; }

    // id which can be used to uniquely identify UPM
    public static UnitSlot DestinationUnitSlot { get; set; }

    // inventory item which has been used
    public static InventoryItem ItemBeingUsed { get; set; }

    public void OnBattleNewUnitHasBeenActivatedEvent(System.Object context)
    {
        Debug.LogWarning("Set ActivePartyUnitUI context");
        // verify if context is correct
        if (context is PartyUnitUI)
        {
            // reset battle context values (we don't want to have previously cached targeted party unit slot and upm index)
            Reset();
            // cache active unit in battle context
            ActivePartyUnitUI = (PartyUnitUI)context;
        }
    }

    public void OnBattleApplyActiveUnitAbility(System.Object context)
    {
        Debug.LogWarning("Set TargetedUnitSlot context");
        // verify if context is correct
        if (context is UnitSlot)
        {
            // cache target unit slot in battle context (unit which has been targeted by ability)
            TargetedUnitSlot = (UnitSlot)context;
        }
    }

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

}
