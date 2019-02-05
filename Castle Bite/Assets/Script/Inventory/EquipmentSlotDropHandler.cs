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

    public override void PutItemIntoSlot(InventoryItemDragHandler itemBeingDragged)
    {
        base.PutItemIntoSlot(itemBeingDragged);
        itemHasBeenDroppedIntoTheItemSlotEvent.Raise(this);
    }

    public override void MoveItemIntoThisSlot()
    {
        // Get source item slot transform
        ItemSlotDropHandler srcItemSlot = InventoryItemDragHandler.itemBeingDragged.ItemBeindDraggedSlot;
        // init exchange flag
        bool thisIsExachnge = false;
        // Get item in this slot
        InventoryItemDragHandler itemInThisSlot = GetComponentInChildren<InventoryItemDragHandler>();
        // verify if there is no item already in this slot
        if (itemInThisSlot != null)
        {
            thisIsExachnge = true;
            // Put item from this slot to the slot of the item beind dragged
            srcItemSlot.PutItemIntoSlot(itemInThisSlot);
        }
        // Put dragged item into this slot
        PutItemIntoSlot(InventoryItemDragHandler.itemBeingDragged);
        // verify if it was not just simple exchange
        if (!thisIsExachnge)
        {
            // verify battle screen is active
            if (transform.root.Find("MiscUI").GetComponentInChildren<BattleScreen>(false) != null)
            {
                // remove all empty slots in inventory
                srcItemSlot.GetComponentInParent<PartyInventoryUI>().RemoveAllEmptySlots();
                // fill in empty slots in inventory
                srcItemSlot.GetComponentInParent<PartyInventoryUI>().FillInEmptySlots();
            }
        }
    }

}
