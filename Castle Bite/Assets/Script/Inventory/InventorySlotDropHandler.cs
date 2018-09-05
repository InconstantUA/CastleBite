using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotDropHandler : MonoBehaviour, IDropHandler {
    public enum Mode
    {
        PartyInventory,
        HeroEquipment
    }
    [SerializeField]
    Mode slotMode;
    [SerializeField]
    HeroEquipmentSlot equipmentSlot;

    public Mode SlotMode
    {
        get
        {
            return slotMode;
        }
    }

    //GameObject Item
    //{
    //    get
    //    {
    //        if (transform.childCount > 1)
    //        {
    //            return transform.GetChild(1).gameObject;
    //        }
    //        return null;
    //    }
    //}

    public Transform GetParentObjectTransform()
    {
        // verify if this hero Equipment or Party inventory slot
        if (slotMode == Mode.HeroEquipment)
        {
            // item being dragged to the party leader (hero)
            // get hero (party leader)
            // structure: 2UnitCanvas[PartyUnitUI->LPartyUnit]-1UnitEquipmentControl-EquipmentButton
            return GetComponentInParent<HeroEquipment>().CallingUnitEquipmentButton.transform.parent.parent.GetComponent<PartyUnitUI>().LPartyUnit.transform;
        }
        else if (slotMode == Mode.PartyInventory)
        {
            // get HeroParty
            return GetComponentInParent<HeroPartyUI>().LHeroParty.transform;
        } else
        {
            Debug.LogError("Unknown slot mode: " + slotMode.ToString());
            return null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (InventoryItemDragHandler.itemBeingDragged != null)
        {
            // Get source item slot transform
            Transform srcItemParentTransform = InventoryItemDragHandler.itemBeingDragged.GetComponent<InventoryItemDragHandler>().StartParentTransform;
            // init exchange flag
            bool thisIsExachnge = false;
            // verify if there is no item already in this slot
            if (transform.childCount > 1)
            {
                thisIsExachnge = true;
                // there is already item in the slot
                // exchange 2 items between each other
                // Get InventoryItemDragHandler in this slot
                InventoryItemDragHandler dstItemDragHandler = GetComponentInChildren<InventoryItemDragHandler>();
                // Get destination item
                InventoryItem dstInventoryItem = dstItemDragHandler.LInventoryItem;
                // Get slot of the item being dragged
                InventorySlotDropHandler inventorySlotOfTheItemBeingDragged = srcItemParentTransform.GetComponent<InventorySlotDropHandler>();
                // move item from this slot to the parent of slot of the item being dragged
                dstInventoryItem.transform.SetParent(inventorySlotOfTheItemBeingDragged.GetParentObjectTransform());
                // move item in this slot to the src item parent
                dstItemDragHandler.transform.SetParent(srcItemParentTransform);
                // reset its position to 0/0/0/0
                dstItemDragHandler.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0); // [ left - bottom ]
                dstItemDragHandler.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0); // [ right - top ]
                // trigger event
                // ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject, null, (x, y) => x.HasChanged());
            }
            else
            {
                // ..
            }
            // move item being dragged UI into this slot
            InventoryItemDragHandler.itemBeingDragged.transform.SetParent(transform);
            // reset UI position to 0/0/0/0
            InventoryItemDragHandler.itemBeingDragged.transform.gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0); // [ left - bottom ]
            InventoryItemDragHandler.itemBeingDragged.transform.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0); // [ right - top ]
            // Get InventoryItem
            InventoryItem inventoryItem = InventoryItemDragHandler.itemBeingDragged.GetComponent<InventoryItemDragHandler>().LInventoryItem;
            // move item to new parent
            inventoryItem.transform.SetParent(GetParentObjectTransform());
            // verify if it was not just simple exchange
            if (!thisIsExachnge)
            {
                // verify if source slot is in party inventory mode
                if (srcItemParentTransform.GetComponent<InventorySlotDropHandler>().SlotMode == Mode.PartyInventory)
                {
                    // .. Optimize
                    // there might be empty slot appear
                    // possible options
                    // 000  - ok
                    // x00  - ok
                    // xx0  - ok
                    // 0xx  - not ok
                    // x0x  - not ok
                    // xxx0 - not ok
                    // we need to remove them
                    // Get parent slots grid
                    Transform srcSlotsGrid = srcItemParentTransform.parent;
                    // get all slots
                    InventorySlotDropHandler[] srcSlots = srcSlotsGrid.GetComponentsInChildren<InventorySlotDropHandler>();
                    // loop through all slots in source inventory
                    foreach (InventorySlotDropHandler slot in srcSlots)
                    {
                        // verify if slot is empty
                        if (slot.GetComponentInChildren<InventoryItemDragHandler>() == null)
                        {
                            // Remove slot
                            Destroy(slot.gameObject);
                        }
                    }
                    // fill in empty slots in inventory
                    srcItemParentTransform.GetComponentInParent<PartyInventoryUI>().FillInEmptySlots();
                }
            }
        }
    }

    //public void OnDrop(PointerEventData eventData)
    //{
    //    RectTransform invPanel = transform as RectTransform;
    //    if(!RectTransformUtility.RectangleContainsScreenPoint(invPanel,Input.mousePosition))
    //    {
    //        Debug.Log("Drop item");
    //    }
    //}
}
