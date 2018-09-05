using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryPanelDropHandler : MonoBehaviour, IDropHandler
{
    InventorySlotDropHandler GetEmptySlot()
    {
        foreach(InventorySlotDropHandler slot in GetComponentsInChildren<InventorySlotDropHandler>())
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
        if (InventoryItemDragHandler.itemBeingDragged != null)
        {
            Debug.Log("Find or create the right slot and put an item into it");
            // Get empty slot
            InventorySlotDropHandler itemSlot = GetEmptySlot();
            // verify if slot is not null
            if (itemSlot == null)
            {
                // get PartyInventoryUI
                PartyInventoryUI partyInventoryUI = GetComponentInParent<PartyInventoryUI>();
                // create new slot
                itemSlot = partyInventoryUI.AddSlot();
            }
            // move item drag handler into slot
            itemSlot.MoveItemIntoSlot(InventoryItemDragHandler.itemBeingDragged);
        }
    }
}
