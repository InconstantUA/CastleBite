using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class EquipmentSlotDropHandler : ItemSlotDropHandler
{
    public override Transform GetParentObjectTransform()
    {
        // item being dragged to the party leader (hero)
        // get hero (party leader)
        // structure: 2UnitCanvas[PartyUnitUI->LPartyUnit]-1UnitEquipmentControl-EquipmentButton
        return GetComponentInParent<HeroEquipment>().LUnitEquipmentButton.transform.parent.parent.GetComponent<PartyUnitUI>().LPartyUnit.transform;
    }

    public override void MoveItemIntoThisSlot()
    {
        // Get source item slot transform
        ItemSlotDropHandler srcItemSlot = InventoryItemDragHandler.itemBeingDragged.ItemBeindDraggedSlot;
        // init exchange flag
        //bool thisIsExachnge = false;
        // Get item in this slot
        InventoryItemDragHandler itemInThisSlot = GetComponentInChildren<InventoryItemDragHandler>();
        // verify if there is no item already in this slot
        if (itemInThisSlot != null)
        {
            //thisIsExachnge = true;
            // Put item from this slot to the slot of the item beind dragged
            srcItemSlot.PutItemIntoSlot(itemInThisSlot);
        }
        // Put dragged item into this slot
        PutItemIntoSlot(InventoryItemDragHandler.itemBeingDragged);
        //// verify if it was not exchange between 2 slots (means that we might need to fill in empty slots in source PartyInventoryUI)
        //if (!thisIsExachnge)
        //{
        //    // verify if source slot is in party inventory mode
        //    if ((srcItemSlot is InventorySlotDropHandler)
        //    // OR verify if source slot is equipment slot
        //     || ((srcItemSlot is EquipmentSlotDropHandler)
        //        // and that battle screen is active
        //        && (transform.root.Find("MiscUI").GetComponentInChildren<BattleScreen>(false) != null)))
        //    {
        //        // .. Optimize
        //        // Get source slot PartyInventoryUI (before slot is removed)
        //        PartyInventoryUI partyInventoryUI = srcItemSlot.GetComponentInParent<PartyInventoryUI>();
        //        // remove all empty slots in inventory
        //        partyInventoryUI.RemoveAllEmptySlots();
        //        // fill in empty slots in inventory
        //        partyInventoryUI.FillInEmptySlots();
        //    }
        //}
    }

}
