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

    public void PutItemIntoSlot(InventoryItemDragHandler itemBeingDragged)
    {
        // move item being dragged UI into this slot
        itemBeingDragged.transform.SetParent(transform);
        // reset UI position to 0/0/0/0
        itemBeingDragged.transform.gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0); // [ left - bottom ]
        itemBeingDragged.transform.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0); // [ right - top ]
        // move InventoryItem to the new parent
        itemBeingDragged.LInventoryItem.transform.SetParent(GetParentObjectTransform());
    }

    public void MoveItemIntoSlot(InventoryItemDragHandler itemBeingDragged)
    {
        // Get source item slot transform
        InventorySlotDropHandler srcItemSlot = InventoryItemDragHandler.itemBeingDragged.ItemBeindDraggedSlot;
        // init exchange flag
        bool thisIsExachnge = false;
        // verify if there is no item already in this slot
        if (transform.childCount > 1)
        {
            thisIsExachnge = true;
            // Put item from this slot to the slot of the item beind dragged
            srcItemSlot.PutItemIntoSlot(GetComponentInChildren<InventoryItemDragHandler>());
            // trigger event
            // ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject, null, (x, y) => x.HasChanged());
        }
        // Put dragged item into slot
        PutItemIntoSlot(InventoryItemDragHandler.itemBeingDragged);
        // verify if it was not just simple exchange
        if (!thisIsExachnge)
        {
            // verify if source slot is in party inventory mode
            if (srcItemSlot.SlotMode == Mode.PartyInventory)
            {
                // .. Optimize
                // Get parent slots grid
                Transform srcSlotsGrid = srcItemSlot.transform.parent;
                // loop through all slots in source inventory
                foreach (InventorySlotDropHandler slot in srcSlotsGrid.GetComponentsInChildren<InventorySlotDropHandler>())
                {
                    // verify if slot is empty
                    if (slot.GetComponentInChildren<InventoryItemDragHandler>() == null)
                    {
                        // Remove slot
                        Destroy(slot.gameObject);
                    }
                }
                // fill in empty slots in inventory
                srcItemSlot.GetComponentInParent<PartyInventoryUI>().FillInEmptySlots();
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (InventoryItemDragHandler.itemBeingDragged != null)
        {
            MoveItemIntoSlot(InventoryItemDragHandler.itemBeingDragged);
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
