using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryPanelDropHandler : MonoBehaviour, IDropHandler
{
    ItemSlotDropHandler GetEmptySlot()
    {
        foreach(ItemSlotDropHandler slot in GetComponentsInChildren<ItemSlotDropHandler>())
        {
            // verify if slot is empty
            if (slot.GetComponentInChildren<InventoryItemDragHandler>() == null)
            {
                return slot;
            }
        }
        // slot not found
        return null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        // verify if item being dragged is not null
        if ((InventoryItemDragHandler.itemBeingDragged != null)
        // verify if we are in edit screen mode
        && (transform.root.Find("MiscUI").GetComponentInChildren<EditPartyScreen>(false) != null))
        {
            Debug.Log("Find or create the right slot and put an item into it");
            // Get empty slot
            ItemSlotDropHandler itemSlot = GetEmptySlot();
            // verify if slot is not null
            if (itemSlot == null)
            {
                // get PartyInventoryUI
                PartyInventoryUI partyInventoryUI = GetComponentInParent<PartyInventoryUI>();
                // create new slot
                itemSlot = partyInventoryUI.AddSlot();
            }
            // move item drag handler into slot
            itemSlot.MoveItemIntoThisSlot();
        }
    }
}
