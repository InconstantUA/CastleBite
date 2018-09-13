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
    bool isDroppable = true;

    public Mode SlotMode
    {
        get
        {
            return slotMode;
        }
    }

    public HeroEquipmentSlot EquipmentSlot
    {
        get
        {
            return equipmentSlot;
        }

        set
        {
            equipmentSlot = value;
        }
    }

    public bool IsDroppable
    {
        set
        {
            isDroppable = value;
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
            return GetComponentInParent<HeroEquipment>().LUnitEquipmentButton.transform.parent.parent.GetComponent<PartyUnitUI>().LPartyUnit.transform;
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
        // verify if we are in edit party screen
        if (transform.root.Find("MiscUI").GetComponentInChildren<EditPartyScreen>(false) != null)
        {
            // move InventoryItem to the new parent object
            itemBeingDragged.LInventoryItem.transform.SetParent(GetParentObjectTransform());
        }
        // verify if we are in battle screen
        else if (transform.root.Find("MiscUI").GetComponentInChildren<BattleScreen>(false) != null)
        {
            // do not move InventoryItem to the new parent object, because this is item equipped on a hero
        }
        else
        {
            Debug.LogError("Unknonw active screen");
        }
    }

    public void MoveItemIntoSlot(InventoryItemDragHandler itemBeingDragged)
    {
        // Get source item slot transform
        InventorySlotDropHandler srcItemSlot = InventoryItemDragHandler.itemBeingDragged.ItemBeindDraggedSlot;
        // init exchange flag
        bool thisIsExachnge = false;
        // init destination slot variable with this slot
        InventorySlotDropHandler destinationSlot = this;
        // verify if there is no item already in this slot
        if (transform.childCount > 1)
        {
            // verify if source is equipment slot and destination is party inventory slot
            if ((srcItemSlot.SlotMode == Mode.HeroEquipment) && (slotMode == Mode.PartyInventory))
            {
                // do not do exchange, just move item into inventory into the new slot
                // create new slot and set it as destination
                destinationSlot = GetComponentInParent<PartyInventoryUI>().AddSlot();
            }
            else
            {
                thisIsExachnge = true;
                // Put item from this slot to the slot of the item beind dragged
                srcItemSlot.PutItemIntoSlot(GetComponentInChildren<InventoryItemDragHandler>());
            }
            // trigger event
            // ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject, null, (x, y) => x.HasChanged());
        }
        // Put dragged item into slot
        destinationSlot.PutItemIntoSlot(InventoryItemDragHandler.itemBeingDragged);
        // verify if it was not just simple exchange
        if (!thisIsExachnge)
        {
            // verify if source slot is in party inventory mode
            if (srcItemSlot.SlotMode == Mode.PartyInventory)
            {
                // .. Optimize
                // remove all empty slots in inventory
                srcItemSlot.GetComponentInParent<PartyInventoryUI>().RemoveAllEmptySlots();
                // fill in empty slots in inventory
                srcItemSlot.GetComponentInParent<PartyInventoryUI>().FillInEmptySlots();
            }
            // verify if this is equipment slot
            else if ((srcItemSlot.SlotMode == Mode.HeroEquipment)
                && (transform.root.Find("MiscUI").GetComponentInChildren<BattleScreen>(false) != null))
            // and that battle screen is active
            {
                // remove all empty slots in inventory
                srcItemSlot.GetComponentInParent<PartyInventoryUI>().RemoveAllEmptySlots();
                // fill in empty slots in inventory
                srcItemSlot.GetComponentInParent<PartyInventoryUI>().FillInEmptySlots();
            }
            // verify if destination slot was changed to the other from this slot
            if (destinationSlot.gameObject.GetInstanceID() != gameObject.GetInstanceID())
            {
                // trigger this party inventory reorganisation
                // remove all empty slots in this inventory
                GetComponentInParent<PartyInventoryUI>().RemoveAllEmptySlots();
                // fill in empty slots in inventory
                GetComponentInParent<PartyInventoryUI>().FillInEmptySlots();
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        // verify if it is item being dragged and not other object
        if (InventoryItemDragHandler.itemBeingDragged != null)
        {
            // verify if slot is droppable for this item (this is being set during drag initiation)
            if (isDroppable)
            {
                // move item into slot
                MoveItemIntoSlot(InventoryItemDragHandler.itemBeingDragged);
                // verify if we are in edit screen mode
                if (transform.root.Find("MiscUI").GetComponentInChildren<EditPartyScreen>(false) != null)
                {
                    // verify if this is hero eqiupment slot
                    if (Mode.HeroEquipment == slotMode)
                    {
                        // change item position parameter
                        InventoryItemDragHandler.itemBeingDragged.LInventoryItem.HeroEquipmentSlot = equipmentSlot;
                        // update unit info UI
                        transform.root.Find("MiscUI/UnitInfoPanel").GetComponent<UnitInfoPanel>().ActivateAdvance(GetComponentInParent<HeroEquipment>().LPartyUnit, UnitInfoPanel.Align.Right, false, UnitInfoPanel.Mode.Short);
                    }
                    else
                    {
                        // change item position parameter
                        InventoryItemDragHandler.itemBeingDragged.LInventoryItem.HeroEquipmentSlot = HeroEquipmentSlot.None;
                    }
                }
                // verify if we are in battle screen mode
                else if (transform.root.Find("MiscUI").GetComponentInChildren<BattleScreen>(false) != null)
                {
                    // take equipment slot parameter from item
                    equipmentSlot = InventoryItemDragHandler.itemBeingDragged.LInventoryItem.HeroEquipmentSlot;
                }
                else
                {
                    Debug.LogError("Unknown active screen");
                }
            }
        }
    }

    //void OnTransformChildrenChanged()
    //{
    //    Debug.Log("On change");
    //}

    //public void OnDrop(PointerEventData eventData)
    //{
    //    RectTransform invPanel = transform as RectTransform;
    //    if(!RectTransformUtility.RectangleContainsScreenPoint(invPanel,Input.mousePosition))
    //    {
    //        Debug.Log("Drop item");
    //    }
    //}
}
