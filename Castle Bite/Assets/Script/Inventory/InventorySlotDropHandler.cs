using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InventorySlotDropHandler: ItemSlotDropHandler
{
    public override Transform GetParentObjectTransform()
    {
        // get HeroParty
        Debug.Log("My parent is " + transform.parent.name);
        Debug.Log("My grandparent is " + transform.parent.parent.name);
        return GetComponentInParent<HeroPartyUI>().LHeroParty.transform;
    }

    public override void MoveItemIntoThisSlot()
    {
        // Get source item slot transform
        ItemSlotDropHandler srcItemSlot = InventoryItemDragHandler.itemBeingDragged.ItemBeindDraggedSlot;
        // init exchange flag
        //bool thisIsExachnge = false;
        // init destination slot variable with this slot
        ItemSlotDropHandler dstItemSlot = this;
        // Get item in this slot
        InventoryItemDragHandler itemInThisSlot = GetComponentInChildren<InventoryItemDragHandler>();
        // verify if there is no item already in this slot
        if (itemInThisSlot != null)
        {
            // verify if source is equipment slot and destination is party inventory slot
            if (srcItemSlot is EquipmentSlotDropHandler)
            {
                // do not do exchange, just move item into inventory into the new slot
                // create new slot and set it as destination
                // reasons: 
                //  we don't want to accidentally exchange boots with sword => in this case sword will appear in boots slot
                //  we don't want to accidentally exchange with the item which is cannot be used by this hero due to requirements (skills) not met.
                dstItemSlot = GetComponentInParent<PartyInventoryUI>().AddSlot();
                // Put dragged item into new slot
                dstItemSlot.PutItemIntoSlot(InventoryItemDragHandler.itemBeingDragged);
            }
            else
            {
                //thisIsExachnge = true;
                // Put item from this slot to the slot of the item beind dragged
                srcItemSlot.ExchangeWithSlotOnDrop(this);
            }
        }
        else
        {
            // Put dragged item into this slot
            dstItemSlot.PutItemIntoSlot(InventoryItemDragHandler.itemBeingDragged);
        }
        //// verify if it was not just simple exchange
        //if (!thisIsExachnge)
        //{
        //    // verify if source slot is in party inventory mode
        //    if ( (srcItemSlot is InventorySlotDropHandler)
        //    // OR verify if source slot is equipment slot
        //     || ( (srcItemSlot is EquipmentSlotDropHandler)
        //        // and that battle screen is active
        //        && (transform.root.Find("MiscUI").GetComponentInChildren<BattleScreen>(false) != null) ) )
        //    {
        //        // .. Optimize
        //        // Get source slot PartyInventoryUI (before slot is removed)
        //        PartyInventoryUI partyInventoryUI = srcItemSlot.GetComponentInParent<PartyInventoryUI>();
        //        // remove all empty slots in inventory
        //        partyInventoryUI.RemoveAllEmptySlots();
        //        // fill in empty slots in inventory
        //        partyInventoryUI.FillInEmptySlots();
        //    }
        //    // verify if destination slot was changed to the other from this slot
        //    if (dstItemSlot.gameObject.GetInstanceID() != gameObject.GetInstanceID())
        //    {
        //        // trigger this party inventory reorganisation
        //        // remove all empty slots in this inventory
        //        GetComponentInParent<PartyInventoryUI>().RemoveAllEmptySlots();
        //        // fill in empty slots in inventory
        //        GetComponentInParent<PartyInventoryUI>().FillInEmptySlots();
        //    }
        //}
    }
    
}
